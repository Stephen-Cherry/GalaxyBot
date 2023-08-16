namespace GalaxyBot.Services;

public class LoggingService
{
    private readonly IDbContextFactory<GalaxyBotContext> _dbContextFactory;
    private readonly DiscordSocketClient _client;

    public LoggingService(
        DiscordSocketClient client,
        InteractionService interactionService,
        IDbContextFactory<GalaxyBotContext> dbContextFactory
    )
    {
        _client = client;
        _dbContextFactory = dbContextFactory;

        client.Log += LogAsync;
        interactionService.Log += LogAsync;
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

        if (message.Exception is CommandException)
        {
            discordLog.Type = LogType.Command;
        }
        else
        {
            discordLog.Type = LogType.General;
        }

        await dbContext.DiscordLogs.AddAsync(discordLog);
        await dbContext.SaveChangesAsync();
        await LogToDiscord(discordLog.Severity, discordLog.Type, discordLog.Message);
    }

    public async Task LogToDiscord(Embed embed)
    {
        SocketTextChannel logChannel = ClientResourceRetrieverService.GetTextChannel(
            _client,
            Constants.LOG_CHANNEL_ID
        );
        await logChannel.SendMessageAsync(embed: embed);
    }

    public async Task LogToDiscord(LogSeverity logSeverity, LogType logType, string message)
    {
        Color embedColor = logSeverity switch
        {
            LogSeverity.Critical => Color.DarkRed,
            LogSeverity.Error => Color.Red,
            LogSeverity.Warning => Color.DarkOrange,
            LogSeverity.Info => Color.LightGrey,
            _ => Color.Default
        };

        EmbedBuilder embedBuilder = new();
        embedBuilder.WithTitle(
            $"[{logSeverity.ToString().ToUpper()}]/[{logType.ToString().ToUpper()}]"
        );
        embedBuilder.WithColor(embedColor);
        embedBuilder.WithDescription(message);
        embedBuilder.WithCurrentTimestamp();
        await LogToDiscord(embedBuilder.Build());
    }
}
