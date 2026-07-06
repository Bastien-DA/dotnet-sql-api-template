using Infrastructure.Persistence;
using MigrationService;

HostApplicationBuilder builder = Host.CreateApplicationBuilder(args);

builder.AddNpgsqlDbContext<AppDbContext>(connectionName: "appdb");

builder.Services.AddHostedService<Worker>();

builder.Services.AddOpenTelemetry()
    .WithTracing(tracing => tracing.AddSource(Worker.ActivitySourceName));

IHost host = builder.Build();
host.Run();