using UserAPI.Business.Models;

namespace UserAPI.Business.Repository.Interfaces
{
    public interface IUserRepository
    {
        Task<User> Add(User user);
        Task<IEnumerable<User>> GetAll();
        Task<User?> GetById(int id);
        Task<User> Update(User user);
        Task<bool> Delete(int id);
        Task<User?> GetByEmail(string? email);
    }
}