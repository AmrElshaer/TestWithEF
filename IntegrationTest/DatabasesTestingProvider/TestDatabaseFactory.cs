namespace IntegrationTest.DatabasesTestingProvider;

using static TestingConfiguration;

public class TestDatabaseFactory
{
    public static async Task<ITestDatabase> CreateAsync()
    {
        ITestDatabase database = DatabaseTypeValue switch
        {
            DatabaseType.Sqlite => new SqliteTestDatabase(),
            DatabaseType.SqlServer => new SqlServerTestDatabase(),
            DatabaseType.InMemory => new InMemoryTestDatabase(),
            _ => throw new ArgumentOutOfRangeException(nameof(DatabaseTypeValue), DatabaseTypeValue, null)
        };

        await database.InitialiseAsync();

        return database;
    }
}
