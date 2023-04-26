using AutoMapper;
using Banking.Core.Application.Dtos.Account;
using Banking.Core.Application.Enums;
using Banking.Core.Application.Helpers;
using Banking.Core.Application.IServices;
using Banking.Core.Application.ViewModels.UserVMS;
using Banking.Infrastructure.Identity.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using WebApp.Banking.Middlewares;

namespace WebApp.Banking.Controllers
{
    public class UserController : Controller
    {
        //[Authorize(Roles = "SuperAdmin, Admin, Basic")] (autoriza a cualquiera que esté-
        //-logueado con esos roles redirect al user index)
        private readonly IMapper _mapper;
        private readonly IUserService userService;
        private readonly IHttpContextAccessor http;
        public UserController(IHttpContextAccessor http, IMapper _mapper, IUserService userService)
        {
            this.http = http;
            this._mapper = _mapper;
            this.userService = userService;

        }

        [ServiceFilter(typeof(LoginAuthorize))]
        public async Task<IActionResult> Index()
        {
            return View(new LoginViewModel());
        }

        [ServiceFilter(typeof(LoginAuthorize))]
        [HttpPost]
        public async Task<IActionResult> Index(LoginViewModel loginvm)
        {
            if (!ModelState.IsValid)
            {
                return View(loginvm);
            }

            AuthenticationResponse response = await userService.LoginAsync(loginvm);
            if (response != null && response.HasError != true)
            {
                if (response.Roles.Any(r => r.Equals(EnumRoles.Admin.ToString())))
                {
                    http.HttpContext.Session.Set<AuthenticationResponse>("user", response);
                    return RedirectToRoute(new { controller = "UserMantainment", action = "Index" });
                }
                else
                {
                    http.HttpContext.Session.Set<AuthenticationResponse>("user", response);
                    return RedirectToRoute(new { controller = "ClientUser", action = "Index" });
                }
            }
            else 
            {
                loginvm.HasError = response.HasError;
                loginvm.ErrorDetails = response.ErrorDetails;
                return View(loginvm);
            }
        }

        public async Task<IActionResult> Logout()
        {
            await userService.SignOutAsync();
            http.HttpContext.Session.Remove("user");
            return RedirectToRoute(new { controller = "User", action = "Index" });
        }

        [ServiceFilter(typeof(LoginAuthorize))]
        public async Task<IActionResult> ForgotPassword()
        {
   
            return View(new ForgotPasswordViewModel());
        }

        [ServiceFilter(typeof(LoginAuthorize))]
        [HttpPost]
        public async Task<IActionResult> ForgotPassword(ForgotPasswordViewModel forgotpassvm)
        {
            if (!ModelState.IsValid)
            {
                return View("ForgotPassword", forgotpassvm);

            }
            string origin = Request.Headers["origin"];
            ForgotPassworResponse responseforgot = await userService.ForgotPasswordAsync(forgotpassvm, origin);
            if (responseforgot.HasError)
            {
                forgotpassvm.HasError = responseforgot.HasError;
                forgotpassvm.ErrorDetails = responseforgot.ErrorDetails;
                return View("ForgotPassword", forgotpassvm);
            }
            return RedirectToRoute(new { controller = "User", action = "Index" });

        }

        [ServiceFilter(typeof(LoginAuthorize))]
        public async Task<IActionResult> ResetPassword(ResetPasswordViewModel vm)
        {
             return View(vm);
        }

        [ServiceFilter(typeof(LoginAuthorize))]
        [HttpPost]
        public async Task<IActionResult> ResetPasswordPost(ResetPasswordViewModel vm)
        {
            if (!ModelState.IsValid)
            {
                return View("ForgotPassword", vm);

            }
            string origin = Request.Headers["origin"];
            ResetPasswordResponse responsereset = await userService.ResetPasswordAsync(vm, origin);
            if (responsereset.HasError)
            {
                vm.HasError = responsereset.HasError;
                vm.ErrorDetails = responsereset.ErrorDetails;
                return View("ForgotPassword", responsereset);
            }
            return RedirectToRoute(new { controller = "User", action = "Index" });
        }

        public async Task<IActionResult> AccessDenied(ForgotPasswordViewModel passwordViewModel)
        {

            return View(passwordViewModel.ErrorDetails);
        }
    }
}
