using System.Text;
using CommandService.EventProcessing;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;

namespace CommandService.AsyncDataServices
{
    public class MessageBusSubscriber : BackgroundService
    {
        private readonly IConfiguration _configuration;
        private readonly IEventProcessor _eventProcessor;
        private IConnection _connection;
        private IModel _channel;
        private string _queueName;

        public MessageBusSubscriber(IConfiguration configuration, IEventProcessor eventProcessor)
        {
            _configuration = configuration;
            _eventProcessor = eventProcessor;
            InitializeRabbitMQ();
        }

        private void InitializeRabbitMQ()
        {
            var factory = SettingUpRabbitMQ();
            try
            {
                _connection = factory.CreateConnection();
                _channel = _connection.CreateModel();

                _channel.ExchangeDeclare(exchange: "trigger", type: ExchangeType.Fanout);
                _queueName = _channel.QueueDeclare().QueueName;
                _channel.QueueBind(queue: _queueName, exchange: "trigger", routingKey: "");


                Console.WriteLine("--> RabbitMQ connected");
                Console.WriteLine("--> Listening on the message Bus...");
            
                _connection.ConnectionShutdown += RabbitMQ_ConnectionShutdown;
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

        protected override Task ExecuteAsync(CancellationToken stoppingToken)
        {
            stoppingToken.ThrowIfCancellationRequested();
            
            var consumer = new EventingBasicConsumer(_channel);

            consumer.Received += (ModuleHandle, ea) =>
            {
                Console.WriteLine("--> Event recieved");
                var body = ea.Body;
                
                var notificationMessage = Encoding.UTF8.GetString(body.ToArray());
                
                _eventProcessor.ProcessEvent(notificationMessage);

            };
            
            _channel.BasicConsume(queue : _queueName, autoAck: true, consumer: consumer);

            return Task.CompletedTask;
        }

        public override void Dispose()
        {
            if(_channel.IsOpen)
            {
                _channel.Close();
                _connection.Close();
                Console.WriteLine("Message bus disposed");
            }
            base.Dispose();
        }
    }
}