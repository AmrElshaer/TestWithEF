using System.Data.Common;
using IntegrationTest.DatabasesTestingProvider;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using ApplicationDbContext= TestWithEF.Identity.ApplicationDbContext;
using TestApplicationDbContext= TestWithEF.TestDbContext;
namespace IntegrationTest;

using static TestingConfiguration;

public class CustomWebApplicationFactory
    : WebApplicationFactory<Program>
{
    private readonly DbConnection _connection;

    public CustomWebApplicationFactory(DbConnection connection)
    {
        _connection = connection;
    }

    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        builder.ConfigureServices(services =>
        {
            services
                .RemoveAll<DbContextOptions<ApplicationDbContext>>().AddDbContext<ApplicationDbContext>((container, options) =>
                {
                    _ = DatabaseTypeValue switch
                    {
                        DatabaseType.Sqlite => options.UseSqlite(_connection),
                        DatabaseType.SqlServer => options.UseSqlServer(_connection),
                        DatabaseType.InMemory => options.UseInMemoryDatabase("InMemoryTestWithEF"),
                        _ => throw new ArgumentOutOfRangeException(nameof(DatabaseTypeValue))
                    };
                });
            services
                .RemoveAll<DbContextOptions<TestApplicationDbContext>>().AddDbContext<TestApplicationDbContext>((container, options) =>
                {
                    _ = DatabaseTypeValue switch
                    {
                        DatabaseType.Sqlite => options.UseSqlite(_connection),
                        DatabaseType.SqlServer => options.UseSqlServer(_connection),
                        DatabaseType.InMemory => options.UseInMemoryDatabase("InMemoryTestWithEF"),
                        _ => throw new ArgumentOutOfRangeException(nameof(DatabaseTypeValue))
                    };
                });
        });

        builder.UseEnvironment("Development");
    }
}
