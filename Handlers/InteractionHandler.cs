using System.Reflection;
using Discord.Interactions;
using Discord.WebSocket;
using GalaxyBot.Data;
using Microsoft.EntityFrameworkCore;

namespace GalaxyBot.Handlers;

public class InteractionHandler
{
    private readonly DiscordSocketClient _client;
    private readonly IServiceProvider _serviceProvider;
    private readonly InteractionService _interactionService;
    private readonly IDbContextFactory<GalaxyBotContext> _dbContextFactory;

    public InteractionHandler(DiscordSocketClient client, InteractionService interactionService, IServiceProvider serviceProvider, IDbContextFactory<GalaxyBotContext> dbContextFactory)
    {
        _client = client;
        _interactionService = interactionService;
        _serviceProvider = serviceProvider;
        _client.InteractionCreated += HandleInteraction;
        _dbContextFactory = dbContextFactory;
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
                var dbContext = _dbContextFactory.CreateDbContext();
                User? user = dbContext.Users.FirstOrDefault(user => user.UserName == interaction.User.Username);
                if (user == null)
                {
                    var task = await dbContext.Users.AddAsync(new User() { UserName = interaction.User.Username });
                    user = task.Entity;
                }
                await dbContext.CommandLogs.AddAsync(new CommandLog()
                {
                    Name = slashCommand.CommandName,
                    User = user,
                    UsedAt = DateTime.UtcNow
                });
                await dbContext.SaveChangesAsync();
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex.ToString());
        }
    }
}