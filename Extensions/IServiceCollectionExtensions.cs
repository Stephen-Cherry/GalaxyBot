namespace GalaxyBot.Extensions;

public static class IServiceCollectionExtensions
{
    public static void AddApplicationServices(this IServiceCollection services)
    {
        services.AddDbContextFactory<GalaxyBotContext>();
        services.AddSingleton(GetDiscordSocketConfig());
        services.AddSingleton<DiscordSocketClient>();
        services.AddSingleton(new InteractionServiceConfig());
        services.AddSingleton<InteractionService>();
        services.AddSingleton<LoggingService>();
        services.AddSingleton<BuffReminderService>();
    }

    private static DiscordSocketConfig GetDiscordSocketConfig()
    {
        return new DiscordSocketConfig()
        {
            GatewayIntents = GatewayIntents.GuildMembers | GatewayIntents.AllUnprivileged | GatewayIntents.MessageContent
        };
    }
}
