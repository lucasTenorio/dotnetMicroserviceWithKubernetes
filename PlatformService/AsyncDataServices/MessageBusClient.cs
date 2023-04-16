using System.Text;
using PlatformService.Dtos;
using RabbitMQ.Client;

namespace PlatformService.AsyncDataServices
{
    public class MessageBusClient : IMessageBusClient
    {
        private readonly IConfiguration _configuration;
        private readonly IConnection? _connection;
        private readonly IModel? _channel;

        public MessageBusClient(IConfiguration configuration)
        {
            _configuration = configuration;
            var factory = SettingUpRabbitMQ();
            try
            {
                _connection = factory.CreateConnection();
                _channel = _connection.CreateModel();

                _channel.ExchangeDeclare(exchange: "trigger", type: ExchangeType.Fanout);

                _connection.ConnectionShutdown += RabbitMQ_ConnectionShutdown;

                Console.WriteLine("--> RabbitMQ connected");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"--> Could not connect to the Message Bus : {ex.Message}");
            }
        }

        private ConnectionFactory SettingUpRabbitMQ()
        {
            if (_configuration == null)
                throw new ArgumentException($"The host is not available");
            int port;

            if (!int.TryParse(_configuration["RabbitMQPort"], out port)) throw new ArgumentException($"The port is not a valid port {_configuration["RabbitMQPort"]}");

            Console.WriteLine($"Trying connect to following host: {_configuration["RabbitMQHost"]}{port}");
            return new ConnectionFactory() { HostName = _configuration["RabbitMQHost"], Port = port };
        }

        private void RabbitMQ_ConnectionShutdown(object? sender, ShutdownEventArgs e)
        {
            Console.WriteLine("--> RabbitMQ connection shutdown");
        }

        public void PublishNewPlatform(PlatformPublishedDto platformPublishedDto)
        {
            var message = System.Text.Json.JsonSerializer.Serialize(platformPublishedDto);

            if (_connection != null && _connection.IsOpen)
            {
                Console.WriteLine(" --> RabbitMQ connection is open, sending message...");
                SendMessage(message);
            }
            else
            {
                Console.WriteLine(" --> RabbitMQ connection is closed!");
            }
        }

        private void SendMessage(string message)
        {
            var body = Encoding.UTF8.GetBytes(message);

            if (_channel == null) return;

            _channel.BasicPublish(exchange: "trigger",
                                    routingKey: "",
                                    basicProperties: null,
                                    body);
            Console.WriteLine($"We have sent a message {message}");                                
        }

        public void Dispose()
        {
            if(_channel == null || _connection == null)
            {
                Console.WriteLine("Connection already disposed");
                return;
            }


            if(_channel.IsOpen)
            {
                _channel.Close();
                _connection.Close();
                Console.WriteLine("Message bus disposed");
            }
        }
    }
}