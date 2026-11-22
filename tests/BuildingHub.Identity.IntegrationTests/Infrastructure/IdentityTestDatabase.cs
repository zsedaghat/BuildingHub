
using System.Text.Json;
using BuildingHub.Modules.Identity.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
using Npgsql;

namespace BuildingHub.Identity.IntegrationTests.Infrastructure;

public static class IdentityTestDatabase
{
    public static DbContextOptions<IdentityDbContext> CreateOptions()
    {
        var path = Path.Combine(
            AppContext.BaseDirectory,
            "appsettings.Test.json");

        using var document = JsonDocument.Parse(
            File.ReadAllText(path));

        var connectionString = document.RootElement
            .GetProperty("ConnectionStrings")
            .GetProperty("IdentityTestConnection")
            .GetString()
            ?? throw new InvalidOperationException(
                "Identity test connection string is missing.");

        var builder = new NpgsqlConnectionStringBuilder(
            connectionString);

        if (!string.Equals(
                builder.Database,
                "BuildingHub_Test",
                StringComparison.OrdinalIgnoreCase))
        {
            throw new InvalidOperationException(
                "Tests must use BuildingHub_Test database.");
        }

        return new DbContextOptionsBuilder<IdentityDbContext>()
            .UseNpgsql(connectionString)
            .Options;
    }
}
