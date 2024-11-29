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
        private readonly ICommonSagaService _commonSagaService;
        private readonly RabbitMQProducer _producer;

        public RabbitMQConsumer(
            IConfiguration configuration,
            IEmailService emailService,
            ICommonSagaService commonSagaService,
            RabbitMQProducer producer)
        {
            _configuration = configuration;
            _emailService = emailService;
            _commonSagaService = commonSagaService;
            _producer = producer;
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

            var queueName = _configuration["RabbitMQ:Queue1"];
            channel.QueueDeclare(queue: queueName, durable: true, exclusive: false, autoDelete: false, arguments: null);

            var consumer = new EventingBasicConsumer(channel);
            consumer.Received += async (model, args) =>
            {
                var body = args.Body.ToArray();
                var message = Encoding.UTF8.GetString(body);

                var data = JsonConvert.DeserializeObject<Event<CreateStudentEventDTO>>(message);

                if (data == null)
                {
                    Console.WriteLine("Invalid message format.");
                    channel.BasicAck(args.DeliveryTag, false);
                    return;
                }

                await ProcessEvent(data, channel, args);
            };

            channel.BasicConsume(queue: queueName, autoAck: false, consumer: consumer);
            Console.WriteLine($"Listening to queue: {queueName}");

            return Task.CompletedTask;
        }

        private async Task ProcessEvent(Event<CreateStudentEventDTO> data, IModel channel, BasicDeliverEventArgs args)
        {
            try
            {
                switch (data.EventType)
                {
                    case EventType.StudentCreated:
                        await HandleStudentCreated(data);
                        break;

                    case EventType.StudentCourseEnrolled:
                        await HandleStudentCourseEnrolled(data);
                        break;

                    case EventType.StudentPaymentFailed:
                        await HandleStudentPaymentFailed(data);
                        break;
                    case EventType.StudentPaymentSucess:
                        await HandleStudentPaymentSucess(data);
                        break;
                    default:
                        Console.WriteLine($"Unknown event type: {data.EventType}");
                        break;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error processing message: {ex.Message}");
            }
            finally
            {
                channel.BasicAck(args.DeliveryTag, false);
            }
        }

        private async Task HandleStudentPaymentSucess(Event<CreateStudentEventDTO> data)
        {
             var newMessage = new Event<CreateStudentEventDTO>
            {
                EventType = EventType.StudentCreateSucess,
                Content = data.Content
            };

            _producer.SendMessage(newMessage);
        }

        private async Task HandleStudentCreated(Event<CreateStudentEventDTO> data)
        {
            bool result = await _commonSagaService.EnrolCourses(data.Content.Student.StudentId, data.Content.StudentIds);

            var newMessage = new Event<CreateStudentEventDTO>
            {
                EventType = result ? EventType.StudentCourseEnrolled : EventType.StudentCourseEnrolledFailed,
                Content = data.Content
            };

            _producer.SendMessage(newMessage);
        }

        private async Task HandleStudentCourseEnrolled(Event<CreateStudentEventDTO> data)
        {
            bool result = await _commonSagaService.UpdatePaymentStatus(data.Content.Student.StudentId);

            var newMessage = new Event<CreateStudentEventDTO>
            {
                EventType = result ? EventType.StudentPaymentSucess : EventType.StudentPaymentFailed,
                Content = data.Content
            };

            _producer.SendMessage(newMessage);

            if (result)
            {
                var email = new EmailModel
                {
                    Sender = _configuration["Smtp:Username"],
                    Recipient = data.Content.Student.Email,
                    Subject = "Student Registered",
                    Content = $"Dear {data.Content.Student.FirstName} {data.Content.Student.LastName}, Your Student ID is: {data.Content.Student.StudentId}."
                };
                _emailService.SendEmail(email);
            }
        }

        private async Task HandleStudentPaymentFailed(Event<CreateStudentEventDTO> data)
        {
            await _commonSagaService.DeleteStudent(data.Content.Student.StudentId);

            var newMessage = new Event<CreateStudentEventDTO>
            {
                EventType = EventType.StudentCreateFailed,
                Content = data.Content
            };

            _producer.SendMessage(newMessage);
        }
    }
}
