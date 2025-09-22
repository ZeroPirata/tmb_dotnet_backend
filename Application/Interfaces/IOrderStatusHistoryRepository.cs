using TMB.Challenge.Domain.Entities;

namespace TMB.Challenge.Application.Interfaces;

public interface IOrderStatusHistoryRepository
{
    Task<OrderStatusHistory> AddAsync(OrderStatusHistory order);

}
