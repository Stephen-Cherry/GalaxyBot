namespace GalaxyBot.Extensions;

public static class IServiceCollectionExtensions
{
    public static void AddApplicationServices(this IServiceCollection services)
    {
        services.AddSingleton(GetDiscordSocketConfig());
        services.AddSingleton<DiscordSocketClient>();
        services.AddSingleton(new InteractionServiceConfig());
        services.AddSingleton<InteractionService>();
        services.AddSingleton<LoggingService>();
        services.AddSingleton<BuffReminderService>();
    }

    private static DiscordSocketConfig GetDiscordSocketConfig() => new()
    {
        GatewayIntents = GatewayIntents.Guilds | GatewayIntents.GuildMessages | GatewayIntents.MessageContent
    };
}
