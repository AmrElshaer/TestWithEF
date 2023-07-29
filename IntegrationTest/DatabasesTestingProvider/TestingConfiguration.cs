using Microsoft.Extensions.Configuration;

namespace IntegrationTest.DatabasesTestingProvider;

public class TestingConfiguration
{
    private static IConfigurationRoot Configuration => new ConfigurationBuilder()
        .AddJsonFile("appsettings.test.json")
        .AddEnvironmentVariables()
        .Build();

    public static string ConnectionString => Configuration.GetConnectionString("DefaultConnection") ?? string.Empty;

    public static DatabaseType DatabaseTypeValue
    {
        get
        {
            var databaseType = Configuration["DatabaseType"];

            return databaseType switch
            {
                "SqlServer" => DatabaseType.SqlServer,
                "Sqlite" => DatabaseType.Sqlite,
                "InMemory" => DatabaseType.InMemory,
                _ => throw new ArgumentOutOfRangeException(nameof(databaseType))
            };
        }
    }
}
