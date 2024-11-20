using System.Net.Mail;

namespace SchoolApi.Core.Extensions
{
    public class EmailModel
    {
        public string Sender { get; set; } = string.Empty;
        public string Recipient { get; set; } = string.Empty;
        public string Subject { get; set; } = string.Empty;
        public string Content { get; set; } = string.Empty;
    }
}