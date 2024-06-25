using API.Data;
using API.Entities;
using API.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace API.Services
{
    public class AccountRepository : IAccountRepository
    {
        private readonly DataContext _context;

        public AccountRepository(DataContext context)
        {
            _context = context;
        }

        
        public async Task AddUser(User user)
        {
            await _context.Users.AddAsync(user);
        }

        public async Task<User> GetUserByUsername(string username)
        {
            return await _context.Users.SingleOrDefaultAsync(u=>u.UserName.ToLower()==username.ToLower());
        }

        public async Task<User> GetUsreByUsernameWithPhotos(string username)
        {
            return await _context.Users.Include(u=>u.Photos).SingleOrDefaultAsync(u => u.UserName.ToLower() == username.ToLower());
        }

        public async Task<bool> IsExistUser(string username)
        {
            return await _context.Users.AnyAsync(u=>u.UserName.ToLower() == username.ToLower());
        }

        public async Task<bool> SaveChanges()
        {
            return await _context.SaveChangesAsync() > 0;
        }
    }
}
