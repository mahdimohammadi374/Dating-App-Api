using API.DTOS;
using API.Entities;
using API.Enums;
using API.Helpers;

namespace API.Interfaces
{
    public interface IUserLikeRepository
    {
        Task<UserLike> GetUserLike(int sourceId, int targetId);
        Task<User> GetUserWithLikes(int sourceId);
        Task<PagedList<MemberDto>> GetUserLikes(GetLikeParams getLikeParams, int userId);
        Task AddLike(int sourceId, int targetId);
    }
}
