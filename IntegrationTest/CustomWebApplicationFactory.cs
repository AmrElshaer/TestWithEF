using System.Data.Common;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using TestWithEF;

namespace IntegrationTest;

// <snippet1>
public class CustomWebApplicationFactory<TProgram>
    : WebApplicationFactory<TProgram> where TProgram : class
{
    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        builder.ConfigureServices(services =>
        {
            var dbContextDescriptor = services.SingleOrDefault(
                d => d.ServiceType ==
                    typeof(DbContextOptions<TestContext>));

            if (dbContextDescriptor != null)
                services.Remove(dbContextDescriptor);

            var dbConnectionDescriptor = services.SingleOrDefault(
                d => d.ServiceType ==
                    typeof(DbConnection));

            if (dbConnectionDescriptor != null)
                services.Remove(dbConnectionDescriptor);

            // Create open SqliteConnection so EF won't automatically close it.
            // services.AddSingleton<DbConnection>(container =>
            // {
            //     var connection = new SqliteConnection("DataSource=:memory:");
            //     connection.Open();
            //
            //     return connection;
            // });

            services.AddDbContext<TestContext>((container, options) =>
            {
                // var connection = container.GetRequiredService<DbConnection>();
                // options.UseSqlite(connection);
                options.UseInMemoryDatabase("InMemoryEmployeeTest");
            });

            var sp = services.BuildServiceProvider();

            using var scope = sp.CreateScope();

            using var appContext = scope.ServiceProvider.GetRequiredService<TestContext>();

            appContext.Database.EnsureCreated();
        });

        builder.UseEnvironment("Development");
    }
}
// </snippet1>
