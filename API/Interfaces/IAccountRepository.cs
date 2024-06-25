using API.Entities;

namespace API.Interfaces
{
    public interface IAccountRepository
    {
        Task<bool> IsExistUser(string username);
        Task AddUser(User user);
        Task<User> GetUserByUsername(string username);
        Task<User> GetUsreByUsernameWithPhotos(string username);
        Task<bool> SaveChanges();
    }
}
