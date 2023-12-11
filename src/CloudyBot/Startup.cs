using Azure.Identity;
using Azure.ResourceManager;
using CloudyBot.Services;
using Discord;
using Discord.Rest;
using Microsoft.Azure.Functions.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection;
using System.Net.Http;

[assembly: FunctionsStartup(typeof(CloudyBot.Startup))]

namespace CloudyBot
{
    public class Startup : FunctionsStartup
    {
        public override void Configure(IFunctionsHostBuilder builder)
        {
            var discordRestClient = new DiscordRestClient(new DiscordRestConfig()
            {
                APIOnRestInteractionCreation = true,
                DefaultRetryMode = RetryMode.AlwaysRetry,
#if DEBUG
                LogLevel = LogSeverity.Debug,
                UseInteractionSnowflakeDate = false
#endif
            });

            builder.Services.AddSingleton<ICommandService>((s) =>
            {
                return new DiscordService(discordRestClient, new HttpClient());
            });
        }
    }
}
