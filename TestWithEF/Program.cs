using System.Reflection;
using System.Threading.Channels;
using BuildingBlocks.Swagger;
using EntityFramework.Exceptions.SqlServer;
using FluentValidation;
using Microsoft.EntityFrameworkCore;
using Serilog;
using TestWithEF;
using TestWithEF.Channels;
using TestWithEF.EndPoints;
using TestWithEF.IRepositories.Base;
using TestWithEF.Services;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddCors();
builder.Host.UseSerilog((context, configuration) => configuration.ReadFrom.Configuration(context.Configuration));
// Use to force loading of appsettings.json of test project
builder.Configuration.AddJsonFile("appsettings.test.json",true);
var configuration = builder.Configuration;
// Add services to the container.
bool useOnlyInMemoryDatabase = false;
if (configuration["UseOnlyInMemoryDatabase"] != null)
{
    useOnlyInMemoryDatabase = bool.Parse(configuration["UseOnlyInMemoryDatabase"]!);
}

if (useOnlyInMemoryDatabase)
{
    builder.Services.AddDbContext<TestContext>(c =>
    {
        c.UseInMemoryDatabase("TestWithEF");
        c.UseExceptionProcessor();
    });
         
    
}
else
{
    builder.Services.AddDbContext<TestContext>(options =>
    {
        options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection"));
        options.UseExceptionProcessor();
    });
}


builder.Services.AddHttpClient("CountriesClient", config =>
{
    config.BaseAddress = new Uri("https://api.first.org/data/v1/");
    config.Timeout = new TimeSpan(0, 0, 30);
    config.DefaultRequestHeaders.Clear();
});

builder.Services.AddHttpClient();
//builder.Services.AddHostedService<NotificationDispatcher>();
builder.Services.AddHostedService<SendEmailDispatcher>();
builder.Services.AddHostedService<UserUpdatedDispatcher>();
//builder.Services.AddSingleton(Channel.CreateUnbounded<string>());
builder.Services.AddSingleton(Channel.CreateUnbounded<UserUpdatedChannel>());
builder.Services.AddSingleton(Channel.CreateUnbounded<SendEmailChannel>());
builder.Services.AddScoped<IChannelService, ChannelService>();

builder.Services.Scan(scan => scan
    .FromCallingAssembly()
    .AddClasses(classes => classes.AssignableTo(typeof(IRepository<,>)))
    .AsImplementedInterfaces()
    .WithScopedLifetime());

builder.Services.AddControllers();
builder.Services.AddValidatorsFromAssembly(Assembly.GetExecutingAssembly());
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddCustomSwagger(builder.Configuration, typeof(TestWithEFRoot).Assembly);
builder.Services.AddCustomVersioning();
builder.AddMinimalEndpoints(assemblies: typeof(TestWithEFRoot).Assembly);

var app = builder.Build();
app.UseSerilogRequestLogging();
app.MapMinimalEndpoints();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseCors(builder =>
    builder.AllowAnyOrigin()
        .AllowAnyHeader()
        .AllowAnyMethod());

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
public partial class Program { }
