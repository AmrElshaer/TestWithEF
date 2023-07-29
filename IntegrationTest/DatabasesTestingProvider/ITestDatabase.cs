using System.Data.Common;

namespace IntegrationTest.DatabasesTestingProvider;

public interface ITestDatabase
{
    Task InitialiseAsync();

    DbConnection GetConnection();

    Task ResetAsync();

    Task DisposeAsync();
}
