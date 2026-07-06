using System.Diagnostics;
using Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace MigrationService;

// Applique les migrations EF Core au démarrage, puis arrête l'application.
// Orchestré par l'AppHost Aspire : l'API attend la fin de ce service (WaitForCompletion).
public class Worker(
    IServiceProvider serviceProvider,
    IHostApplicationLifetime hostApplicationLifetime) : BackgroundService
{
    public const string ActivitySourceName = "Migrations";
    private static readonly ActivitySource ActivitySource = new(ActivitySourceName);

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        using Activity? activity = ActivitySource.StartActivity("Migrating database", ActivityKind.Client);

        try
        {
            using IServiceScope scope = serviceProvider.CreateScope();
            var dbContext = scope.ServiceProvider.GetRequiredService<AppDbContext>();

            await RunMigrationAsync(dbContext, stoppingToken);
        }
        catch (Exception ex)
        {
            activity?.AddException(ex);
            throw;
        }

        hostApplicationLifetime.StopApplication();
    }

    private static async Task RunMigrationAsync(AppDbContext dbContext, CancellationToken cancellationToken)
    {
        // Utilise la stratégie d'exécution du provider (utile si EnableRetryOnFailure est activé).
        var strategy = dbContext.Database.CreateExecutionStrategy();
        await strategy.ExecuteAsync(() => dbContext.Database.MigrateAsync(cancellationToken));
    }
}

