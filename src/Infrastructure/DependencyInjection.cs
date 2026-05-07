using Infrastructure.Persistence;
using Microsoft.Extensions.Hosting;

namespace Infrastructure;

public static class DependencyInjection
{
    private const string DatabaseConnectionName = "appdb";

    public static IHostApplicationBuilder AddInfrastructure(this IHostApplicationBuilder builder)
    {
        builder.AddNpgsqlDbContext<AppDbContext>(DatabaseConnectionName);
        return builder;
    }
}
