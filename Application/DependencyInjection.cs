using Microsoft.Extensions.DependencyInjection;
using TMB.Challenge.Application.Handler;

namespace TMB.Challenge.Application;


public static class DependencyInjection
{
    public static IServiceCollection AddApplication(this IServiceCollection services)
    {
        services.AddScoped<OrderHandler>();

        return services;
    }
}