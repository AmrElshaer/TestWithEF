using System.Data.Common;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;

namespace IntegrationTest.DatabasesTestingProvider;

public class InMemoryTestDatabase : ITestDatabase
{
    private readonly string _connectionString;

    public InMemoryTestDatabase()
    {
        _connectionString = "InMemoryTestWithEF";
    }

    public async Task InitialiseAsync()
    {
        var options = new DbContextOptionsBuilder<ApplicationContext>()
            .UseInMemoryDatabase(_connectionString)
            .Options;

        var context = new ApplicationContext(options);

        await context.Database.EnsureCreatedAsync();
    }

    public DbConnection GetConnection()
    {
        return new SqlConnection();
    }

    public async Task ResetAsync()
    {
        await InitialiseAsync();
    }

    public async Task DisposeAsync()
    {
        await Task.CompletedTask;
    }
}
