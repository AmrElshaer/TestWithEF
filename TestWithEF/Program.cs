using System.Reflection;
using System.Text;
using System.Threading.Channels;
using BuildingBlocks.Swagger;
using EntityFramework.Exceptions.SqlServer;
using FluentValidation;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Polly;
// using Rebus.Config;
using Rebus.Routing.TypeBased;
using TestWithEF;
using TestWithEF.Channels;
using TestWithEF.EndPoints;
using TestWithEF.Features.Orders.Commands.CreateOrder;
using TestWithEF.Identity;
using TestWithEF.IRepositories.Base;
using TestWithEF.Models;
using TestWithEF.Services;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddCors();

builder.Services.AddLogging(logging => logging.AddConsole());

//builder.Host.UseSerilog((context, configuration) => configuration.ReadFrom.Configuration(context.Configuration));

builder.Services.AddDbContext<TestDbContext>(options =>
{
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection"));

    options.LogTo(s => Console.WriteLine(s), LogLevel.Information)
        .EnableDetailedErrors()
        .EnableSensitiveDataLogging();

    options.UseExceptionProcessor();
});

// add mediatr
builder.Services.AddMediatR(cfg => cfg.RegisterServicesFromAssembly(typeof(Program).Assembly));

builder.Services.AddDbContext<ApplicationDbContext>(options =>
{
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection"));

    options.LogTo(s => Console.WriteLine(s), LogLevel.Information)
        .EnableDetailedErrors()
        .EnableSensitiveDataLogging();

    options.UseExceptionProcessor();
});

builder.Services.AddIdentity<ApplicationUser, ApplicationRole>()
    .AddEntityFrameworkStores<ApplicationDbContext>()
    .AddDefaultTokenProviders();

builder.Services.AddAuthentication(options =>
    {
        options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
        options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
        options.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
    })

// Adding Jwt Bearer
    .AddJwtBearer(options =>
    {
        options.SaveToken = true;
        options.RequireHttpsMetadata = false;

        options.TokenValidationParameters = new TokenValidationParameters()
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidAudience = builder.Configuration["JWT:ValidAudience"],
            ValidIssuer = builder.Configuration["JWT:ValidIssuer"],
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(builder.Configuration["JWT:Secret"]))
        };
    });

builder.Services.AddHttpClient("CountriesClient", config =>
{
    config.BaseAddress = new Uri("https://api.first.org/data/v1/");
    config.Timeout = new TimeSpan(0, 0, 30);
    config.DefaultRequestHeaders.Clear();
});

// mapster configuration
builder.Services.AddMapster();
builder.Services.AddHttpClient();
//builder.Services.AddHostedService<NotificationDispatcher>();
builder.Services.AddHostedService<SendEmailDispatcher>();
builder.Services.AddHostedService<UserUpdatedDispatcher>();
//builder.Services.AddSingleton(Channel.CreateUnbounded<string>());
builder.Services.AddSingleton(Channel.CreateUnbounded<UserUpdatedChannel>());
builder.Services.AddSingleton(Channel.CreateUnbounded<SendEmailChannel>());
builder.Services.AddScoped<IChannelService, ChannelService>();
builder.Services.ConfigureOptions<JwtOptionsSetup>();
builder.Services.AddTransient<IWeatherService, WeatherService>();

var basicCircuitBreakerPolicy = Policy
    .HandleResult<HttpResponseMessage>(r => !r.IsSuccessStatusCode)
    .CircuitBreakerAsync(2, TimeSpan.FromMilliseconds(120000));

// builder.Services.AddRebus(
//     rebus => rebus
//         .Routing(r =>
//             r.TypeBased().MapAssemblyOf<Program>("testwithef-queue"))
//         .Transport(t =>
//             t.UseRabbitMq("amqp://guest:guest@localhost:5672",
//                 inputQueueName: "testwithef-queue"))
//         .Sagas(s =>
//             s.StoreInSqlServer(
//                 builder.Configuration.GetConnectionString("DefaultConnection"),
//                 dataTableName: "Sagas",
//                 indexTableName: "SagaIndexes"))
                
    //         ),onCreated:
    // async bus =>
    // {
    //    // await bus.Subscribe<OrderCreatedEvent>();
    //    // await bus.Subscribe<OrderConfirmationEmailSent>(); if you using publish== cheogra ,send orchi
    //     //await bus.Subscribe<OrderPaymentRequestSent>();
    // }
    //  you can using timeout to send event after amount of time like send follow updat after three week
    // .Timeouts(t =>
    //     t.StoreInSqlServer(
    //         builder.Configuration.GetConnectionString("DefaultConnection"),
    //         tableName: "Timeouts"))
//);

//builder.Services.AutoRegisterHandlersFromAssemblyOf<Program>();

builder.Services.AddHttpClient<IWeatherService, WeatherService>(client =>
    {
        client.BaseAddress = new Uri("https://official-joke-api.appspot.com/random_joke");
    })
    .AddPolicyHandler(basicCircuitBreakerPolicy);

builder.Services.AddHttpClient("TemperatureService", client =>
{
    client.BaseAddress = new Uri("http://localhost:6001/");
    client.DefaultRequestHeaders.Add("Accept", "application/json");
}).AddPolicyHandler(basicCircuitBreakerPolicy);

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
//app.UseSerilogRequestLogging();
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
app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();

public partial class Program { }
