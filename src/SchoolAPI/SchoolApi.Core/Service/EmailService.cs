using System.Net;
using System.Net.Mail;
using Microsoft.Extensions.Configuration;
using SchoolApi.Core.Extensions;

namespace SchoolApi.Core.Service
{
    public class EmailService:IEmailService
    {
        private readonly IConfiguration _configuration;
        public EmailService(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public async Task SendEmail(EmailModel emailData)
        {
            Console.WriteLine("Email Conf");
            var smtpClient = new SmtpClient
            {
                DeliveryMethod = SmtpDeliveryMethod.Network,
                UseDefaultCredentials = false,
                Host = _configuration["Smtp:Server"],
                Port = int.Parse(_configuration["Smtp:Port"]),
                Credentials = new NetworkCredential(_configuration["Smtp:Username"], _configuration["Smtp:Password"]),
                EnableSsl = true
            };

            var mailMessage = new MailMessage
            {
                From = new MailAddress (emailData.Sender,"Student Portal"),
                Subject = emailData.Subject,
                Body = emailData.Content,
                IsBodyHtml = false
            };

            mailMessage.To.Add(emailData.Recipient);
            Console.WriteLine("Sending Email...");
            await smtpClient.SendMailAsync(mailMessage);
            Console.WriteLine("Email Sent");
        }
        
    }
}