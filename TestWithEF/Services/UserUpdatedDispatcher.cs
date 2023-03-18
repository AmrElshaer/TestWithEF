using System.Threading.Channels;
using TestWithEF.Channels;

namespace TestWithEF.Services
{
    public class UserUpdatedDispatcher : BackgroundService
    {
        private readonly Channel<UserUpdatedChannel> channel;
        private readonly ILogger<UserUpdatedChannel> logger;

        public UserUpdatedDispatcher(Channel<UserUpdatedChannel> channel, ILogger<UserUpdatedChannel> logger)
        {
            this.channel = channel;
            this.logger = logger;
        }
        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!channel.Reader.Completion.IsCompleted) // if not complete
            {
                // read from channel
                var msg = await channel.Reader.ReadAsync();
                if (msg != null)
                {
                    await Task.Delay(2000);
                    logger.LogInformation($"User that name is {msg.Name} is updated");
                }

            }
        }
    }
}
