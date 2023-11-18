using System.Threading.Channels;
using Microsoft.EntityFrameworkCore;
using TestWithEF.ValueObjects;

namespace TestWithEF.Services
{
    public class NotificationDispatcher : BackgroundService
    {
        private readonly Channel<string> channel;
        private readonly ILogger<NotificationDispatcher> logger;
        private readonly IHttpClientFactory httpClientFactory;
        private readonly IServiceProvider provider;

        public NotificationDispatcher
        (
            Channel<string> channel,
            ILogger<NotificationDispatcher> logger,
            IHttpClientFactory httpClientFactory,
            IServiceProvider provider
        )
        {
            this.channel = channel;
            this.logger = logger;
            this.httpClientFactory = httpClientFactory;
            this.provider = provider;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!channel.Reader.Completion.IsCompleted) // if not complete
            {
                // read from channel
                var msg = await channel.Reader.ReadAsync();

                try
                {
                    using (var scope = provider.CreateScope())
                    {
                        var database = scope.ServiceProvider.GetRequiredService<TestContext>();
                        var user = await database.Authors.FirstOrDefaultAsync();
                        var client = httpClientFactory.CreateClient();
                        var response = await client.GetStringAsync("https://docs.microsoft.com/en-us/dotnet/core/");
                        var authorNameRes = AuthorName.CreateAuthorName(response);

                        if (authorNameRes.IsFailure)
                            throw new ArgumentException(authorNameRes.Error);

                        user = user.UpdateName(authorNameRes.Value);
                        await database.SaveChangesAsync();
                        logger.LogInformation("Complete");
                    }
                }
                catch (Exception e)
                {
                    logger.LogError(e, "notification failed");
                }
            }
        }
    }
}
