using Microsoft.EntityFrameworkCore;
using OpenTelemetry.Resources;
using OpenTelemetry.Trace;
using Serilog;
using TMB.Challenge.API.Hubs;
using TMB.Challenge.API.Workers;
using TMB.Challenge.Application;
using TMB.Challenge.Infrastructure;
using TMB.Challenge.Infrastructure.Config;
using TMB.Challenge.Infrastructure.Data;
using Microsoft.AspNetCore.Diagnostics.HealthChecks;
using System.Text.Json;
using HealthChecks.UI.Client;

var builder = WebApplication.CreateBuilder(args);

// --- 1. CONFIGURAÇÃO DE LOGGING E TELEMETRIA ---
builder.Host.UseSerilog((context, config) =>
{
    config.ReadFrom.Configuration(context.Configuration)
          .WriteTo.Console()
          .WriteTo.File("logs/log.txt", rollingInterval: RollingInterval.Day);
});

builder.Services.AddOpenTelemetry()
    .ConfigureResource(resource => resource.AddService("TMB.Challenge.API"))
    .WithTracing(tracing => tracing
        .AddAspNetCoreInstrumentation()
        .AddConsoleExporter());

// --- 2. CONFIGURAÇÃO DOS SERVIÇOS ---
var pgConfig = new PostgresConnection(builder.Configuration);
var connectionString = pgConfig.GetConnectionString();

builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseNpgsql(connectionString));

builder.Services.AddApplication();
builder.Services.AddInfrastructure(builder.Configuration, builder.Environment);
builder.Services.AddHostedService<OrderStatusUpdateListener>();

var myAllowSpecificOrigins = "_myAllowSpecificOrigins";
builder.Services.AddCors(options =>
{
    options.AddPolicy(name: myAllowSpecificOrigins, policy =>
    {
        policy.WithOrigins("http://localhost:3000")
              .AllowAnyHeader()
              .AllowAnyMethod()
              .AllowCredentials();
    });
});

builder.Services.AddControllers();
builder.Services.AddSignalR();
builder.Services.AddOpenApi();
builder.Services.AddEndpointsApiExplorer();

// --- 3. CONFIGURAÇÃO DOS HEALTH CHECKS ---
// Construir a connection string do RabbitMQ
var rabbitmqHost = builder.Configuration["RABBITMQ_HOST"] ?? "localhost";
var rabbitmqUser = builder.Configuration["RABBITMQ_USER"] ?? "guest";
var rabbitmqPass = builder.Configuration["RABBITMQ_PASS"] ?? "guest";
var rabbitmqConnectionString = $"amqp://{rabbitmqUser}:{rabbitmqPass}@{rabbitmqHost}:5672/";

Console.WriteLine($"RabbitMQ Connection String: {rabbitmqConnectionString}");
Console.WriteLine($"PostgreSQL Connection String: {connectionString}");

builder.Services.AddHealthChecks()
    .AddNpgSql(connectionString, name: "PostgreSQL")
    .AddRabbitMQ(rabbitmqConnectionString, name: "RabbitMQ", failureStatus: Microsoft.Extensions.Diagnostics.HealthChecks.HealthStatus.Degraded)
    .AddCheck("API", () =>
    {
        // Self check simples - se chegou até aqui, a API está funcionando
        return Microsoft.Extensions.Diagnostics.HealthChecks.HealthCheckResult.Healthy("API is running");
    });

var app = builder.Build();

// --- 4. CONFIGURAÇÃO DO PIPELINE HTTP ---
app.UseSerilogRequestLogging();
app.UseCors(myAllowSpecificOrigins);

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.MapControllers();
app.MapHub<OrdersHub>("/hubs/orders");

// --- 5. HEALTH CHECKS ENDPOINT ---
app.MapHealthChecks("/health", new()
{
    ResponseWriter = UIResponseWriter.WriteHealthCheckUIResponse
});

// --- 6. MIGRAÇÕES AUTOMÁTICAS ---
try
{
    Console.WriteLine("Attempting to apply database migrations...");
    using (var scope = app.Services.CreateScope())
    {
        var dbContext = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
        dbContext.Database.Migrate();
    }
    Console.WriteLine("Database migrations applied successfully.");
}
catch (Exception ex)
{
    Log.Error(ex, "Failed to apply database migrations");
}

app.Run();