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
    /// <summary>
    /// Permite a configuração de serviços como repositórios, serviços de mensageria, etc.
    /// </summary>
    /// <param name="services">a coleção de serviços</param>
    /// <param name="configuration">configuração da aplicação</param>
    /// <param name="environment">o ambiente de hospedagem</param>
    /// <returns></returns>
    public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration, IHostEnvironment environment)
    {

        // Configuração para o serviço de mensageria RabbitMQ
        services.AddSingleton<IMessageConsumerService, RabbitMqConsumerService>();
        services.AddSingleton<IMessageBusService, RabbitMqBusService>();

        // Configuração do DbContext para utilização das funções referente a banco de dados
        services.AddScoped<IOrderRepository, OrderRepository>();
        services.AddScoped<IOrderStatusHistoryRepository, OrderStatusHistoryRepository>();
        services.AddScoped<IOutboxMessage, OutboxMessageRepository>();

        return services;
    }
}