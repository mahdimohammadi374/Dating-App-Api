using API.DTOS;
using API.Enums;
using API.Errors;
using API.Extensions;
using API.Helpers;
using API.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class UserLikeController : ControllerBase
    {
        private readonly IUnitOfWork _uow;

        public UserLikeController(IUnitOfWork uow)
        {
            _uow = uow;
        }

        [HttpPost("add-like")]
        public async Task<IActionResult> AddLike([FromQuery] string targetUserName)
        {
            var userId = User.GetUserId();
            var targetUser = await _uow.UserRepository.GetUserByUserName(targetUserName);
            if (targetUser == null) { return NotFound(new ApiResponse(404, "User not found")); };
            if (targetUser.Id == userId) return BadRequest(new ApiResponse(400, "You can not like yourself"));

            var userLike = await _uow.UserLikeRepository.GetUserLike(userId, targetUser.Id);
            if (userLike != null) { return BadRequest(new ApiResponse(400, "You already liked this user")); };

            await _uow.UserLikeRepository.AddLike(userId, targetUser.Id);
            if (await _uow.CompleteAsync()) return Ok();
            return BadRequest(new ApiResponse(400, "An error occurred please try again"));
        }

        [HttpGet("get-likes")]
        public async Task<ActionResult<PagedList<MemberDto>>> GetLikes([FromQuery] GetLikeParams getLikeParams)
        {
            return Ok(await _uow.UserLikeRepository.GetUserLikes(getLikeParams, User.GetUserId()));
        }

    }
}
