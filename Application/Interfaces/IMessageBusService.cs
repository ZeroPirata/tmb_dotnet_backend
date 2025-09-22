
namespace TMB.Challenge.Application.Interfaces;

public interface IMessageBusService
{
    Task PublishMessageAsync(object message, string queueOrTopicName);
}