using System.IO;

namespace FeatureFlagsEngineAPI.Models
{
    public class UserInfo
    {
        public int IdUser { get; set; }
        public string UserName { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public int? RoleId { get; set; }
        public string? RoleName { get; set; }
        public List<RoleInfo> AllRoles { get; set; } = new();
        public List<PermissionInfo> DirectPermissions { get; set; } = new();
        public List<PermissionInfo> RolePermissions { get; set; } = new();
        public List<string> AllPermissions { get; set; } = new();
    }
}
