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
using TestWithEF;
using TestWithEF.Channels;
using TestWithEF.EndPoints;
using TestWithEF.Identity;
using TestWithEF.IRepositories.Base;
using TestWithEF.Models;
using TestWithEF.Services;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddCors();
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
