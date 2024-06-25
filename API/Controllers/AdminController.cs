using API.DTOS;
using API.Entities;
using API.Errors;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AdminController : ControllerBase
    {
        private readonly UserManager<User> _userManager;

        public AdminController(UserManager<User> userManager)
        {
            _userManager = userManager;
        }

        [HttpGet("get-users-with-username")]
        public async Task<IActionResult> GetUsersWithRoles()
        {
            var users = await _userManager.Users.Include(x => x.UserRoles).ThenInclude(c => c.Role).OrderBy(o => o.UserName).Select(
                v => new UserTokenDto
                {
                    Id = v.Id,
                    username = v.UserName,
                    Gender = v.Gender,
                    KnownAs = v.KnownAs,
                    Roles = v.UserRoles.Select(r => r.Role.Name).ToList(),
                }
                ).ToListAsync();
            return Ok(users);
        }

        [HttpPost("edit-roles/{userName}")]
        public async Task<IActionResult> EditUserRoles(string userName, [FromQuery] string roles)
        {
            var selectedRoles = roles.Split(',');
            var user = await _userManager.FindByNameAsync(userName);
            if (user == null)
            {
                return BadRequest(new ApiResponse(400, "Incorrect Username or password"));

            }
            var userRoles = await _userManager.GetRolesAsync(user);
            var del = await _userManager.RemoveFromRolesAsync(user, userRoles);
            if (!del.Succeeded)
                return BadRequest(new ApiResponse(400, "An error occurred while deleteing roles"));

            var addRole = await _userManager.AddToRolesAsync(user, selectedRoles);
            if (!addRole.Succeeded)
                return BadRequest(new ApiResponse(400, "An error occurred while adding roles"));
            return Ok(selectedRoles);
        }
    }
}
