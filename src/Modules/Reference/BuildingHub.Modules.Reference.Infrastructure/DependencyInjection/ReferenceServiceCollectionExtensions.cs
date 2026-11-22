using BuildingHub.Modules.Reference.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace BuildingHub.Modules.Reference.Infrastructure.DependencyInjection;

public static class ReferenceServiceCollectionExtensions
{
    public static IServiceCollection AddReferenceInfrastructure(
        this IServiceCollection services,
        string? connectionString)
    {
        if (string.IsNullOrWhiteSpace(connectionString))
        {
            throw new InvalidOperationException(
                "DefaultConnection connection string is not configured.");
        }

        services.AddDbContext<ReferenceDbContext>(options =>
            options.UseNpgsql(
                connectionString,
                npgsqlOptions => npgsqlOptions.MigrationsHistoryTable(
                    "__EFMigrationsHistory",
                    "reference")));

        return services;
    }
}
