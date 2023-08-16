namespace GalaxyBot.Extensions;

public static class IServiceProviderExtensions
{
    public static async Task StartApplicationServices(this IServiceProvider serviceProvider)
    {
        LoggingService loggingService = serviceProvider.GetRequiredService<LoggingService>();

        await loggingService.LogToDiscord(LogSeverity.Info, LogType.General, "Starting application services");

        await InitializeInteractionService(serviceProvider);
        InitializeBuffReminderService(serviceProvider);
        InitializeDatabaseService(serviceProvider);

        await loggingService.LogToDiscord(LogSeverity.Info, LogType.General, "Application services started successfully");
    }

    private static async Task InitializeInteractionService(IServiceProvider serviceProvider)
    {
        InteractionService interactionService = serviceProvider.GetRequiredService<InteractionService>();

        _ = await interactionService.AddModulesAsync(Assembly.GetEntryAssembly(), serviceProvider);
        _ = await interactionService.RegisterCommandsGloballyAsync();

        DiscordSocketClient client = serviceProvider.GetRequiredService<DiscordSocketClient>();
        client.InteractionCreated += async (interaction) =>
        {
            try
            {
                SocketInteractionContext socketInteractionContext = new(client, interaction);
                _ = await interactionService.ExecuteCommandAsync(socketInteractionContext, serviceProvider);
            }
            catch (Exception ex)
            {
                LoggingService loggingService = serviceProvider.GetRequiredService<LoggingService>();
                await loggingService.LogToDiscord(LogSeverity.Error, LogType.Command, ex.ToString());
            }
        };
    }

    private static void InitializeBuffReminderService(IServiceProvider serviceProvider)
    {
        BuffReminderService buffReminderService = serviceProvider.GetRequiredService<BuffReminderService>();
        buffReminderService.StartService();
    }

    private static void InitializeDatabaseService(IServiceProvider serviceProvider)
    {
        GalaxyBotContext context = serviceProvider
            .GetRequiredService<IDbContextFactory<GalaxyBotContext>>()
            .CreateDbContext();
        Console.WriteLine($"Data saving to {context.DbPath}");
    }
}
