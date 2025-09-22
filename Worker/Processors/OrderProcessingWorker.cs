using TMB.Challenge.Application.Interfaces;

namespace TMB.Challenge.Worker.Processors;

public class OrderProcessingWorker(ILogger<OrderProcessingWorker> logger, IMessageConsumerService consumerService) : BackgroundService
{
    private readonly ILogger<OrderProcessingWorker> _logger = logger;
    private readonly IMessageConsumerService _consumerService = consumerService;

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        _logger.LogInformation("Worker de Processamento de Pedidos iniciado.");
        await _consumerService.ProcessMessagesAsync(stoppingToken);
        _logger.LogInformation("Worker de Processamento de Pedidos está parando.");
    }
}