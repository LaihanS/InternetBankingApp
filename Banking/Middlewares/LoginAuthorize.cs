using Microsoft.AspNetCore.Mvc.Filters;
using WebApp.Banking.Controllers;
using Microsoft.AspNetCore.Http;
using Banking.Core.Application.Dtos.Account;
using Banking.Core.Application.Helpers;
using Banking.Core.Application.ViewModels.UserVMS;
using Banking.Core.Application.Enums;

namespace WebApp.Banking.Middlewares
{
    public class LoginAuthorize : IAsyncActionFilter
    {
        private readonly ValidateUserSession _userSession;
        IHttpContextAccessor httpContextAccessor;
        AuthenticationResponse User = new();
        public LoginAuthorize(IHttpContextAccessor httpContextAccessor, ValidateUserSession userSession)
        {
            this.httpContextAccessor = httpContextAccessor;
            _userSession = userSession;
           User =  httpContextAccessor.HttpContext.Session.Get<AuthenticationResponse>("user");
        }

        public async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
        {
            if (_userSession.HasUser())
            {
                var controller = (UserController)context.Controller;
                if (User.Roles.Any(r => r.Equals(EnumRoles.Basic.ToString()) && User.Roles.Count == 1))
                {
                    context.Result = controller.RedirectToAction("Index", "ClientUser");
                }
                else if (User.Roles.Any(r => r.Equals(EnumRoles.Admin.ToString())))
                {
                    context.Result = controller.RedirectToAction("Index", "UserMantainment");
                }
                            
            }
            else
            {
                await next();
            }
        }
    }
}
