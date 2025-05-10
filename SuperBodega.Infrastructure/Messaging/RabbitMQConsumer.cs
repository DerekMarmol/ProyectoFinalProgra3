using System;
using System.Collections.Generic;
using System.Text;
using System.Text.Json;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using SuperBodega.Core.Messaging;
using System.Threading.Tasks;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;

namespace SuperBodega.Infrastructure.Messaging
{
    public class RabbitMQConsumer : IMessageConsumer, IDisposable
    {
        private readonly IConnection _connection;
        private readonly IModel _channel;
        private readonly ILogger<RabbitMQConsumer> _logger;
        private readonly Dictionary<string, List<Delegate>> _handlers = new Dictionary<string, List<Delegate>>();
        private readonly Dictionary<string, string> _queueNames = new Dictionary<string, string>();

        public RabbitMQConsumer(IConfiguration configuration, ILogger<RabbitMQConsumer> logger)
        {
            _logger = logger;

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

        public void Subscribe<T>(string topic, Func<T, Task> handler)
        {
            _channel.ExchangeDeclare(exchange: topic, type: ExchangeType.Fanout, durable: true);

            var queueName = _channel.QueueDeclare().QueueName;
            _channel.QueueBind(queue: queueName, exchange: topic, routingKey: string.Empty);

            if (!_handlers.ContainsKey(topic))
            {
                _handlers[topic] = new List<Delegate>();
                _queueNames[topic] = queueName;
            }

            _handlers[topic].Add(handler);
        }

        public void StartConsuming()
        {
            foreach (var topic in _handlers.Keys)
            {
                var consumer = new EventingBasicConsumer(_channel);
                consumer.Received += async (model, ea) =>
                {
                    var body = ea.Body.ToArray();
                    var message = Encoding.UTF8.GetString(body);

                    try
                    {
                        foreach (var handler in _handlers[topic])
                        {
                            var messageType = handler.Method.GetParameters()[0].ParameterType;
                            var typedMessage = JsonSerializer.Deserialize(message, messageType);

                            await (Task)handler.DynamicInvoke(typedMessage);
                        }

                        _channel.BasicAck(deliveryTag: ea.DeliveryTag, multiple: false);
                    }
                    catch (Exception ex)
                    {
                        _logger.LogError(ex, "Error processing message from topic {Topic}: {Message}", topic, message);
                        _channel.BasicNack(deliveryTag: ea.DeliveryTag, multiple: false, requeue: true);
                    }
                };

                _channel.BasicConsume(
                    queue: _queueNames[topic],
                    autoAck: false,
                    consumer: consumer);
            }
        }

        public void StopConsuming()
        {
            _channel?.Close();
        }

        public void Dispose()
        {
            _channel?.Dispose();
            _connection?.Dispose();
        }
    }
}