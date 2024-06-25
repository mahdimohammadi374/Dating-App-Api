 using API.Data;
using API.DTOS;
using API.Entities;
using API.Errors;
using API.Extensions;
using API.Helpers;
using API.Interfaces;
using API.Middlewares;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [ServiceFilter(typeof(logUserActivity))]

    public class UserController : ControllerBase
    {
        private readonly IUnitOfWork _uow;
        private readonly IMapper _mapper;
        private readonly IPhotoService _photoService;

        public UserController(
            IMapper mapper,
            IPhotoService photoService
,
            IUnitOfWork uow)
        {
            _mapper = mapper;
            _photoService = photoService;
            _uow = uow;
        }



        [HttpGet("get-all-users")]
        [Authorize]
        public async Task<ActionResult<PagedList<MemberDto>>> GetUsers([FromQuery]UserParams userParams)
        {
            userParams.currentUser = User.GetUserName();
           var users = await _uow.UserRepository.GetAllMemberDtos(userParams);
            //Response.AddPaginationHeader(users.CurrentPage, users.PageSize, users.TotalCount, users.TotalPage);
            return Ok(users);
        }




        [HttpGet("get-user-by-id/{id}")]
        [Authorize]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<MemberDto>> GetUser(int id)
        {
            MemberDto user = await _uow.UserRepository.GetMemberDtoById(id);
            if (user == null)
            {
                return BadRequest(new ApiResponse(400, "User  not found"));
            }

            return Ok(user);
        }




        [HttpGet("get-user-by-username/{userName}" , Name ="GetUser")]
        [Authorize]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<MemberDto>> GetUser(string userName)
        {
            MemberDto user = await _uow.UserRepository.GetMemberDtoByUserName(userName);
            if (user == null)
            {
                return BadRequest(new ApiResponse(400, "User  not found"));
            }

            return Ok(user);
        }


        [HttpPut("update-user")]
        [Authorize]
        public async Task<ActionResult<MemberDto>> UpdateUser(UpdateUserDto model)
        {
            //var username = HttpContext.User.FindFirst("nameid").Value;
            //var username = User.FindFirst(ClaimTypes.NameIdentifier).Value;
            var username = User.GetUserName();
            var member=await _uow.UserRepository.GetUserByUsreNameWithPhotos(username);
            if(member == null) { return NotFound(new ApiResponse(404, "Your not found")); };
             member = _mapper.Map(model, member);
            _uow.UserRepository.Update(member);
            if (await _uow.CompleteAsync())
            {
                return Ok(_mapper.Map<MemberDto>(member));
            }
            return BadRequest(new ApiResponse(400));


        }


        [HttpPost("add-photo")]
        [Authorize]
        public async Task<IActionResult> AddPhoto(IFormFile file) 
        {
            var result =await _photoService.AddPhotoAsync(file);
            if (result.Error != null)
            {
                return BadRequest(new ApiResponse(400 , "An Error occurred while uploading image. please try again."));
            }
             
            var user=await _uow.UserRepository.GetUserByUsreNameWithPhotos(User.GetUserName());
            if (user == null) { return BadRequest(new ApiResponse(400)); };
            Photo photo = new Photo
            {
                Url = result.SecureUri.AbsoluteUri,
                PublicId = result.PublicId,
                UserId = user.Id,
                IsMain = user.Photos.Count() == 0 ? true : false,


            };
            user.Photos.Add(photo);
            _uow.UserRepository.Update(user);
            if (await _uow.CompleteAsync())
            {
                return CreatedAtRoute("GetUser", new
                {
                    userName = user.UserName
                },_mapper.Map<PhotoDto>(photo)) ;
            }
            return BadRequest(new ApiResponse(400, "An Error occurred while uploading image. please try again."));
        }


        [HttpPut("set-main-photo/{photoId}")]
        [Authorize]
        public async Task<ActionResult<PhotoDto>> setMainPhoto(int photoId) 
        {
            var user = await _uow.UserRepository.GetUserByUsreNameWithPhotos(User.GetUserName());
            if (user == null)
            {
                return NotFound(new ApiResponse(404, "Not found"));
            }
            var photo = user.Photos.FirstOrDefault(x => x.Id == photoId);
            if (photo == null)
            {
                return NotFound(new ApiResponse(404, "Image Not found"));
            }

            if (photo.IsMain)
            {
                return BadRequest(new ApiResponse(400, "It is main photo alreadey"));
            }
            var mainPhoto = user.Photos.FirstOrDefault(x => x.IsMain == true);
            mainPhoto.IsMain = false;
            photo.IsMain= true;
            _uow.UserRepository.Update(user);
            if (await _uow.CompleteAsync())
            {
                return Ok(_mapper.Map<PhotoDto>(photo));
            }
            return BadRequest(new ApiResponse(400, "Not found"));

        }


        [HttpDelete("delete-photo/{photoId}")]
        public async Task<ActionResult<PhotoDto>> deletePhoto(int photoId)
        {
            var user = await _uow.UserRepository.GetUserByUsreNameWithPhotos(User.GetUserName());
            if(user == null) { return NotFound(new ApiResponse(404)); };
            var photo=user.Photos.FirstOrDefault(p=>p.Id==photoId );
            if (photo == null) { return NotFound(new ApiResponse(404)); };
            if(photo.IsMain) { return BadRequest(new ApiResponse(400, "You can not delete main phot")); };
            await _photoService.DeletePhotoAsync(photo.PublicId);
            user.Photos.Remove(photo);
            _uow.UserRepository.Update(user);
            if(await _uow.CompleteAsync())
            {
                return Ok(_mapper.Map<PhotoDto>(photo));
            };
            return BadRequest(new ApiResponse(400, "An error occurred whilr deleting photo. Please try again"));
        }


        [HttpGet("is-exist-username/{username}")]
        public async Task<ActionResult<bool>> isExistUsername(string username)
        {
            return await _uow.UserRepository.IsExistUserName(username);
        }


    }
}
