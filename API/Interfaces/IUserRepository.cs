using API.DTOS;
using API.Entities;
using API.Helpers;

namespace API.Interfaces
{
    public interface IUserRepository
    {
        void Update(User user);

        Task<PagedList<MemberDto>> GetAllMemberDtos(UserParams userParams);
        Task<MemberDto> GetMemberDtoById(int id);
        Task<MemberDto> GetMemberDtoByUserName(string userName);
        Task<bool> IsExistUserName(string userName);
        Task<IEnumerable<User>> GetAllUsers();
        Task<User> GetUserById(int id);
        Task<User> GetUserByUserName(string userName);
        Task<User> GetUserByUsreNameWithPhotos(string userName);
    }
}
