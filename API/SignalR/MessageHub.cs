using API.DTOS;
using API.Entities;
using API.Errors;
using API.Extensions;
using API.Interfaces;
using API.Services;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;

namespace API.SignalR
{
    [Authorize]
    public class MessageHub : Hub
    {
        private readonly IUnitOfWork _uow;

        private readonly IMapper _mapper;
        private readonly PresenceTracker _tracker;
        private readonly IHubContext<PresenceHub> _presence;
        public MessageHub(IMapper mapper,PresenceTracker tracker, IHubContext<PresenceHub> presence, IUnitOfWork uow)
        {
            _mapper = mapper;
            _tracker = tracker;
            _presence = presence;
            _uow = uow;
        }

        public override async Task OnConnectedAsync()
        {
            var httpContext = Context.GetHttpContext();
            var caller = Context.User.GetUserName();
            var otherUser = httpContext.Request.Query["user"].ToString();
            var groupName = CreateGroupName(caller, otherUser);

            await AddToGroupWithConnections(Context, groupName);

            await Groups.AddToGroupAsync(Context.ConnectionId, groupName);
            var mesages = await _uow.MessageRepository.GetMessgeThread(caller, otherUser);
            if (_uow.HasChanges()) await _uow.CompleteAsync();
            await Clients.Group(groupName).SendAsync("RecieveMessageThread", mesages);


        }

        public override async Task OnDisconnectedAsync(Exception exception)
        {
            await RemoveFromMessageGroup(Context.ConnectionId);
            await base.OnDisconnectedAsync(exception);
        }

        public async Task sendMessage(CreateMessageDto createMessage)
        {
            var currentUser = Context.User.GetUserName();
            if (currentUser == createMessage.RecipientUserName) throw new HubException("You cannot send message to yourself");
            var sender = await _uow.UserRepository.GetUserByUserName(currentUser);
            var reciever = await _uow.UserRepository.GetUserByUserName(createMessage.RecipientUserName);
            if (reciever == null) throw new HubException("Reciever User  Not Found");
            if (sender == null) throw new HubException("Sender User  Not Found");

            var message = new Message
            {
                SenderId = sender.Id,
                RecieverId = reciever.Id,
                RecieverUserName = reciever.UserName,
                SenderUserName = sender.UserName,
                Content = createMessage.Content,
            };
            var groupName = CreateGroupName(currentUser, reciever.UserName);
            var group = await _uow.MessageRepository.GetMessageGroup(groupName);
            if (group.Connections.Any(x => x.UserName == reciever.UserName))
            {
                message.DateRead = DateTime.Now;
                message.IsRead = true;
            }
            else
            {
                var conections=await _tracker.GetUserConnections(reciever.UserName);
                if (conections != null)
                {
                  await _presence.Clients.Clients(conections).SendAsync("newUnreadMessage", new
                    {
                        username = sender.UserName,
                        content = createMessage.Content,
                    });
                }
            }
            await _uow.MessageRepository.AddMessage(message);
            if (await _uow.CompleteAsync())
            {
                await Clients.Group(groupName).SendAsync("NewMessage", _mapper.Map<MessageDto>(message));

            }
        }


        private static string CreateGroupName(string caller, string other)
        {
            var compare = string.Compare(caller, other) < 0;
            return compare ? $"{caller}--{other}" : $"{other}--{caller}";
        }

        private async Task<bool> AddToGroupWithConnections(HubCallerContext context, string groupName)
        {
            var group = await _uow.MessageRepository.GetMessageGroup(groupName);
            var connection = new Connection(context.ConnectionId, context.User.GetUserName());
            if (group == null)
            {
                group = new Group(groupName);
                _uow.MessageRepository.AddGroup(group);
            }
            group.Connections.Add(connection);
            return await _uow.CompleteAsync();
        }

        private async Task RemoveFromMessageGroup(string connectionId)
        {
            var connection = await _uow.MessageRepository.GetConnection(connectionId);
            _uow.MessageRepository.RemoveConnection(connection);
            await _uow.CompleteAsync();
        }
    }
}
