using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System.Text;
using System.Text.Json;
using TMB.Challenge.Application.DTOs.Order;
using TMB.Challenge.Application.Handler;
using TMB.Challenge.Application.Interfaces;

namespace TMB.Challenge.Infrastructure.Services;

public class RabbitMqConsumerService : IMessageConsumerService
{
    private readonly IModel _channel;
    private readonly IServiceProvider _serviceProvider;
    private readonly ILogger<RabbitMqConsumerService> _logger;

    public RabbitMqConsumerService(IServiceProvider serviceProvider, ILogger<RabbitMqConsumerService> logger)
    {
        var factory = new ConnectionFactory() { HostName = "rabbitmq" };
        var connection = factory.CreateConnection();
        _channel = connection.CreateModel();
        _serviceProvider = serviceProvider;
        _logger = logger;
    }

    public Task ProcessMessagesAsync(CancellationToken stoppingToken)
    {
        var queueName = "orders";
        _channel.QueueDeclare(queue: queueName, durable: true, exclusive: false, autoDelete: false, arguments: null);
        var consumer = new EventingBasicConsumer(_channel);

        consumer.Received += async (model, ea) =>
        {
            using (var scope = _serviceProvider.CreateScope())
            {
                var orderHandler = scope.ServiceProvider.GetRequiredService<OrderHandler>();
                var body = ea.Body.ToArray();
                var messageJson = Encoding.UTF8.GetString(body);
                var orderMessage = JsonSerializer.Deserialize<OrderMessageDTO>(messageJson);

                if (orderMessage is not null)
                {
                    await orderHandler.ProcessOrderFromQueue(orderMessage);
                }
            }
            _channel.BasicAck(deliveryTag: ea.DeliveryTag, multiple: false);
        };

        _channel.BasicConsume(queue: queueName, autoAck: false, consumer: consumer);
        _logger.LogInformation("Worker conectado à fila '{queueName}'.", queueName);
        return Task.CompletedTask;
    }
}