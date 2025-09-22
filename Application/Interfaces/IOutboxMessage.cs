using TMB.Challenge.Application.DTOs.Order;
using TMB.Challenge.Domain.Entities;

namespace TMB.Challenge.Application.Interfaces;

public interface IOutboxMessage
{
    Task AddAsync(OrderMessageDTO message);
}
