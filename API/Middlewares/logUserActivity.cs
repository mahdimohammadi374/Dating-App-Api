using API.Extensions;
using API.Interfaces;
using Microsoft.AspNetCore.Mvc.Filters;

namespace API.Middlewares
{
    public class logUserActivity : IAsyncActionFilter
    {
        public async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
        {
            var resultcontext = await next();
            if (!resultcontext.HttpContext.User.Identity.IsAuthenticated) { return; }

            var userName=resultcontext.HttpContext.User.GetUserName();
            var rep=resultcontext.HttpContext.RequestServices.GetService<IUnitOfWork>();
            var user=await rep.UserRepository.GetUserByUserName(userName);
            user.LastActive=DateTime.Now;
            rep.UserRepository.Update(user);
            await rep.CompleteAsync();

        }
    }
}
