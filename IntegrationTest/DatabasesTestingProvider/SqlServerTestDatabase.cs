using System.Data.Common;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Respawn;
using Respawn.Graph;
using static IntegrationTest.DatabasesTestingProvider.TestingConfiguration;

namespace IntegrationTest.DatabasesTestingProvider;

public class SqlServerTestDatabase : ITestDatabase
{
    private readonly SqlConnection _connection = null!;
    private Respawner _respawner = null!;

    public SqlServerTestDatabase()
    {
        _connection = new SqlConnection(ConnectionString);
    }

    public async Task InitialiseAsync()
    {
        var options = new DbContextOptionsBuilder<ApplicationContext>()
            .UseSqlServer(ConnectionString)
            .Options;

        var context = new ApplicationContext(options);

        context.Database.Migrate();

        _respawner = await Respawner.CreateAsync(ConnectionString, new RespawnerOptions
        {
            TablesToIgnore = new Table[]
            {
                "__EFMigrationsHistory"
            }
        });
    }

    public DbConnection GetConnection()
    {
        return _connection;
    }

    public async Task ResetAsync()
    {
        await _respawner.ResetAsync(ConnectionString);
    }

    public async Task DisposeAsync()
    {
        await _connection.DisposeAsync();
    }
}
