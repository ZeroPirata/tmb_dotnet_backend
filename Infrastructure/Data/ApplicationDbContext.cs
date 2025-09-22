using Microsoft.EntityFrameworkCore;
using TMB.Challenge.Domain.Entities;


namespace TMB.Challenge.Infrastructure.Data;

/// <summary>
/// Classe especializada para interagir com o banco de dados da aplicação, gerando o contexto necessário para as operações.
/// </summary>
public class ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : DbContext(options)
{
    public DbSet<Order> Orders { get; set; }
    public DbSet<OrderStatusHistory> OrderStatusHistories { get; set; }
    public DbSet<OutboxMessage> OutboxMessages { get; set; }
}