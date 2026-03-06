using FeatureFlagsEngineAPI.Models;

namespace FeatureFlagsEngineAPI.Interfaces.DataLayerInterfaces
{
    public interface IAuthService
    {
            AuthResponse Login(string email, string password);
            void Logout(int userId);
        
    }
}
