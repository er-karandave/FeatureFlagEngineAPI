using FeatureFlagsEngineAPI.Interfaces.DataLayerInterfaces;
using FeatureFlagsEngineAPI.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity.Data;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace FeatureFlagsEngineAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly IAuthService _authService;

        public AuthController(IAuthService authService)
        {
            _authService = authService;
        }

        [HttpPost("login")]
        public IActionResult Login([FromBody] Models.LoginRequest request)
        {
            if (string.IsNullOrWhiteSpace(request.Email) || string.IsNullOrWhiteSpace(request.Password))
            {
                return BadRequest(new AuthResponse
                {
                    Success = false,
                    Message = "Email and password are required."
                });
            }

            var response = _authService.Login(request.Email, request.Password);

            if (response.Success)
            {
                return Ok(response);
            }
            else
            {
                return Unauthorized(response);
            }
        }

        [HttpPost("logout")]
        public IActionResult Logout()
        {
            try
            {
                // ✅ Get user ID from token claims
                var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

                if (!string.IsNullOrEmpty(userIdClaim) && int.TryParse(userIdClaim, out int userId))
                {
                    _authService.Logout(userId);
                }

                return Ok(new { Success = true, Message = "Logged out successfully." });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { Success = false, Message = "Logout failed." });
            }
        }
    }
}
