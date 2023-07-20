using Discord;
using Discord.Commands;
using Discord.Interactions;
using Discord.WebSocket;
using GalaxyBot.Data;
using Microsoft.EntityFrameworkCore;

namespace GalaxyBot.Services;
public class LoggingService
{
    private readonly IDbContextFactory<GalaxyBotContext> _dbContextFactory;

    public LoggingService(DiscordSocketClient client, InteractionService interactionService, IDbContextFactory<GalaxyBotContext> dbContextFactory)
    {
        client.Log += LogAsync;
        interactionService.Log += LogAsync;
        _dbContextFactory = dbContextFactory;
    }

    private Task LogAsync(LogMessage message)
    {
        if (message.Exception is CommandException commandException)
        {
            Console.WriteLine($"[Command/{message.Severity}] {commandException.Command.Aliases[0]}"
            + $" failed to execute in {commandException.Context.Channel}.");
        }
        else
        {
            Console.WriteLine($"[General]/{message.Severity}] {message}");
        }
        return Task.CompletedTask;
    }
    public async Task LogSlashCommand(SocketSlashCommand slashCommand)
    {
        GalaxyBotContext dbContext = _dbContextFactory.CreateDbContext();
        IQueryable<User> userQuery = from user in dbContext.Users
                                     where user.UserName == slashCommand.User.Username
                                     select user;
        User? interactionUser = userQuery.FirstOrDefault();

        if (interactionUser == null)
        {
            var task = await dbContext.Users.AddAsync(new User() { UserName = slashCommand.User.Username });
            interactionUser = task.Entity;
        }
        await dbContext.CommandLogs.AddAsync(new CommandLog()
        {
            Name = slashCommand.CommandName,
            User = interactionUser,
            UsedAt = DateTime.UtcNow
        });
        await dbContext.SaveChangesAsync();
    }
}