
using System.Text.Json;
using Npgsql;
using Xunit;

public class IdentityDatabaseTests
{
    [Fact]
    public async Task Should_Connect_To_Test_Database()
    {
        var json = await File.ReadAllTextAsync(
            "appsettings.Test.json");

        using var document = JsonDocument.Parse(json);

        var connectionString = document.RootElement
            .GetProperty("ConnectionStrings")
            .GetProperty("IdentityTestConnection")
            .GetString();

        var builder = new NpgsqlConnectionStringBuilder(connectionString);

        Assert.Equal("BuildingHub_Test", builder.Database);

        await using var connection =
            new NpgsqlConnection(builder.ConnectionString);

        await connection.OpenAsync();

        await using var command =
            new NpgsqlCommand("SELECT current_database()", connection);

        var database = (string?)await command.ExecuteScalarAsync();

        Assert.Equal("BuildingHub_Test", database);
    }
}
