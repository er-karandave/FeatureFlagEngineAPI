using FeatureFlagsEngineAPI.Models;

namespace FeatureFlagsEngineAPI.Interfaces.ControllerInterfaces
{
    public interface IUserService
    {
        string Login(User user);
        void LogOut(string username, string password);
    }
}
