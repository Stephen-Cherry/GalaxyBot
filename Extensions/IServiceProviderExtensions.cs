namespace GalaxyBot.Extensions;

public static class IServiceProviderExtensions
{
    public static async Task StartApplicationServices(this IServiceProvider serviceProvider)
    {
        LoggingService loggingService = serviceProvider.GetRequiredService<LoggingService>();
        await loggingService.LogToDiscord(
            LogLevel.Info,
            LogType.General,
            "Starting application services"
        );

        await InitializeInteractionService(serviceProvider);
        InitializeBuffReminderService(serviceProvider);
        InitializeDatabaseService(serviceProvider);

        await loggingService.LogToDiscord(
            LogLevel.Info,
            LogType.General,
            "Application services started successfully"
        );
    }

    private static async Task InitializeInteractionService(IServiceProvider serviceProvider)
    {
        InteractionService interactionService =
            serviceProvider.GetRequiredService<InteractionService>();

        _ = await interactionService.AddModulesAsync(Assembly.GetEntryAssembly(), serviceProvider);
        _ = await interactionService.RegisterCommandsGloballyAsync();

        DiscordSocketClient client = serviceProvider.GetRequiredService<DiscordSocketClient>();
        client.InteractionCreated += interaction =>
            HandleInteraction(interaction, serviceProvider, interactionService, client);
    }

    private static async Task HandleInteraction(
        SocketInteraction interaction,
        IServiceProvider serviceProvider,
        InteractionService interactionService,
        DiscordSocketClient client
    )
    {
        try
        {
            SocketInteractionContext socketInteractionContext = new(client, interaction);
            await interactionService.ExecuteCommandAsync(socketInteractionContext, serviceProvider);

            if (interaction is SocketSlashCommand slashCommand)
            {
                LoggingService loggingService =
                    serviceProvider.GetRequiredService<LoggingService>();
                await loggingService.LogSlashCommand(slashCommand);
            }
        }
        catch (Exception ex)
        {
            LoggingService loggingService = serviceProvider.GetRequiredService<LoggingService>();
            await loggingService.LogToDiscord(LogLevel.Error, LogType.Command, ex.ToString());
        }
    }

    private static void InitializeBuffReminderService(IServiceProvider serviceProvider)
    {
        BuffReminderService buffReminderService =
            serviceProvider.GetRequiredService<BuffReminderService>();
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
