using API.Data;
using API.Interfaces;
using AutoMapper;

namespace API.Services
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly DataContext _context;
        private readonly IMapper _mapper;
        public UnitOfWork(DataContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        public IMessageRepository MessageRepository
        {
            get { return new MessageRepository(_context, _mapper); }
        }

        public IUserLikeRepository UserLikeRepository => new UserLikeRepository(_context, _mapper);

        public IUserRepository UserRepository => new UserRepository(_context, _mapper);

        public async Task<bool> CompleteAsync()
        {
            return await _context.SaveChangesAsync() > 0;
        }

        public bool HasChanges()
        {
            return _context.ChangeTracker.HasChanges();
        }
    }
}
