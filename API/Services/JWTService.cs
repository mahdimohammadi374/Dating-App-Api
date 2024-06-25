using API.Entities;
using API.Interfaces;
using Microsoft.AspNetCore.Identity;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace API.Services
{
    public class JWTService : IJWTService
    {
        private readonly UserManager<User> _userManager;

        public JWTService(UserManager<User> userManager)
        {
            _userManager = userManager;
        }

        public async Task<string> GenerateJWT(User user)
        {
            SymmetricSecurityKey key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes("{8CCC4F4C-47CC-4C90-B4CF-C69795A501A2-A8E4E217-0AC5-4E75-B5E9-01317A10D12F}"));
            List<Claim> claims = new List<Claim>()
            {
                new(JwtRegisteredClaimNames.NameId,user.UserName),
                new(JwtRegisteredClaimNames.Sid,user.Id.ToString()),

            };
            var roles = await _userManager.GetRolesAsync(user);
            if (roles.Any())
            {
            claims.AddRange(roles.Select(x => new Claim(ClaimTypes.Role, x)));
            }
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha512Signature);
            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(claims),
                Expires = DateTime.UtcNow.AddDays(5),
                SigningCredentials = creds,
            };
            var tokenHandler=new JwtSecurityTokenHandler();
            var token=tokenHandler.CreateToken(tokenDescriptor);
            return tokenHandler.WriteToken(token);
        }
    }
}
