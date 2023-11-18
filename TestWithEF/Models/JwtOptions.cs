using Microsoft.Extensions.Options;

namespace TestWithEF.Models;

public class JwtOptions
{
    public string ValidAudience { get; set; } = null!;

    public string ValidIssuer { get; set; } = null!;

    public string Secret { get; set; } = null!;
}

public class JwtOptionsSetup : IConfigureOptions<JwtOptions>
{
    private const string Section = "JWT";
    private readonly IConfiguration _configuration;

    public JwtOptionsSetup(IConfiguration configuration)
    {
        _configuration = configuration;
    }

    public void Configure(JwtOptions options)
    {
        _configuration.GetSection(Section).Bind(options);
    }
}
