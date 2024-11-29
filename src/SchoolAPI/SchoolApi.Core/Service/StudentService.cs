
using System.Text;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using SchoolApi.Core.Extensions;

namespace SchoolApi.Core.Service
{
    public class StudentService : IStudentService
    {
        private readonly IConfiguration _configuration;
        public StudentService(IConfiguration configuration)
        {
            _configuration = configuration;
        }
        public int GetAge(DateTime birthDate)
        {
            DateTime currentDate = DateTime.Now;
            int age = currentDate.Year - birthDate.Year;

            if (currentDate < birthDate.AddYears(age))
            {
                age--;
            }
            return age;
        }

        public async Task<bool> GetStudentStatusAsync()
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

            var queueName = _configuration["RabbitMQ:Queue2"];
            channel.QueueDeclare(queue: queueName, durable: true, exclusive: false, autoDelete: false, arguments: null);

            var messageReceived = new TaskCompletionSource<bool>();
            bool taskStatus = false;

            var consumer = new EventingBasicConsumer(channel);
            consumer.Received += (model, args) =>
            {
                var body = args.Body.ToArray();
                var message = Encoding.UTF8.GetString(body);
                Event<CreateStudentEventDTO> formattedMessage = JsonConvert.DeserializeObject<Event<CreateStudentEventDTO>>(message);
                taskStatus = formattedMessage.EventType == EventType.StudentCreateFailed ;
                channel.BasicAck(deliveryTag: args.DeliveryTag, multiple: false);
                messageReceived.TrySetResult(true);
            };
            channel.BasicConsume(queue: queueName, autoAck: false, consumer: consumer);
            return !taskStatus;
        }

    }
}
