using Domain.Users;
using Infrastructure.Persistence;
using Infrastructure.Users;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace Infrastructure;

public static class DependencyInjection
{
    private const string DatabaseConnectionName = "appdb";

    public static IHostApplicationBuilder AddInfrastructure(this IHostApplicationBuilder builder)
    {
        builder.AddNpgsqlDbContext<AppDbContext>(DatabaseConnectionName);

        builder.Services.AddScoped<IUserDbAction, UserDbAction>();

        return builder;
    }
}