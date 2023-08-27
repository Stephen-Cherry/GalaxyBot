namespace GalaxyBot.Extensions;

public static class IServiceProviderExtensions
{
    private static bool _firstRun = true;
    public static async Task StartApplicationServices(this IServiceProvider serviceProvider)
    {
        if (!_firstRun) return;

        _firstRun = false;

        LoggingService loggingService = serviceProvider.GetRequiredService<LoggingService>();

        await loggingService.LogAsync(new BotLogMessage()
        {
            Severity = LogSeverity.Info,
            Message = "Starting application services"
        });

        await InitializeInteractionService(serviceProvider);
        InitializeBuffReminderService(serviceProvider);

        await loggingService.LogAsync(new BotLogMessage()
        {
            Severity = LogSeverity.Info,
            Message = "Application services started successfully"
        });
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
            catch (Exception exception)
            {
                LoggingService loggingService = serviceProvider.GetRequiredService<LoggingService>();

                await loggingService.LogAsync(new BotLogMessage()
                {
                    Source = exception.Source,
                    Severity = LogSeverity.Error,
                    Message = exception.ToString()
                });
            }
        };
    }

    private static void InitializeBuffReminderService(IServiceProvider serviceProvider) => serviceProvider
            .GetRequiredService<BuffReminderService>()
            .StartService();
}
