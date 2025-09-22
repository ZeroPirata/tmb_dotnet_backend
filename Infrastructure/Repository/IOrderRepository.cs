using Azure.Core;
using Microsoft.EntityFrameworkCore;
using TMB.Challenge.Application.Common;
using TMB.Challenge.Application.Interfaces;
using TMB.Challenge.Domain.Entities;
using TMB.Challenge.Domain.Enum;
using TMB.Challenge.Infrastructure.Data;

namespace TMB.Challenge.Infrastructure.Repository;

public class OrderRepository(ApplicationDbContext context) : IOrderRepository
{
    private readonly ApplicationDbContext _context = context;

    public async Task<Order> AddAsync(Order order)
    {

        var initialHistory = new OrderStatusHistory
        {
            Order = order,
            NewStatus = OrderStatus.Pendente,
            ChangedAt = DateTime.UtcNow
        };

        await _context.Orders.AddAsync(order);
        await _context.OrderStatusHistories.AddAsync(initialHistory);
        await _context.SaveChangesAsync();
        return order;
    }

    public async Task<Order?> GetByUuidAsync(string uuid)
    {
        return await _context.Orders.Include(o => o.StatusHistories).FirstOrDefaultAsync(o => o.Uuid == uuid);
    }

    public async Task<PaginatedResult<Order>> GetAllAsync(int pageNumber, int pageSize, OrderStatus? status = null)
    {
        if (pageNumber < 1) pageNumber = 1;
        if (pageSize <= 0) pageSize = 10;

        var query = _context.Orders
                            .AsNoTracking()
                            .AsQueryable();

        if (status.HasValue)
        {
            query = query.Where(o => o.Status == status.Value);
        }

        var totalCount = await query.CountAsync();

        var items = await query
        .OrderByDescending(o => o.DataCriacao)
        .Skip((pageNumber - 1) * pageSize)
        .Take(pageSize)
        .ToListAsync();


        return new PaginatedResult<Order>(items, totalCount, pageNumber, pageSize);

    }


    public async Task<Order> UpdateAsync(Order order)
    {
        _context.Orders.Update(order);
        await _context.SaveChangesAsync();
        return order;
    }
}