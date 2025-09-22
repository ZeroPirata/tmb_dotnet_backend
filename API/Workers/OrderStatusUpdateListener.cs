

using System.Text;
using System.Text.Json;
using Microsoft.AspNetCore.SignalR;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using RabbitMQ.Client.Exceptions;
using TMB.Challenge.API.Hubs;

namespace TMB.Challenge.API.Workers;

public class OrderStatusUpdateListener(ILogger<OrderStatusUpdateListener> logger, IHubContext<OrdersHub> hubContext, IConfiguration configuration) : BackgroundService
{
    private readonly ILogger<OrderStatusUpdateListener> _logger = logger;
    private readonly IHubContext<OrdersHub> _hubContext = hubContext;
    private IConnection? _rabbitConnection;
    private IModel? _channel;
    private readonly IConfiguration _configuration = configuration;

    public override async Task StartAsync(CancellationToken cancellationToken)
    {
        var rabbitmqHost = _configuration["RABBITMQ_HOST"] ?? "rabbitmq";
        var factory = new ConnectionFactory() { HostName = rabbitmqHost, DispatchConsumersAsync = true };

        for (int i = 0; i < 5; i++)
        {
            try
            {
                _rabbitConnection = factory.CreateConnection();
                _channel = _rabbitConnection.CreateModel();
                _logger.LogInformation("OrderStatusUpdateListener conectado ao RabbitMQ com sucesso.");
                await base.StartAsync(cancellationToken);
                return;
            }
            catch (BrokerUnreachableException ex)
            {
                _logger.LogWarning(ex, "Não foi possível conectar ao RabbitMQ. Tentando novamente em 5s... ({Attempt})", i + 1);
                await Task.Delay(5000, cancellationToken);
            }
        }
        _logger.LogError("Falha ao conectar no RabbitMQ após múltiplas tentativas. O serviço não iniciará.");
    }

    protected override Task ExecuteAsync(CancellationToken stoppingToken)
    {
        if (_channel is null || stoppingToken.IsCancellationRequested)
        {
            return Task.CompletedTask;
        }

        stoppingToken.Register(() => _logger.LogInformation("OrderStatusUpdateListener está parando."));
        var queueName = "order-status-updates";
        _channel.QueueDeclare(queue: queueName, durable: true, exclusive: false, autoDelete: false, arguments: null);

        var consumer = new AsyncEventingBasicConsumer(_channel);
        consumer.Received += async (model, ea) =>
        {
            try
            {
                var body = ea.Body.ToArray();
                var message = Encoding.UTF8.GetString(body);
                var update = JsonSerializer.Deserialize<JsonElement>(message);
                var uuid = update.GetProperty("Uuid").GetString();
                var status = update.GetProperty("Status").GetString();
                var history = update.GetProperty("StatusHistories");

                if (!string.IsNullOrEmpty(uuid))
                    await _hubContext.Clients.All.SendAsync("OrderStatusUpdated", uuid, status, history, stoppingToken);
                _channel.BasicAck(ea.DeliveryTag, false);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao processar mensagem no OrderStatusUpdateListener.");
                _channel.BasicNack(ea.DeliveryTag, false, false);
            }
        };
        _channel.BasicConsume(queue: queueName, autoAck: false, consumer: consumer);
        return Task.Delay(Timeout.Infinite, stoppingToken);
    }

    public override void Dispose()
    {
        _channel?.Dispose();
        _rabbitConnection?.Dispose();
        base.Dispose();
    }
}