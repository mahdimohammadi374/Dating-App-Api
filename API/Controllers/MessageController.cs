using API.DTOS;
using API.Entities;
using API.Errors;
using API.Extensions;
using API.Helpers;
using API.Interfaces;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class MessageController : ControllerBase
    {
        private readonly IUnitOfWork _uow;
        private readonly IMapper _mapper;
        public MessageController( IMapper mapper, IUnitOfWork uow)
        {
            _mapper = mapper;
            _uow = uow;
        }

        [HttpPost("create-message")]
        public async Task<ActionResult<MessageDto>> createMessage(CreateMessageDto createMessage)
        {
            var currentUser=User.GetUserName();
            if (currentUser == createMessage.RecipientUserName) return BadRequest(new ApiResponse(400, "You can NOT send message to yourself"));
            var sender=await _uow.UserRepository.GetUserByUserName(currentUser);
            var reciever=await _uow.UserRepository.GetUserByUserName(createMessage.RecipientUserName);
            if (reciever == null) return NotFound(new ApiResponse(404, "Reciever User  Not Found"));
            if (sender == null) return NotFound(new ApiResponse(404, "Sender User  Not Found"));

            var message = new Message
            {
                SenderId = sender.Id,
                RecieverId = reciever.Id,
                RecieverUserName = reciever.UserName,
                SenderUserName = sender.UserName,
                Content = createMessage.Content,
            };
            await _uow.MessageRepository.AddMessage(message);
            if (await _uow.CompleteAsync())
            {
                return Ok(_mapper.Map<Message, MessageDto>(message));

            }
            return BadRequest(new ApiResponse(400, "An error occurred"));
        }

        [HttpGet("get-messages")]
        public async Task<ActionResult<PagedList<MessageDto>>> GetMessages([FromQuery] MessageParams messageParams)
            {
            var currentUser=User.GetUserName();
            messageParams.UserName = currentUser;
            return Ok(await _uow.MessageRepository.GetMessgeForUser(messageParams));

        }

        [HttpGet("get-messages-thread/{recipientName}")]
        public async Task<ActionResult<IEnumerable<MessageDto>>> GetMessagesThread( string recipientName)
        {
            var currentUser=User.GetUserName();
            return Ok(await _uow.MessageRepository.GetMessgeThread(currentUser, recipientName));
        }
    }
}
