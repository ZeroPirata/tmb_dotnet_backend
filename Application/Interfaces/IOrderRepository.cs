// Dentro do projeto Application
using TMB.Challenge.Application.Common;
using TMB.Challenge.Domain.Entities;
using TMB.Challenge.Domain.Enum;


namespace TMB.Challenge.Application.Interfaces;

public interface IOrderRepository
{
    Task<Order> AddAsync(Order order);
    Task<Order?> GetByUuidAsync(string uuid);
    Task<PaginatedResult<Order>> GetAllAsync(int pageNumber, int pageSize, OrderStatus? status = null);
    Task<Order> UpdateAsync(Order order);
}