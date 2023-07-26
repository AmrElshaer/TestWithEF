using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System.Net.Http;
using System.Threading.Channels;
using TestWithEF.Channels;
using TestWithEF.Entities;

namespace TestWithEF.Services
{
    public class SendEmailDispatcher : BackgroundService
    {
        private readonly Channel<SendEmailChannel> channel;
        private readonly ILogger<SendEmailChannel> logger;

        public SendEmailDispatcher(Channel<SendEmailChannel> channel, ILogger<SendEmailChannel> logger)
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
                    logger.LogInformation("Send Email to {MsgEmail} that name is {MsgName}", msg.Email, msg.Name);
                }
              
            }
        }
    }
}
