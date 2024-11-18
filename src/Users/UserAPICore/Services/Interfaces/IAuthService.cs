using UserAPI.Business.Models;

namespace UserAPI.Business.Services.Interfaces
{
    public interface IAuthService
    {
        string Login(User user);
    }
}