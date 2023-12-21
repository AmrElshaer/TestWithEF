using IntegrationTest.DatabasesTestingProvider;
using MediatR;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using TestWithEF.Identity;
using TestWithEF.ValueObjects;
using TestApplicationDbContext= TestWithEF.TestDbContext;
namespace IntegrationTest;

[SetUpFixture]
public partial class Testing
{
    private static ITestDatabase _database = null!;
    private static CustomWebApplicationFactory _factory = null!;
    private static IServiceScopeFactory _scopeFactory = null!;
    private static Guid? _userId;

    [OneTimeSetUp]
    public async Task RunBeforeAnyTests()
    {
        _database = await TestDatabaseFactory.CreateAsync();
        _factory = new CustomWebApplicationFactory(_database.GetConnection());
        _scopeFactory = _factory.Services.GetRequiredService<IServiceScopeFactory>();
    }

    public static async Task<TResponse> SendAsync<TResponse>(IRequest<TResponse> request)
    {
        using var scope = _scopeFactory.CreateScope();

        var mediator = scope.ServiceProvider.GetRequiredService<ISender>();

        return await mediator.Send(request);
    }

    public static async Task SendAsync(IBaseRequest request)
    {
        using var scope = _scopeFactory.CreateScope();

        var mediator = scope.ServiceProvider.GetRequiredService<ISender>();

        await mediator.Send(request);
    }

    public static Guid? GetUserId()
    {
        return _userId;
    }

    private static async Task<Guid> AddUser(string userName, string password, string[] roles)
    {
        using var scope = _scopeFactory.CreateScope();

        var userManager = scope.ServiceProvider.GetRequiredService<UserManager<ApplicationUser>>();
        var emailResult = Email.CreateEmail(userName);

        if (emailResult.Failure)
        {
            throw emailResult.Error;
        }

        var userResult = ApplicationUser.Create(emailResult.Value);

        if (userResult.Failure)
        {
            throw userResult.Error;
        }

        var result = await userManager.CreateAsync(userResult.Value, password);

        if (roles.Any())
        {
            var roleManager = scope.ServiceProvider.GetRequiredService<RoleManager<IdentityRole>>();

            foreach (var role in roles)
            {
                await roleManager.CreateAsync(new IdentityRole(role));
            }

            await userManager.AddToRolesAsync(userResult.Value, roles);
        }

        if (!result.Succeeded)
            throw new Exception($"Unable to create {userName}");

        _userId = userResult.Value.Id;

        return _userId.Value;
    }

    // before any test method
    public static async Task ResetState()
    {
        try
        {
            await _database.ResetAsync();
        }
        catch (Exception) { }

        _userId = null;
    }

    public static async Task<TEntity?> FindAsync<TEntity>(params object[] keyValues)
        where TEntity : class
    {
        using var scope = _scopeFactory.CreateScope();

        var context = scope.ServiceProvider.GetRequiredService<TestApplicationDbContext>();

        return await context.FindAsync<TEntity>(keyValues);
    }

    public static async Task AddAsync<TEntity>(TEntity entity)
        where TEntity : class
    {
        using var scope = _scopeFactory.CreateScope();

        var context = scope.ServiceProvider.GetRequiredService<TestApplicationDbContext>();

        context.Add(entity);

        await context.SaveChangesAsync();
    }
    public static async Task AddRangeAsync<TEntity>(List<TEntity> entity)
        where TEntity : class
    {
        using var scope = _scopeFactory.CreateScope();

        var context = scope.ServiceProvider.GetRequiredService<TestApplicationDbContext>();

        context.AddRange(entity);

        await context.SaveChangesAsync();
    }
    public static async Task<int> CountAsync<TEntity>() where TEntity : class
    {
        using var scope = _scopeFactory.CreateScope();

        var context = scope.ServiceProvider.GetRequiredService<TestApplicationDbContext>();

        return await context.Set<TEntity>().CountAsync();
    }

    [OneTimeTearDown]
    public async Task RunAfterAnyTests()
    {
        await _database.DisposeAsync();
        await _factory.DisposeAsync();
    }
}
