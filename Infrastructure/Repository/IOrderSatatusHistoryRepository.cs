using TMB.Challenge.Application.Interfaces;
using TMB.Challenge.Domain.Entities;
using TMB.Challenge.Infrastructure.Data;

namespace TMB.Challenge.Infrastructure.Repository;

public class OrderStatusHistoryRepository(ApplicationDbContext context) : IOrderStatusHistoryRepository
{
    private readonly ApplicationDbContext _context = context;

    public async Task<OrderStatusHistory> AddAsync(OrderStatusHistory orderStatusHistory)
    {
        await _context.OrderStatusHistories.AddAsync(orderStatusHistory);
        await _context.SaveChangesAsync();
        return orderStatusHistory;
    }
}
