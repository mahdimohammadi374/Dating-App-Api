using System.Security.Claims;

namespace API.Extensions
{
    public static class ClaimsPrincipleExtention
    {
        public static string GetUserName(this ClaimsPrincipal principal)
        {
            return principal.FindFirst(ClaimTypes.NameIdentifier).Value;
        }
        public static int GetUserId(this ClaimsPrincipal principal)
        {
            var userId=principal.Claims.ToList().FirstOrDefault(x=>x.Type=="sid").Value;
            return int.Parse(userId);
            //return Convert.ToInt32(principal.FindFirst(ClaimTypes.Sid)?.Value);
        }
    }
}
