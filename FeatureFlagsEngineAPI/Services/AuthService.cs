using FeatureFlagsEngineAPI.Interfaces.DataLayerInterfaces;
using FeatureFlagsEngineAPI.Models;
using Microsoft.Data.SqlClient;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.IO;
using System.Security.Claims;
using System.Text;

namespace FeatureFlagsEngineAPI.Services
{
    public class AuthService : IAuthService
    {
        private readonly IConfiguration _configuration;
        private readonly string _connectionString;

        public AuthService(IConfiguration configuration)
        {
            _configuration = configuration;
            _connectionString = _configuration.GetConnectionString("DefaultConnection")
                ?? throw new InvalidOperationException("Connection string not found.");
        }

        public AuthResponse Login(string email, string password)
        {
            try
            {
                using var connection = new SqlConnection(_connectionString);
                connection.Open();

                string userQuery = @"SELECT idUser, UserName, UserEmail, IsActive 
                            FROM tblUsers 
                            WHERE UserEmail = @UserEmail AND IsActive = 1";

                using var userCommand = new SqlCommand(userQuery, connection);
                userCommand.Parameters.AddWithValue("@UserEmail", email);

                using var reader = userCommand.ExecuteReader();

                if (!reader.Read())
                {
                    return new AuthResponse
                    {
                        Success = false,
                        Message = "Invalid email or password."
                    };
                }

                var user = new UserInfo
                {
                    IdUser = reader.GetInt32(0),
                    UserName = reader.GetString(1),
                    Email = reader.GetString(2)
                };

                reader.Close();

                string rolesQuery = @"
            SELECT r.idRole, r.RoleName, r.Description
            FROM tblUserRoles ur
            INNER JOIN Roles r ON ur.RoleId = r.idRole
            WHERE ur.UserId = @UserId AND ur.IsActive = 1 AND r.IsActive = 1";

                using var rolesCommand = new SqlCommand(rolesQuery, connection);
                rolesCommand.Parameters.AddWithValue("@UserId", user.IdUser);

                using var rolesReader = rolesCommand.ExecuteReader();
                while (rolesReader.Read())
                {
                    var role = new RoleInfo
                    {
                        IdRole = rolesReader.GetInt32(0),
                        RoleName = rolesReader.GetString(1),
                        Description = rolesReader.IsDBNull(2) ? null : rolesReader.GetString(2)
                    };
                    user.AllRoles.Add(role);

                    if (user.RoleId == null)
                    {
                        user.RoleId = role.IdRole;
                        user.RoleName = role.RoleName;
                    }
                }
                rolesReader.Close();

                string directPermQuery = @"
    SELECT p.IdPermission, p.PermissionName, p.PermissionCode
    FROM UserPermission up
    INNER JOIN tblPermissions p ON up.PermissionId = p.IdPermission
    WHERE up.UserId = @UserId AND up.IsActive = 1 AND p.IsActive = 1";

                using var directPermCommand = new SqlCommand(directPermQuery, connection);
                directPermCommand.Parameters.AddWithValue("@UserId", user.IdUser);

                using var directPermReader = directPermCommand.ExecuteReader();
                while (directPermReader.Read())
                {
                    user.DirectPermissions.Add(new PermissionInfo
                    {
                        IdPermission = directPermReader.GetInt32(0),
                        PermissionName = directPermReader.GetString(1),
                        PermissionCode = directPermReader.IsDBNull(2) ? null : directPermReader.GetString(2)
                    });
                }
                directPermReader.Close();

                if (user.AllRoles.Count > 0)
                {
                    var roleIds = user.AllRoles.Select(r => r.IdRole).ToList();
                    string roleIdsParam = string.Join(",", roleIds);

                    string rolePermQuery = $@"
                SELECT p.IdPermission, p.PermissionName, p.PermissionCode
                FROM RolePermission rp
                INNER JOIN tblPermissions p ON rp.PermissionId = p.IdPermission
                WHERE rp.RoleId IN ({roleIdsParam}) AND rp.IsActive = 1 AND p.IsActive = 1";

                    using var rolePermCommand = new SqlCommand(rolePermQuery, connection);
                    using var rolePermReader = rolePermCommand.ExecuteReader();
                    while (rolePermReader.Read())
                    {
                        user.RolePermissions.Add(new PermissionInfo
                        {
                            IdPermission = rolePermReader.GetInt32(0),
                            PermissionName = rolePermReader.GetString(1),
                            PermissionCode = rolePermReader.IsDBNull(2) ? null : rolePermReader.GetString(2)
                        });
                    }
                }

                var allPermissionCodes = user.DirectPermissions
                    .Select(p => p.PermissionCode)
                    .Concat(user.RolePermissions.Select(p => p.PermissionCode))
                    .Where(c => !string.IsNullOrEmpty(c))
                    .Distinct()
                    .ToList();

                user.AllPermissions = allPermissionCodes;

                string token = GenerateJwtToken(user);

                return new AuthResponse
                {
                    Success = true,
                    Message = "Login successful.",
                    Token = token,
                    User = user
                };
            }
            catch (Exception ex)
            {
                Console.Error.WriteLine($"Login error: {ex.Message}");
                return new AuthResponse
                {
                    Success = false,
                    Message = "An error occurred during login."
                };
            }
        }

        public void Logout(int userId)
        {
            try
            {
                using var connection = new SqlConnection(_connectionString);
                connection.Open();

                string query = @"INSERT INTO UserAuditLog (IdUser, ActionType, ActionDate, IPAddress)
                                VALUES (@UserId, 'LOGOUT', GETDATE(), @IPAddress)";

                using var command = new SqlCommand(query, connection);
                command.Parameters.AddWithValue("@UserId", userId);
                command.Parameters.AddWithValue("@IPAddress", "0.0.0.0");

                command.ExecuteNonQuery();
            }
            catch
            {
                
            }

            
        }

        private string GenerateJwtToken(UserInfo user)
        {
            var key = Encoding.UTF8.GetBytes(_configuration["Jwt:Key"] ?? "YourSuperSecretKeyAtLeast32CharactersLong!");

            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.NameIdentifier, user.IdUser.ToString()),
                new Claim(ClaimTypes.Name, user.UserName),
                new Claim(ClaimTypes.Email, user.Email),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
            };

            if (!string.IsNullOrEmpty(user.RoleName))
            {
                claims.Add(new Claim(ClaimTypes.Role, user.RoleName));
            }

            foreach (var permission in user.AllPermissions)
            {
                claims.Add(new Claim("Permission", permission));
            }

            claims.Add(new Claim("UserName", user.UserName));
            claims.Add(new Claim("RoleId", user.RoleId?.ToString() ?? "0"));

            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(claims),
                Expires = DateTime.UtcNow.AddMinutes(30), 
                Issuer = _configuration["Jwt:Issuer"] ?? "YourApp",
                Audience = _configuration["Jwt:Audience"] ?? "YourAppUsers",
                SigningCredentials = new SigningCredentials(
                    new SymmetricSecurityKey(key),
                    SecurityAlgorithms.HmacSha256)
            };

            var tokenHandler = new JwtSecurityTokenHandler();
            var token = tokenHandler.CreateToken(tokenDescriptor);
            return tokenHandler.WriteToken(token);
        }

        private string HashPassword(string password)
        {
            using var sha256 = System.Security.Cryptography.SHA256.Create();
            var hashedBytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(password));
            return Convert.ToBase64String(hashedBytes);
        }
    }
}
