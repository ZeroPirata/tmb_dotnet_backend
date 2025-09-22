using System.Text.Json;
using Microsoft.EntityFrameworkCore;
using TMB.Challenge.Application.Interfaces;
using TMB.Challenge.Infrastructure.Data;

namespace TMB.Challenge.Worker.Processors;

public class OutboxProcessorWorker(
    ILogger<OutboxProcessorWorker> logger,
    IMessageBusService messageBus,
    IServiceProvider serviceProvider) : BackgroundService
{
    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        logger.LogInformation("Processador de Outbox iniciado.");
        while (!stoppingToken.IsCancellationRequested)
        {
            await ProcessOutboxMessages(stoppingToken);
            await Task.Delay(TimeSpan.FromSeconds(10), stoppingToken);
        }
    }

    private async Task ProcessOutboxMessages(CancellationToken stoppingToken)
    {
        using var scope = serviceProvider.CreateScope();
        var dbContext = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();

        var pendingMessages = await dbContext.OutboxMessages
            .Where(m => m.Status == "Pendente")
            .OrderBy(m => m.CreatedAt)
            .Take(20)
            .ToListAsync(stoppingToken);

        foreach (var message in pendingMessages)
        {
            try
            {
                object? orderMessage = null;
                if (!string.IsNullOrEmpty(message.Payload))
                {
                    orderMessage = JsonSerializer.Deserialize<object>(message.Payload);
                }
                if (orderMessage != null)
                {
                    await messageBus.PublishMessageAsync(orderMessage, "orders");
                }
                message.Status = "Processado";
                await dbContext.SaveChangesAsync(stoppingToken);
                logger.LogInformation("Mensagem Outbox {MessageId} processada e enviada.", message.Id);
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Erro ao processar mensagem Outbox {MessageId}.", message.Id);
            }
        }
    }
}