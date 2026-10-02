using Aspire.Hosting;
using Aspire.Hosting.ApplicationModel;
using Aspire.Hosting.Testing;

[assembly: AssemblyFixture(typeof(Api.Integration.Test.AspireFixture))]

namespace Api.Integration.Test;

/// <summary>
/// Démarre l'AppHost Aspire une seule fois pour tout l'assembly : un vrai PostgreSQL (conteneur),
/// le MigrationService puis l'API. Les tests parlent à l'API en HTTP.
/// </summary>
public sealed class AspireFixture : IAsyncLifetime
{
    private static readonly TimeSpan StartupTimeout = TimeSpan.FromMinutes(3);

    private DistributedApplication _app = null!;

    public HttpClient ApiClient { get; private set; } = null!;

    public async ValueTask InitializeAsync()
    {
        using var cts = CancellationTokenSource.CreateLinkedTokenSource(TestContext.Current.CancellationToken);
        cts.CancelAfter(StartupTimeout);
        CancellationToken ct = cts.Token;

        IDistributedApplicationTestingBuilder builder =
            await DistributedApplicationTestingBuilder.CreateAsync<Projects.AppHost>(ct);

        // L'AppHost monte un volume pour le dev : on le retire pour partir d'une base vide à chaque run.
        foreach (IResource resource in builder.Resources)
        {
            foreach (ContainerMountAnnotation mount in resource.Annotations
                         .OfType<ContainerMountAnnotation>()
                         .Where(m => m.Type == ContainerMountType.Volume)
                         .ToList())
            {
                resource.Annotations.Remove(mount);
            }
        }

        _app = await builder.BuildAsync(ct);
        await _app.StartAsync(ct);

        // Les migrations EF Core doivent s'être appliquées avant que l'API ne démarre.
        ResourceEvent migrations = await _app.ResourceNotifications.WaitForResourceAsync(
            "migrations",
            e => KnownResourceStates.TerminalStates.Contains(e.Snapshot.State?.Text),
            ct);
        Assert.Equal(0, migrations.Snapshot.ExitCode);

        await _app.ResourceNotifications.WaitForResourceHealthyAsync("api", ct);

        ApiClient = _app.CreateHttpClient("api");
    }

    public async ValueTask DisposeAsync()
    {
        ApiClient.Dispose();

        await _app.DisposeAsync();
    }
}
