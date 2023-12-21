using System.Threading.Channels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TestWithEF.ValueObjects;

namespace TestWithEF.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class ChannellController : ControllerBase
    {
        private readonly ILogger<ChannellController> logger;
        private readonly IServiceProvider provider;
        private readonly IHttpClientFactory httpClientFactory;

        public ChannellController
        (
            IServiceProvider provider,
            IHttpClientFactory httpClientFactor,
            ILogger<ChannellController> logger
        )
        {
            this.httpClientFactory = httpClientFactor;
            this.provider = provider;
            this.logger = logger;
        }

        [HttpGet]
        public async Task<bool> SendC([FromServices] Channel<string> channel)
        {
            await channel.Writer.WriteAsync("Hello");

            return true;
        }

        [HttpGet]
        public IActionResult Get()
        {
            Task.Run(async () =>
            {
                using (var scope = provider.CreateScope())
                {
                    var database = scope.ServiceProvider.GetRequiredService<TestDbContext>();
                    var user = await database.Authors.FirstOrDefaultAsync();
                    var client = httpClientFactory.CreateClient();
                    var response = await client.GetStringAsync("https://docs.microsoft.com/en-us/dotnet/core/");
                    var authorNameRes = AuthorName.CreateAuthorName(response);

                    if (authorNameRes.Failure)
                        throw authorNameRes.Error;

                    user = user.UpdateName(authorNameRes.Value);
                    await database.SaveChangesAsync();
                    logger.LogInformation("Complete");
                }
            });

            return Ok();
        }
    }
}
