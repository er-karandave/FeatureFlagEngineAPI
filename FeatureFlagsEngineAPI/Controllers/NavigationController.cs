using FeatureFlagsEngineAPI.Interfaces.DataLayerInterfaces;
using FeatureFlagsEngineAPI.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace FeatureFlagsEngineAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class NavigationController : ControllerBase
    {
        private readonly INavigationService _navigationService;

        public NavigationController(INavigationService navigationService)
        {
            _navigationService = navigationService;
        }

        [HttpGet("menu")]
        public async Task<IActionResult> GetNavigationMenu()  // ✅ Async controller method
        {
            try
            {
                var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

                if (string.IsNullOrEmpty(userIdClaim) || !int.TryParse(userIdClaim, out int userId))
                {
                    return Unauthorized(new NavigationResponse
                    {
                        Success = false,
                        Message = "User not authenticated.",
                        Data = new List<NavItem>()
                    });
                }

                var permissions = User.FindAll("Permission")
                    .Select(c => c.Value)
                    .ToList();

                var roles = User.FindAll(ClaimTypes.Role)
                    .Select(c => c.Value)
                    .ToList();

                if (roles.Any(r => r.Contains("Admin", StringComparison.OrdinalIgnoreCase) || r.Contains("Super", StringComparison.OrdinalIgnoreCase)))
                {
                    permissions.Add("ADMIN_ACCESS");
                }

                var navItems = await _navigationService.GetNavigationItemsAsync(userId, permissions);

                return Ok(new NavigationResponse
                {
                    Success = true,
                    Message = "Navigation menu retrieved successfully.",
                    Data = navItems
                });
            }
            catch (Exception ex)
            {
                Console.Error.WriteLine($"Navigation API error: {ex.Message}");
                return StatusCode(500, new NavigationResponse
                {
                    Success = false,
                    Message = "Error loading navigation menu.",
                    Data = new List<NavItem>()
                });
            }
        }
    }
}

