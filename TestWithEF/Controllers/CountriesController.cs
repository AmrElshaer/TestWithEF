using System.Net;
using Microsoft.AspNetCore.Mvc;
using Polly;

namespace TestWithEF.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class CountriesController : ControllerBase
    {
        private readonly ILogger<CountriesController> logger;

        public CountriesController(ILogger<CountriesController> logger)
        {
            this.logger = logger;
        }

        [HttpGet]
        public async Task<ActionResult> GetCountries()
        {
            var token = String.Empty;
            var timeout = 0;

            var unauthroizePolicy = Policy.Handle<HttpRequestException>
                    (r => (int) r.StatusCode == StatusCodes.Status401Unauthorized)
                .RetryAsync(async (e, n) =>
                    {
                        logger.LogWarning($"unauthroize request to get countries Message:{e.Message}");
                        logger.LogInformation("get refresh token");
                        await Task.Delay(1000);
                        token = Guid.NewGuid().ToString();
                    }
                );

            var timeoutPolicy = Policy
                .Handle<HttpRequestException>(ex => ex.StatusCode == HttpStatusCode.RequestTimeout)
                .WaitAndRetryAsync(new[]
                    {
                        TimeSpan.FromSeconds(1),
                        TimeSpan.FromSeconds(2),
                        TimeSpan.FromSeconds(3)
                    }, (h, y) => logger.LogWarning("timeoutPolicy")
                );

            var fallbackPolicy = Policy<string>.Handle<HttpRequestException>()
                .FallbackAsync(async (e) =>
                {
                    await Task.Delay(1000);
                    logger.LogWarning("fallbackPolicy can't solve the problem");

                    return "fallbackPolicy can't solve the problem";
                });

            var result = await fallbackPolicy
                .WrapAsync(timeoutPolicy)
                .WrapAsync(unauthroizePolicy)
                .ExecuteAsync(async () =>
                    {
                        logger.LogInformation("Execute the task");
                        await Task.Delay(1000);

                        if (string.IsNullOrEmpty(token))
                        {
                            throw new HttpRequestException("unauthorization user", null, HttpStatusCode.Unauthorized);
                        }

                        if (timeout < 1)
                        {
                            timeout++;

                            throw new HttpRequestException("request time out", null, HttpStatusCode.RequestTimeout);
                        }

                        return "Ok";
                    }
                );

            return Ok(result);
        }
    }
}
