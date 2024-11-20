using System.Text;
using Newtonsoft.Json;
using RabbitMQ.Client;

namespace SchoolApi.Helper
{
    public class RabbitMQProducer
    {
        private readonly IConfiguration _configuration;

        public RabbitMQProducer(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public void SendMessage<T>(T message)
        {
            var factory = new ConnectionFactory
            {
                HostName = _configuration["RabbitMQ:Host"],
                Port = int.Parse(_configuration["RabbitMQ:Port"]),
                UserName = _configuration["RabbitMQ:Username"],
                Password = _configuration["RabbitMQ:Password"]
            };

            using var connection = factory.CreateConnection();
            using var channel = connection.CreateModel();

            var exchange = _configuration["RabbitMQ:Exchange"];
            var queue = _configuration["RabbitMQ:Queue"];
            var routingKey = _configuration["RabbitMQ:RoutingKey"];

            channel.ExchangeDeclare(exchange, ExchangeType.Direct, true);
            channel.QueueDeclare(queue, true, false, false, null);
            channel.QueueBind(queue, exchange, routingKey, null);

            var messageBody = Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(message));
            channel.BasicPublish(exchange, routingKey, null, messageBody);
        }
    }


}