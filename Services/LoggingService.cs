namespace GalaxyBot.Services;

public class LoggingService
{
    private readonly IDbContextFactory<GalaxyBotContext> _dbContextFactory;

    public LoggingService(
        DiscordSocketClient client,
        InteractionService interactionService,
        IDbContextFactory<GalaxyBotContext> dbContextFactory
    )
    {
        client.Log += LogAsync;
        interactionService.Log += LogAsync;
        _dbContextFactory = dbContextFactory;
    }

    private async Task LogAsync(LogMessage message)
    {
        using GalaxyBotContext dbContext = _dbContextFactory.CreateDbContext();

        DiscordLog discordLog =
            new()
            {
                Severity = message.Severity,
                Message = message.Message,
                CreatedAt = DateTimeOffset.UtcNow
            };

        if (message.Exception is CommandException commandException)
            discordLog.Type = LogType.Command;
        else
            discordLog.Type = LogType.General;

        await dbContext.DiscordLogs.AddAsync(discordLog);
        await dbContext.SaveChangesAsync();
    }

    public async Task LogSlashCommand(SocketSlashCommand slashCommand)
    {
        using GalaxyBotContext dbContext = _dbContextFactory.CreateDbContext();

        IQueryable<User> userQuery =
            from user in dbContext.Users
            where user.UserName == slashCommand.User.Username
            select user;

        User? interactionUser = userQuery.FirstOrDefault();

        if (interactionUser is null)
        {
            EntityEntry<User> userEntry = await dbContext.Users.AddAsync(
                new User() { UserName = slashCommand.User.Username }
            );
            interactionUser = userEntry.Entity;
        }

        await dbContext.CommandLogs.AddAsync(
            new CommandLog()
            {
                Name = slashCommand.CommandName,
                User = interactionUser,
                UsedAt = DateTime.UtcNow
            }
        );

        await dbContext.SaveChangesAsync();
    }
}
