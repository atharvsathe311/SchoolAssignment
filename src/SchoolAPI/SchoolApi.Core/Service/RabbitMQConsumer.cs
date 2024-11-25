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
        private readonly IEmailService _emailService;

        public RabbitMQConsumer(IConfiguration configuration, IEmailService emailService)
        {
            _configuration = configuration;
            _emailService = emailService;
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
            var queue = _configuration["RabbitMQ:Queue1"];

            var consumer = new EventingBasicConsumer(channel);
            consumer.Received += (model, args) =>
            {
                var body = args.Body.ToArray();
                var message = Encoding.UTF8.GetString(body);

                var data = JsonConvert.DeserializeObject<Event<StudentGetDTO>>(message) ?? throw new Exception("Null");

                if (data.EventType == EventType.StudentCreated)
                {
                    var email = new EmailModel
                    {
                        Sender = _configuration["Smtp:Username"],
                        Recipient = data.Content.Email,
                        Subject = "Student Registered",
                        Content = $"Dear {data.Content.FirstName}  {data.Content.LastName} , Your Student ID is : {data.Content.StudentId}."
                    };

                    _emailService.SendEmail(email);
                }

                if (data.EventType == EventType.StudentUpdated)
                {
                    var email = new EmailModel
                    {
                        Sender = _configuration["Smtp:Username"],
                        Recipient = data.Content.Email,
                        Subject = "Student Updated",
                        Content = $"Dear {data.Content.FirstName}  {data.Content.LastName} , Your Account has been updated"
                    };

                    _emailService.SendEmail(email);
                }

                
                if (data.EventType == EventType.StudentDeleted)
                {
                    var email = new EmailModel
                    {
                        Sender = _configuration["Smtp:Username"],
                        Recipient = data.Content.Email,
                        Subject = "Student Registered",
                        Content = $"Dear {data.Content.FirstName}  {data.Content.LastName} , Your account with Student ID is : {data.Content.StudentId} is Deleted."
                    };

                    _emailService.SendEmail(email);
                }

                Console.WriteLine($"Message Received: {message}");
                channel.BasicAck(args.DeliveryTag, false);
            };

            channel.BasicConsume(queue, false, consumer);
            return Task.CompletedTask;
        }


    }

}