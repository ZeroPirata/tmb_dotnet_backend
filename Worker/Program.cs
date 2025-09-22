using Microsoft.EntityFrameworkCore;
using OpenTelemetry.Resources;
using OpenTelemetry.Trace;
using TMB.Challenge.Application;
using TMB.Challenge.Infrastructure;
using TMB.Challenge.Infrastructure.Config;
using TMB.Challenge.Infrastructure.Data;
using TMB.Challenge.Worker.Processors;

var builder = Host.CreateApplicationBuilder(args);

PostgresConnection pgConfig = new(builder.Configuration);
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseNpgsql(pgConfig.GetConnectionString()));

builder.Services.AddApplication();
builder.Services.AddInfrastructure(builder.Configuration, builder.Environment);
builder.Services.AddHostedService<OrderProcessingWorker>();
builder.Services.AddHostedService<OutboxProcessorWorker>();

builder.Services.AddOpenTelemetry()
    .ConfigureResource(resource => resource.AddService("TMB.Challenge.Worker"))
    .WithTracing(tracing => tracing
        .AddConsoleExporter());
var host = builder.Build();
host.Run();