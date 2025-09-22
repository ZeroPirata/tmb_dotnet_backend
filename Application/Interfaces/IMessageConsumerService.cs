
namespace TMB.Challenge.Application.Interfaces;

public interface IMessageConsumerService
{
    Task ProcessMessagesAsync(CancellationToken stoppingToken);
}