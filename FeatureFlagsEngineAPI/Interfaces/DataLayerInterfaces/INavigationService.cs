using FeatureFlagsEngineAPI.Models;

namespace FeatureFlagsEngineAPI.Interfaces.DataLayerInterfaces
{
    public interface INavigationService
    {
        Task<List<NavItem>> GetNavigationItemsAsync(int userId, List<string> userPermissions);
}
}
