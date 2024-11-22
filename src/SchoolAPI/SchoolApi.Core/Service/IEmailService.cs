using SchoolApi.Core.Extensions;

namespace SchoolApi.Core.Service
{
    public interface IEmailService
    {
        Task SendEmail(EmailModel emailData); 
    }
}