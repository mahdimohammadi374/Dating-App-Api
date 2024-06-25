using API.Data;
using API.DTOS;
using API.Entities;
using API.Helpers;
using API.Interfaces;
using AutoMapper;
using AutoMapper.QueryableExtensions;
using Microsoft.EntityFrameworkCore;

namespace API.Services
{
    public class UserRepository : IUserRepository
    {
        private readonly DataContext _context;
        private readonly IMapper _mapper;

        public UserRepository(DataContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        public async Task<PagedList<MemberDto>> GetAllMemberDtos(UserParams userParams)
        {
            var query= _context.Users.AsNoTracking();
            var minDate = DateTime.Today.AddYears(-userParams.maxAge - 1);
            var maxDate = DateTime.Today.AddYears(-userParams.minAge);
            query = query.
                Where(
                x => x.UserName != userParams.currentUser &&
                x.Gender==userParams.Gender &&
                x.DateOfBirth.Date >= minDate.Date &&
                x.DateOfBirth.Date <= maxDate.Date);

            if (userParams.TypeSort == TypeSort.dec)
            {

            query = userParams.OrderBy switch
            {
                OrderBy.lastActive => query.OrderByDescending(x => x.LastActive),
                OrderBy.age=>query.OrderByDescending(x=>x.DateOfBirth),
                OrderBy.created=>query.OrderByDescending(x=>x.Created),
                _=> query.OrderByDescending(x => x.LastActive)
            } ;
            }
            else
            {
                query = userParams.OrderBy switch
                {
                    OrderBy.lastActive => query.OrderBy(x => x.LastActive),
                    OrderBy.age => query.OrderBy(x => x.DateOfBirth),
                    OrderBy.created => query.OrderBy(x => x.Created),
                    _ => query.OrderBy(x => x.LastActive)
                };
            }

            var result = query.ProjectTo<MemberDto>(_mapper.ConfigurationProvider);
           var final= await PagedList<MemberDto>.CreateAsync(result, userParams.PageNumber , userParams.PageSize);
            return final;
        }

        public async Task<IEnumerable<User>> GetAllUsers()
        {
            return await _context.Users.ToListAsync();
        }

        public async Task<MemberDto> GetMemberDtoById(int id)
        {
            return await _context.Users
                .ProjectTo<MemberDto>(_mapper.ConfigurationProvider).SingleOrDefaultAsync(x => x.Id == id);
        }


        public async Task<MemberDto> GetMemberDtoByUserName(string userName)
        {
            return await _context.Users.ProjectTo<MemberDto>(_mapper.ConfigurationProvider)
                .SingleOrDefaultAsync(x=>x.UserName.ToLower()==userName.ToLower());
        }

        public async Task<User> GetUserById(int id)
        {
            return await _context.Users.FindAsync(id);
        }

        public async Task<User> GetUserByUserName(string userName)
        {
            return await _context.Users.SingleOrDefaultAsync(u => u.UserName == userName);
        }

        public async Task<User> GetUserByUsreNameWithPhotos(string userName)
        {
            return await _context.Users.Include(u=>u.Photos).SingleOrDefaultAsync(u => u.UserName == userName);
        }

        public async Task<bool> IsExistUserName(string userName)
        {
            return await _context.Users.AnyAsync(u => u.UserName.ToLower() == userName.ToLower());
        }

       

        public void Update(User user)
        {
            _context.Entry(user).State = EntityState.Modified;
        }
    }
}
