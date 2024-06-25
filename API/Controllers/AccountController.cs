using API.Data;
using API.DTOS;
using API.Entities;
using API.Errors;
using API.Interfaces;
using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Cryptography;
using System.Text;

namespace API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AccountController : ControllerBase
    {
        private readonly IJWTService _jwtService;
        private readonly IMapper _mapper;
        private readonly SignInManager<User> _signInManager;
        private readonly UserManager<User> _userManager;
        public AccountController(IJWTService jwtService, IMapper mapper, SignInManager<User> signInManager, UserManager<User> userManager)
        {
            _jwtService = jwtService;
            _mapper = mapper;
            _signInManager = signInManager;
            _userManager = userManager;
        }
        /// <summary>
        /// Register a new user
        /// </summary>
        [HttpPost("register")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<UserTokenDto>> Register(registerDto model)
        {
            if (await IsExistUserName(model.username))
            {
                return BadRequest(new ApiResponse(400, $"{model.username} exist already"));
            }
            using var hmac = new HMACSHA512();
            var user = _mapper.Map<User>(model);
            var result = await _userManager.CreateAsync(user, model.password);

            var roleResult = await _userManager.AddToRoleAsync(user, "member");
            if (!roleResult.Succeeded)
                return BadRequest(new ApiResponse(400, "An error occurred while registering data"));

            if (result.Succeeded)
            {
                return Ok(new UserTokenDto()
                {
                    username = user.UserName,
                    token = await _jwtService.GenerateJWT(user),
                    Gender= user.Gender,
                    KnownAs=user.KnownAs,
                });
            }
            return BadRequest(new ApiResponse(400, "An error occurred while registering data"));


        }
        /// <summary>
        /// login user
        /// </summary>
        [HttpPost("login")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status400BadRequest)]

        public async Task<ActionResult<UserTokenDto>> Login(loginDto model)
        {
            User user = await _userManager.Users.Include(x => x.Photos).FirstOrDefaultAsync(x => x.UserName == model.username);
            if (user == null)
            {
                return BadRequest(new ApiResponse(400, "Incorrect Username or password"));
            }

            var result = await _signInManager.CheckPasswordSignInAsync(user, model.password, false);
            if (!result.Succeeded)
            {
                return BadRequest(new ApiResponse(400, "Incorrect Username or password"));

            }
            try
            {
                UserTokenDto fUser = new UserTokenDto()
                {
                    username = user.UserName,
                    token = await _jwtService.GenerateJWT(user),
                    photoUrl = user.Photos.Count > 0 ? user.Photos.FirstOrDefault(p => p.IsMain).Url : "",
                    Gender = user.Gender,
                    KnownAs = user.KnownAs,
                };
                return Ok(fUser);
            }
            catch (Exception ex)
            {
                return BadRequest(new ApiResponse(400, ex.Message));
            }

        }

        [HttpGet("IsExistUserName/{username}")]
        public async Task<bool> IsExistUserName(string username)
        {
            return await _userManager.Users.AnyAsync(x => x.UserName == username.ToLower());
        }
    }
}
