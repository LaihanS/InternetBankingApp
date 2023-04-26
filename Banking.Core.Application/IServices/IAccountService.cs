using Banking.Core.Application.Dtos.Account;
using Banking.Core.Application.ViewModels.UserVMS;

namespace Banking.Core.Application.IServices
{
    public interface IAccountService
    {
        Task<AuthenticationResponse> AuthAsync(AuthenticationRequest request);
        Task<string> ConfirmUserAsync(string userId, string token);
        Task<ForgotPassworResponse> ForgotPasswordAsync(ForgotPassworRequest request, string origin);
        Task<RegisterResponse> RegisterBasicUserAsync(RegisterRequest request, string origin, bool IsAdmin);
        Task<ResetPasswordResponse> ResetPasswordAsync(ResetPasswordRequest reset);
        Task ActivateOrInactivateUser(string id);
        Task EditUser(UserViewModel edituser);
        //Task DeleteUserAsync(UserViewModel uservm);
        Task<List<UserViewModel>> GetUsers(List<string> userids);

        //Task<UserViewModel> GetEditAsync(string id);
        Task SingOutAsync();
    }
}