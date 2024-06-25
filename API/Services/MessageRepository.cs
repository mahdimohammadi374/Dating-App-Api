using API.Data;
using API.DTOS;
using API.Helpers;
using API.Interfaces;
using API.Extensions;
using Microsoft.EntityFrameworkCore;
using API.Entities;
using AutoMapper;
using AutoMapper.QueryableExtensions;


namespace API.Services
{
    public class MessageRepository : IMessageRepository
    {
        private readonly DataContext _context;
        private readonly IMapper _mapper;

        public MessageRepository(DataContext dataContext, IMapper mapper)
        {
            _context = dataContext;
            _mapper = mapper;
        }

        public async Task AddMessage(Message message)
        {
            await _context.Messages.AddAsync(message);
        }

        public async Task<Message> GetMessageById(int id)
        {
            return await _context.Messages.FindAsync(id);
        }

        public async Task<PagedList<MessageDto>> GetMessgeForUser(MessageParams messageParams)
        {
            var query = _context.Messages.OrderByDescending(x => x.MessageSent).AsQueryable();
            if (messageParams.Container == "Unread")
            {
                query = query.Where(m => m.RecieverUserName == messageParams.UserName && !m.IsRead && !m.RecierverDeleted);
            }
            if (messageParams.Container == "Inbox")
            {
                query = query.Where(m => m.RecieverUserName == messageParams.UserName && !m.RecierverDeleted);
            }
            if (messageParams.Container == "Outbox")
            {
                query = query.Where(m => m.SenderUserName == messageParams.UserName && !m.SenderDeleted);
            }
            var message = query.ProjectTo<MessageDto>(_mapper.ConfigurationProvider);
            return await PagedList<MessageDto>.CreateAsync(message, messageParams.PageNumber, messageParams.PageSize);
        }

        public async Task<IEnumerable<MessageDto>> GetMessgeThread(string currsentUserName, string recipientName)
        {
            var query=await _context.Messages.Where(
                m=>m.SenderUserName==currsentUserName && m.RecieverUserName==recipientName ||
                m.SenderUserName==recipientName && m.RecieverUserName==currsentUserName 
                )
                .ProjectTo<MessageDto>(_mapper.ConfigurationProvider)
                .OrderBy(m=>m.MessageSent)
                .ToListAsync();

            await UpdateMessageToRead(query, currsentUserName);
            return query;
        }

     
        public async Task UpdateMessageToRead(List<MessageDto> messages , string userName)
        {
            messages = messages.Where(m=>!m.DateRead.HasValue && m.RecieverUserName==userName).ToList();
            if (messages.Any())
            {

            foreach (var msg in messages)
            {
                msg.DateRead = DateTime.Now;
                msg.IsRead=true;
            }
            _context.UpdateRange(_mapper.Map<List<Message>>(messages));
            await _context.SaveChangesAsync();
            }
        }

        public void DeleteMessage(Message message)
        {
        }

        public  void AddGroup(Group group)
        {
            _context.Groups.Add(group);
        }

        public void RemoveConnection(Connection connection)
        {
            _context.Connections.Remove(connection);
        }

        public async Task<Connection> GetConnection(string connectionId)
        {
           return await _context.Connections.FirstOrDefaultAsync(c=>c.ConnectionId==connectionId);
        }

        public async Task<Group> GetMessageGroup(string groupName)
        {
            return await _context.Groups.Include(g=>g.Connections).FirstOrDefaultAsync(x=>x.Name==groupName);
        }
    }
}
