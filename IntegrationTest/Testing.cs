using System.Net.Http.Headers;
using System.Text;
using IntegrationTest.DatabasesTestingProvider;
using IntegrationTest.TestData;
using Newtonsoft.Json;
using TestWithEF.Controllers;
using TestWithEF.Extensions;
using TestWithEF.Models;

namespace IntegrationTest;

[SetUpFixture]
public partial class Testing
{
    private static ITestDatabase _database = null!;
    private static CustomWebApplicationFactory _factory = null!;

    public static HttpClient Client = default!;

    [OneTimeSetUp]
    public async Task RunBeforeAnyTests()
    {
        _database = await TestDatabaseFactory.CreateAsync();
        _factory = new CustomWebApplicationFactory(_database.GetConnection());
        Client = _factory.CreateClient();
        var loginResponse = await AddUser();
        Client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", loginResponse!.Token);
    }

    private static async Task<LoginResponse?> AddUser()
    {
        var registerUser = DataGenerator.RegisterUserRequest().Generate();
        var registerUserJsonContent = JsonConvert.SerializeObject(registerUser);
        var httpContent = new StringContent(registerUserJsonContent, Encoding.UTF8, "application/json");
        _ = await Client.PostAsync("/api/user/register", httpContent);

        var loginUser = JsonConvert.SerializeObject(new LoginModel
        {
            Username = registerUser.Username,
            Password = registerUser.Password
        });

        var loginContent = new StringContent(loginUser, Encoding.UTF8, "application/json");
        var token = await Client.PostAsync("/api/user/login", loginContent);
        var tokenString = await token.Content.ReadAsStringAsync();
        var loginResponse = tokenString.FromJson<LoginResponse>();

        return loginResponse;
    }

    // before any test method
    public static async Task ResetState()
    {
        try
        {
            await _database.ResetAsync();
        }
        catch (Exception) { }
    }

    [OneTimeTearDown]
    public async Task RunAfterAnyTests()
    {
        await _database.DisposeAsync();
        await _factory.DisposeAsync();
        Client.Dispose();
    }
}
