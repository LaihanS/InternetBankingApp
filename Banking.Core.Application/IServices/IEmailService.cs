using Banking.Core.Application.Dtos.Email;

namespace Banking.Core.Application.IServices
{
    public interface IEmailService
    {
        Task SendAsync(EmailRequest email);
    }
}