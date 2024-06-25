using API.Entities;

namespace API.Interfaces
{
    public interface IJWTService
    {
        Task<string> GenerateJWT(User user);
    }
}
