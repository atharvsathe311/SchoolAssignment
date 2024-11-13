using UserAPI.Business.Models;
using UserAPI.Core.GeneralModels;

namespace UserAPI.Business.Services.Interfaces
{
    public interface IAuthService
    {
        string Login(LoginRequest loginRequest, User user);
    }
}