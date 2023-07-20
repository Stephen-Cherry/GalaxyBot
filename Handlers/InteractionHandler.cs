using System.Reflection;
using Discord.Interactions;
using Discord.WebSocket;
using GalaxyBot.Services;

namespace GalaxyBot.Handlers;

public class InteractionHandler
{
    private readonly DiscordSocketClient _client;
    private readonly IServiceProvider _serviceProvider;
    private readonly InteractionService _interactionService;
    private readonly LoggingService _loggingService;

    public InteractionHandler(DiscordSocketClient client,
                              InteractionService interactionService,
                              IServiceProvider serviceProvider,
                              LoggingService loggingService)

    {
        _client = client;
        _interactionService = interactionService;
        _serviceProvider = serviceProvider;
        _client.InteractionCreated += HandleInteraction;
        _loggingService = loggingService;
    }

    public async Task InitializeAsync()
    {
        await _interactionService.AddModulesAsync(Assembly.GetEntryAssembly(), _serviceProvider);
        await _interactionService.RegisterCommandsGloballyAsync();
    }

    private async Task HandleInteraction(SocketInteraction interaction)
    {
        try
        {
            var ctx = new SocketInteractionContext(_client, interaction);
            await _interactionService.ExecuteCommandAsync(ctx, _serviceProvider);

            if (interaction is SocketSlashCommand slashCommand)
            {
                await _loggingService.LogSlashCommand(slashCommand);
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex.ToString());
        }
    }

}