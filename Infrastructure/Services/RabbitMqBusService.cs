using RabbitMQ.Client;
using System.Text;
using System.Text.Json;
using TMB.Challenge.Application.Interfaces;
using Microsoft.Extensions.Configuration;
using TMB.Challenge.Application.DTOs.Order;

namespace TMB.Challenge.Infrastructure.Services;

public class RabbitMqBusService : IMessageBusService, IDisposable
{
    private readonly IConnection _connection;
    private readonly IModel _channel;

    public RabbitMqBusService(IConfiguration configuration)
    {
        var factory = new ConnectionFactory() { HostName = "rabbitmq" };
        _connection = factory.CreateConnection();
        _channel = _connection.CreateModel();
    }

    public Task PublishMessageAsync(object message, string queueName)
    {
        _channel.QueueDeclare(queue: queueName, durable: true, exclusive: false, autoDelete: false, arguments: null);
        var jsonMessage = JsonSerializer.Serialize(message);
        var body = Encoding.UTF8.GetBytes(jsonMessage);
        var properties = _channel.CreateBasicProperties();
        properties.Persistent = true;

        if (message is OrderMessageDTO orderMessage)
        {
            properties.CorrelationId = orderMessage.Uuid;
            properties.Type = "OrderCreated";
        }

        _channel.BasicPublish(exchange: "", routingKey: queueName, basicProperties: properties, body: body);
        return Task.CompletedTask;
    }

    public void Dispose()
    {
        _channel?.Close();
        _connection?.Close();
    }
}