using API.Data;
using API.DTOS;
using API.Entities;
using API.Enums;
using API.Helpers;
using API.Interfaces;
using AutoMapper;
using AutoMapper.QueryableExtensions;
using Microsoft.EntityFrameworkCore;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory;

namespace API.Services
{
    public class UserLikeRepository : IUserLikeRepository
    {
        private readonly DataContext _context;
        private readonly IMapper _mapper;

        public UserLikeRepository(DataContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        public async Task AddLike(int sourceId, int targetId)
        {
            await _context.UsersLikes.AddAsync(new UserLike
            {
                SourceUserId = sourceId,
                TargetUserId = targetId
            });


        }

        public async Task<UserLike> GetUserLike(int sourceId, int targetId)
        {
            return await _context.UsersLikes.FirstOrDefaultAsync(x=>x.SourceUserId==sourceId && x.TargetUserId==targetId);
        }

        public async Task<PagedList<MemberDto>> GetUserLikes(GetLikeParams getLikeParams, int userId)
        {
            var users = _context.Users.AsQueryable();
            var likes = _context.UsersLikes.AsQueryable();


            if(getLikeParams.PredicateUserLike == PredicateLikeEnum.Liked) 
            {
                likes=likes.Include(x=>x.TargetUser)
                    .ThenInclude(x=>x.Photos).
                    Where(x=>x.SourceUserId==userId);
                users = likes.Select(x => x.TargetUser);
            }
            if(getLikeParams.PredicateUserLike == PredicateLikeEnum.LikeBy) 
            {
                likes=likes.Include(x=>x.SourceUser)
                    .ThenInclude(v=>v.Photos)
                    .Where(b=>b.TargetUserId==userId);
                users=likes.Select(x => x.SourceUser);
            }
            var result = users.ProjectTo<MemberDto>(_mapper.ConfigurationProvider);
            var final = await PagedList<MemberDto>.CreateAsync(result, getLikeParams.PageNumber, getLikeParams.PageSize);
            return final;
        }

        public async Task<User> GetUserWithLikes(int sourceId)
        {
            return await _context.Users.Include(x => x.TargetUserLikes).FirstOrDefaultAsync(x => x.Id == sourceId);
        }

       
    }
}
