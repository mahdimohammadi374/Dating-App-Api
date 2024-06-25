using API.DTOS;
using API.Entities;
using API.Helpers;

namespace API.Interfaces
{
    public interface IMessageRepository
    {
        Task AddMessage(Message message);
        Task<Message> GetMessageById(int id);
        void DeleteMessage(Message message);
        Task<PagedList<MessageDto>> GetMessgeForUser(MessageParams messageParams);
        Task<IEnumerable<MessageDto>> GetMessgeThread(string currsentUserName, string recipientName);
        Task UpdateMessageToRead(List<MessageDto> messages, string userName);

        // SignalR
        void AddGroup(Group group);
        void RemoveConnection(Connection connection);
        Task<Connection> GetConnection(string connectionId);
        Task<Group> GetMessageGroup(string groupName);

    }
}
