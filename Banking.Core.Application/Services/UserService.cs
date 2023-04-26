using AutoMapper;
using Banking.Core.Application.Dtos.Account;
using Banking.Core.Application.Dtos.ImportantDto;
using Banking.Core.Application.Enums;
using Banking.Core.Application.Helpers;
using Banking.Core.Application.IRepositories;
using Banking.Core.Application.IServices;
using Banking.Core.Application.ViewModels.Product;
using Banking.Core.Application.ViewModels.UserVMS;
using Banking.Core.Domain.Entities;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Banking.Core.Application.Services
{
    public class UserService : IUserService
    {
        private readonly IAccountService _accountService;
        private readonly IMapper imapper;
        private readonly IUserRepository userRepository;
        private readonly IProductRepository productRepository;
        IHttpContextAccessor httpContextAccessor;
        AuthenticationResponse User = new();

        public UserService(IHttpContextAccessor httpContextAccessor, IProductRepository productRepository, IUserRepository userRepository, IMapper imapper, IAccountService accountService)
        {
            this.httpContextAccessor = httpContextAccessor;
            User = httpContextAccessor.HttpContext.Session.Get<AuthenticationResponse>("user");
            this.userRepository = userRepository;
            this.imapper = imapper;
            this.productRepository = productRepository;
            _accountService = accountService;
        }


        public async Task<List<UserViewModel>> GetAllUsersAsync()
        {
            List<UserViewModel> userlist = imapper.Map<List<UserViewModel>>(await userRepository.GetAsync());
            return userlist;

        }

        public async Task<List<UserViewModel>> ActiveClients()
        {
            List<UserViewModel> userlist = imapper.Map<List<UserViewModel>>(await userRepository.GetAsync());
            List<string> userids = new();

            if (userlist != null)
            {
                foreach (UserViewModel viewmodel in userlist)
                {
                    userids.Add(viewmodel.Id);
                }
            }

            List<UserViewModel> userlistas = await _accountService.GetUsers(userids);

            List<UserViewModel> clients = new();

            clients = userlistas.Where(u => u.EmailConfirmed == true && u.Roles.Any(r => r.Equals(EnumRoles.Basic.ToString()))).ToList();


            return clients;

        }


        public async Task<List<UserViewModel>> UnactiveClients()
        {
            List<UserViewModel> userlist = imapper.Map<List<UserViewModel>>(await userRepository.GetAsync());  
            List<string> userids = new();

            if (userlist != null)
            {
                foreach (UserViewModel viewmodel in userlist)
                {
                    userids.Add(viewmodel.Id);
                }
            }
           
            List<UserViewModel> userlistas = await _accountService.GetUsers(userids);

            List<UserViewModel> clients = new();
         
             clients = userlistas.Where(u => u.EmailConfirmed == false && u.Roles.Any(r => r.Equals(EnumRoles.Basic.ToString()))).ToList();  

            return clients;

        }


        public async Task<List<UserViewModel>> GetAllUsersAsyncJoined()
        {
            List<UserViewModel> userlist = imapper.Map<List<UserViewModel>>(await userRepository.GetAsync());
            List<string> userids = new();

            foreach (UserViewModel viewmodel in userlist)
            {
                userids.Add(viewmodel.Id);
            }

            List<UserViewModel> userlistas = await _accountService.GetUsers(userids);
            List<UserViewModel> users = userlistas.Where(users => users.Id != User.id).ToList();
            return users;

        }


        public async Task ActivateOrInactivate(string id)
        {
            await _accountService.ActivateOrInactivateUser(id);
        }

        public async Task<AuthenticationResponse> LoginAsync(LoginViewModel loginvm)
        {

            AuthenticationRequest loginrequest = imapper.Map<AuthenticationRequest>(loginvm);

            AuthenticationResponse authenticationResponse = await _accountService.AuthAsync(loginrequest);

            return authenticationResponse;

        }

        public async Task SignOutAsync()
        {
            await _accountService.SingOutAsync();
        } 

        public async Task DeleteUserAsync(UserViewModel user)
        {
            List<Products> products = await productRepository.GetAsync();
            if (products != null)
            {
                products = products.Where(res => res.UserID == user.Id).ToList();

                foreach (Products item in products)
                {
                    await productRepository.DeleteAsync(item);
                }
                await userRepository.DeleteAsync(imapper.Map<UserDto>(user), user.Id);
            }
         
        }


        public async Task<RegisterResponse> RegisterAsync(SaveUserViewModel saveuservm, string origin)
        {
            Sequence9Digit sequence9Digit = new();
            if (saveuservm.Monto == null)
            {
                saveuservm.Monto = 0;
            }

            RegisterRequest registerRequest = imapper.Map<RegisterRequest>(saveuservm);

            RegisterResponse response = await _accountService.RegisterBasicUserAsync(registerRequest, origin, saveuservm.IsAdmin);

            if (!saveuservm.IsAdmin && response.HasError != true)
            {
                List<Products> products = await productRepository.GetAsync();
                
                List<string> sequences = products.Select(p => p.UnicDigitSequence).ToList();

                Products product = new();
                product.UnicDigitSequence = sequence9Digit.Secuencia(sequences);
                product.IsPrincipalAccount = true;
                product.ProductAmount = saveuservm.Monto;
                product.ProductType = ProductTypeEnum.Cuenta_Ahorro.ToString();
                product.UserID = response.UserID;

                Products respones =  await productRepository.AddAsync(product);

                Products product0k = respones;

            }

            return response;
        }

        public async Task EditUser(SaveUserViewModel uservmsave)
        {
            await userRepository.EditAsync(imapper.Map<UserDto>(uservmsave), uservmsave.Id);
          
            await _accountService.EditUser(imapper.Map<UserViewModel>(uservmsave));

            List<Products> accounts = await productRepository.GetAsync();

            Products cuentaprincipal = new();

            if (accounts != null)
            {
                cuentaprincipal = accounts.Find(acc => acc.UserID == uservmsave.Id && acc.IsPrincipalAccount);
            }

            if (uservmsave.Monto != 0 && uservmsave.Monto != null)
            {
                cuentaprincipal.ProductAmount += uservmsave.Monto;
                await productRepository.EditAsync(cuentaprincipal, cuentaprincipal.id);
            }
        }

        public async Task<SaveUserViewModel> GetEditAsync(string id)
        {
           UserViewModel user = imapper.Map<UserViewModel>(await userRepository.GetByidAsync(id));

            return imapper.Map<SaveUserViewModel>(user);
        }


        public async Task<string> ConfirmAsync(string userid, string token)
        {
            return await _accountService.ConfirmUserAsync(userid, token);
        }

        public async Task<ForgotPassworResponse> ForgotPasswordAsync(ForgotPasswordViewModel forgotPasswordvm, string origin)
        {
            ForgotPassworRequest forgotPassworRequest = imapper.Map<ForgotPassworRequest>(forgotPasswordvm);

            return await _accountService.ForgotPasswordAsync(forgotPassworRequest, origin);
        }

        public async Task<ResetPasswordResponse> ResetPasswordAsync(ResetPasswordViewModel resetPasswordvm, string origin)
        {
            ResetPasswordRequest resetPassworRequest = imapper.Map<ResetPasswordRequest>(resetPasswordvm);

            return await _accountService.ResetPasswordAsync(resetPassworRequest);
        }
    }
}

