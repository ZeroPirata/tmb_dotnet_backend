using System.Text.Json;
using TMB.Challenge.Application.DTOs.Order;
using TMB.Challenge.Application.Interfaces;
using TMB.Challenge.Domain.Entities;
using TMB.Challenge.Infrastructure.Data;

namespace TMB.Challenge.Infrastructure.Repository;

public class OutboxMessageRepository(ApplicationDbContext context) : IOutboxMessage
{
    private readonly ApplicationDbContext _context = context;

    public async Task AddAsync(OrderMessageDTO message)
    {
        var outboxMessage = new OutboxMessage
        {
            EventType = "OrderCreated",
            Payload = JsonSerializer.Serialize(message)
        };
        await _context.OutboxMessages.AddAsync(outboxMessage);
        await _context.SaveChangesAsync();
    }
}

