using Microsoft.EntityFrameworkCore;
using UserAPI.Business.Data;
using UserAPI.Business.Models;
using UserAPI.Business.Repository.Interfaces;

namespace UserAPI.Business.Repository
{
    public class UserRepository : IUserRepository
    {
        private readonly UserAPIDbContext _context;

        public UserRepository(UserAPIDbContext context)
        {
            _context = context;
        }

        public async Task<User> Add(User user)
        {
            await _context.Users.AddAsync(user);
            await _context.SaveChangesAsync();
            return user;
        }

        public async Task<bool> Delete(int id)
        {
            User user = await _context.Users.FindAsync(id) ?? new User();; 
            user.IsActive = false;
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<IEnumerable<User>> GetAll()
        {
            return await _context.Users.Where(u => u.IsActive == true).ToListAsync();
        }

        public async Task<User?> GetById(int id)
        {
            User? user = await _context.Users.FirstOrDefaultAsync(s => s.UserId == id && s.IsActive == true);
            return user;
        }

        public async Task<User> Update(User user)
        {
            _context.Update(user);
            await _context.SaveChangesAsync();
            return user;
        }

        public async Task<User?> GetByEmail(string? email)
        {
            User? user = await _context.Users.FirstOrDefaultAsync(s => s.IsActive == true && s.Email == email);
            return user;
        }
    }
}