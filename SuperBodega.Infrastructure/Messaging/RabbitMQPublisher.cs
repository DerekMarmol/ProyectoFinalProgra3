using System;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using System;
using RabbitMQ.Client;
using SuperBodega.Core.Messaging;

namespace SuperBodega.Infrastructure.Messaging
{
    public class RabbitMQPublisher : IMessagePublisher, IDisposable
    {
        private readonly IConnection _connection;
        private readonly IModel _channel;

        public RabbitMQPublisher(IConfiguration configuration)
        {
            var factory = new ConnectionFactory
            {
                HostName = configuration["RabbitMQ:HostName"] ?? "localhost",
                UserName = configuration["RabbitMQ:UserName"] ?? "guest",
                Password = configuration["RabbitMQ:Password"] ?? "guest",
                Port = int.Parse(configuration["RabbitMQ:Port"] ?? "5672")
            };

            _connection = factory.CreateConnection();
            _channel = _connection.CreateModel();
        }

        public Task PublishAsync<T>(string topic, T message)
        {
            _channel.ExchangeDeclare(exchange: topic, type: ExchangeType.Fanout, durable: true);

            var json = JsonSerializer.Serialize(message);
            var body = Encoding.UTF8.GetBytes(json);

            _channel.BasicPublish(
                exchange: topic,
                routingKey: string.Empty,
                basicProperties: null,
                body: body);

            return Task.CompletedTask;
        }

        public void Dispose()
        {
            _channel?.Dispose();
            _connection?.Dispose();
        }
    }
}