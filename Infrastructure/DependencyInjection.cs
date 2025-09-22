using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using TMB.Challenge.Infrastructure.Data;
using TMB.Challenge.Infrastructure.Config;
using Microsoft.AspNetCore.Hosting;
using TMB.Challenge.Application.Interfaces;
using TMB.Challenge.Infrastructure.Repository;
using TMB.Challenge.Infrastructure.Services;
using Microsoft.Extensions.Hosting;

namespace TMB.Challenge.Infrastructure;


public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration, IHostEnvironment environment)
    {
        services.AddSingleton<IMessageConsumerService, RabbitMqConsumerService>();
        services.AddSingleton<IMessageBusService, RabbitMqBusService>();
        services.AddScoped<IOrderRepository, OrderRepository>();
        services.AddScoped<IOrderStatusHistoryRepository, OrderStatusHistoryRepository>();
        services.AddScoped<IOutboxMessage, OutboxMessageRepository>();
        return services;
    }
}