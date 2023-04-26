using Banking.Core.Application.Dtos.Account;
using Banking.Core.Application.ViewModels.UserVMS;

namespace Banking.Core.Application.IServices
{
    public interface IUserService
    {
        Task<string> ConfirmAsync(string userid, string token);
        Task<ForgotPassworResponse> ForgotPasswordAsync(ForgotPasswordViewModel forgotPasswordvm, string origin);
        Task<AuthenticationResponse> LoginAsync(LoginViewModel loginvm);
        Task<RegisterResponse> RegisterAsync(SaveUserViewModel saveuservm, string origin);
        Task<ResetPasswordResponse> ResetPasswordAsync(ResetPasswordViewModel resetPasswordvm, string origin);
        Task<List<UserViewModel>> GetAllUsersAsync();
        Task<List<UserViewModel>> GetAllUsersAsyncJoined();
        Task<List<UserViewModel>> ActiveClients();
        Task<List<UserViewModel>> UnactiveClients();
        Task DeleteUserAsync(UserViewModel user);
        Task ActivateOrInactivate(string id);
        Task<SaveUserViewModel> GetEditAsync(string id);
        Task EditUser(SaveUserViewModel uservmsave);
        Task SignOutAsync();
    }
}