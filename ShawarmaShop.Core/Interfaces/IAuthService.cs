using Authentication;

namespace ShawarmaShopCore.Interfaces
{
    public interface IAuthService
    {
        bool Login(string username, string password);
        void Register(User user, string password);
        bool UserExists(string username);
    }
}
