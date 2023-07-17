using Discord;
using Discord.Interactions;
using Discord.WebSocket;
using GalaxyBot.Data;
using GalaxyBot.Handlers;
using GalaxyBot.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace GalaxyBot.Extensions;

public static class HostBuilderExtensions
{
    public static IHostBuilder SetAppConfiguration(this IHostBuilder hostBuilder)
    {
        return hostBuilder.ConfigureAppConfiguration((hostingContext, configuration) =>
        {
            if (hostingContext.HostingEnvironment.IsDevelopment())
            {
                configuration.AddUserSecrets<Program>();
            }
            else
            {
                configuration.AddEnvironmentVariables();
            }
        });
    }

    public static IHostBuilder SetAppServices(this IHostBuilder hostBuilder)
    {
        return hostBuilder.ConfigureServices((_, services) =>
        {
            services

            .AddDbContextFactory<GalaxyBotContext>()
            .AddSingleton(new DiscordSocketConfig()
            {
                GatewayIntents = GatewayIntents.GuildMembers | GatewayIntents.AllUnprivileged | GatewayIntents.MessageContent
            })
            .AddSingleton<DiscordSocketClient>()
            .AddSingleton(new InteractionServiceConfig())
            .AddSingleton<InteractionService>()
            .AddSingleton<LoggingService>()
            .AddSingleton<BuffReminderService>()
            .AddSingleton<InteractionHandler>();
        });
    }
}