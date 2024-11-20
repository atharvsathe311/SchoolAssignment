using System.Net;
using System.Net.Mail;
using System.Text;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Newtonsoft.Json;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using SchoolApi.Core.Extensions;

namespace SchoolApi.Core.Service
{
    public class RabbitMQConsumer : BackgroundService
    {
        private readonly IConfiguration _configuration;

        public RabbitMQConsumer(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        protected override Task ExecuteAsync(CancellationToken stoppingToken)
        {
            var factory = new ConnectionFactory
            {
                HostName = _configuration["RabbitMQ:Host"],
                Port = int.Parse(_configuration["RabbitMQ:Port"]),
                UserName = _configuration["RabbitMQ:Username"],
                Password = _configuration["RabbitMQ:Password"]
            };

            var connection = factory.CreateConnection();
            var channel = connection.CreateModel();

            var exchange = _configuration["RabbitMQ:Exchange"];
            var queue = _configuration["RabbitMQ:Queue"];
            var routingKey = _configuration["RabbitMQ:RoutingKey"];

            channel.ExchangeDeclare(exchange, ExchangeType.Direct, true);
            channel.QueueDeclare(queue, true, false, false, null);
            channel.QueueBind(queue, exchange, routingKey, null);

            var consumer = new EventingBasicConsumer(channel);
            consumer.Received += (model, args) =>
            {
                var body = args.Body.ToArray();
                var message = Encoding.UTF8.GetString(body);

                var emailData = JsonConvert.DeserializeObject<EmailModel>(message);
                if (emailData==null)
                    throw new Exception("Null");

                SendEmail(emailData);

                Console.WriteLine($"Message Received: {message}");
                channel.BasicAck(args.DeliveryTag, false);
            };

            channel.BasicConsume(queue, false, consumer);
            return Task.CompletedTask;
        }

        private async void SendEmail(EmailModel emailData)
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