using AutoMapper;
using Banking.Core.Application.Dtos.Account;
using Banking.Core.Application.Dtos.Email;
using Banking.Core.Application.Enums;
using Banking.Core.Application.IServices;
using Banking.Core.Application.ViewModels.UserVMS;
using Banking.Infrastructure.Identity.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.WebUtilities;
using Org.BouncyCastle.Asn1.Ocsp;
using Org.BouncyCastle.Security;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Banking.Infrastructure.Identity.Services
{
    public class AccountService : IAccountService
    {
        private readonly UserManager<User> userManager;
        private readonly SignInManager<User> signInManager;
        private readonly IEmailService emailService;
        private readonly IMapper imapper;

        public AccountService(IMapper imapper, IEmailService emailService, UserManager<User> userManager, SignInManager<User> signInManager)
        {
            this.emailService = emailService;
            this.userManager = userManager;
            this.signInManager = signInManager;
            this.imapper = imapper;
        }

        public async Task<AuthenticationResponse> AuthAsync(AuthenticationRequest request)
        {
            AuthenticationResponse response = new();

            User user = await userManager.FindByEmailAsync(request.Email);

            if (user == null)
            {
                response.HasError = true;
                response.ErrorDetails = $"No se jayó un usuario con {request.Email}";
                return response;
            }

            var result = await signInManager.PasswordSignInAsync(user.UserName, request.Password, false, lockoutOnFailure: false);

            if (!result.Succeeded)
            {
                response.HasError = true;
                response.ErrorDetails = $"No se jayó un usuario para {request.Email} con esa contraseña";
                return response;
            }

            if (!user.EmailConfirmed)
            {
                response.HasError = true;
                response.ErrorDetails = $"No se ha confirmado el correo: {request.Email}";
                return response;
            }

            response.id = user.Id;
            response.UserName = user.UserName;
            var rolelist = await userManager.GetRolesAsync(user).ConfigureAwait(false);
            response.Roles = rolelist.ToList();
            response.Verified = user.EmailConfirmed;
            return response;
        }

        public async Task SingOutAsync()
        {
            await signInManager.SignOutAsync();
        }

        public async Task ActivateOrInactivateUser(string id)
        {
           User user = await userManager.FindByIdAsync(id);
            if (user.EmailConfirmed == true)
            {
                user.EmailConfirmed = false;
            }
            else
            {
                user.EmailConfirmed = true;
            }

            await userManager.UpdateAsync(user);
          
        }

        public async Task<List<UserViewModel>> GetUsers(List<string> userids)
        {
            
            //List<User> userlist = new();
            List<UserViewModel> userlista = new();

            foreach (string item in userids)
            {
                User user = await userManager.FindByIdAsync(item);
                UserViewModel uservm = imapper.Map<UserViewModel>(user);
                uservm.Roles = await userManager.GetRolesAsync(user);
                userlista.Add(uservm);
            }


            return userlista;
          
        }

        public async Task EditUser(UserViewModel edituser)
        {
           
            User user = await userManager.FindByIdAsync(edituser.Id);
        
            var token = await userManager.GeneratePasswordResetTokenAsync(user);
            await userManager.ResetPasswordAsync(user, token, edituser.Password);
        }

        //public async Task DeleteUserAsync(UserViewModel uservm)
        //{
        //    User user = imapper.Map<User>(uservm);

        //    await userManager.DeleteAsync(user);
        //}

        public async Task<string> ConfirmUserAsync(string userid, string token)
        {
            var user = await userManager.FindByIdAsync(userid);
            if (user == null)
            {
                return "No hay cuentas registradas con este usuario";
            }

            token = Encoding.UTF8.GetString(WebEncoders.Base64UrlDecode(token));
            var result = await userManager.ConfirmEmailAsync(user, token);
            if (result.Succeeded)
            {
                return $"Mete mano {user.FirstName}";
            }
            else
            {
                return $"Error confirmando el usuario {user.FirstName}";
            }
        }

        public async Task<ForgotPassworResponse> ForgotPasswordAsync(ForgotPassworRequest request, string origin)
        {
            ForgotPassworResponse response = new()
            {
                HasError = false
            };

            var account = await userManager.FindByEmailAsync(request.Email);
            if (account == null)
            {
                response.HasError = true;
                response.ErrorDetails = $"No se jayó un usuario con {request.Email}";
                return response;
            }

            var verificationuri = await VerificationPasswordUriAsync(account, origin);
            await emailService.SendAsync(new EmailRequest()
            {
                To = account.Email,
                Body = $"{account.FirstName}, recupere su contraseña accediendo aquí: {verificationuri}",
                Subject = $"Restablecimiento de contraseña, {account.FirstName}",
            });

            return response;
        }

        public async Task<ResetPasswordResponse> ResetPasswordAsync(ResetPasswordRequest reset)
        {
            ResetPasswordResponse response = new()
            {
                HasError = false
            };

            var user = await userManager.FindByEmailAsync(reset.Email);

            if (user == null)
            {
                response.HasError = true;
                response.ErrorDetails = $"No se hayó cuenta con el correo: {reset.Email}";
                return response;
            }

            reset.Token = Encoding.UTF8.GetString(WebEncoders.Base64UrlDecode(reset.Token));
            var result = await userManager.ResetPasswordAsync(user, reset.Token, reset.Password);
            if (!result.Succeeded)
            {
                response.HasError = true;
                response.ErrorDetails = $"Hubo un error al restablecer la contraseña. Revise los datos de su transaccion";
                return response;
            }

            return response;
        }

        public async Task<RegisterResponse> RegisterBasicUserAsync(RegisterRequest request, string origin, bool IsAdmin)
        {
            RegisterResponse response = new();
            response.HasError = false;

            var userWithSameName = await userManager.FindByNameAsync(request.UserName);

            if (userWithSameName != null)
            {
                response.HasError = true;
                response.ErrorDetails = $"Ya existe un usuario con el username: {request.UserName}";
                return response;
            }
            var userWithSameEmail = await userManager.FindByEmailAsync(request.Email);

            

            if (userWithSameEmail != null)
            {
                response.HasError = true;
                response.ErrorDetails = $"Ya existe un usuario con el email: {request.Email}";
                return response;
            }

            var user = new User
            {
                UserName = request.UserName,
                FirstName = request.FirstName,
                LastName = request.LastName,
                Email = request.Email,
                Cedula = request.Cedula,
            };

            var createuserresult = await userManager.CreateAsync(user, request.Password);
            if (createuserresult.Succeeded)
            {
                if (IsAdmin)
                {
                    await userManager.AddToRoleAsync(user, EnumRoles.Admin.ToString());
                }
                else
                {
                    await userManager.AddToRoleAsync(user, EnumRoles.Basic.ToString());
                }
                var verificationuri = await VerificationUriAsync(user, origin);
                //await emailService.SendAsync(new EmailRequest()
                //{
                //    To = user.Email,
                //    Body = $"{user.FirstName}, active su cuenta accediendo aquí: {verificationuri}",
                //    Subject = "Activación de cuenta",
                //});

                User ObtainUser = await userManager.FindByEmailAsync(user.Email);
                
                response.UserID = ObtainUser.Id;
            }
            else
            {
                response.HasError = true;
                response.ErrorDetails = $"No se ha podido crear el usuario";
                return response;
            }

            return response;
        }

        private async Task<string> VerificationUriAsync(User user, string origin)
        {
            var code = await userManager.GenerateEmailConfirmationTokenAsync(user);
            code = WebEncoders.Base64UrlEncode(Encoding.UTF8.GetBytes(code));
            var route = "User/ConfirmEmail";
            var uri = new Uri(string.Concat($"{origin}/", route));
            var verificationUri = QueryHelpers.AddQueryString(uri.ToString(), "userid", user.Id);
            verificationUri = QueryHelpers.AddQueryString(verificationUri, "token", code);

            return verificationUri;
        }

        private async Task<string> VerificationPasswordUriAsync(User user, string origin)
        {
            var code = await userManager.GeneratePasswordResetTokenAsync(user);
            code = WebEncoders.Base64UrlEncode(Encoding.UTF8.GetBytes(code));
            var route = "User/ResetPassword";
            var uri = new Uri(string.Concat($"{origin}/", route));
            var verificationstring = QueryHelpers.AddQueryString(uri.ToString(), "Token", code);

            return verificationstring;
        }

    }
}