namespace GalaxyBot.Services;

public class LoggingService
{
    private readonly IDbContextFactory<GalaxyBotContext> _dbContextFactory;
    private readonly DiscordSocketClient _client;
    private readonly ulong _logChannelId;

    public LoggingService(DiscordSocketClient client,
                          IConfiguration configuration,
                          InteractionService interactionService,
                          IDbContextFactory<GalaxyBotContext> dbContextFactory)
    {
        _client = client;
        _dbContextFactory = dbContextFactory;
        _logChannelId = configuration.GetValue<ulong>(Constants.LOG_CHANNEL_ID);

        client.Log += LogAsync;
        interactionService.Log += LogAsync;
    }

    public async Task LogAsync(LogMessage logMessage)
    {
        BotLogMessage botLogMessage = new()
        {
            Message = logMessage.Message,
            Severity = logMessage.Severity,
            Source = logMessage.Source
        };

        await LogAsync(botLogMessage);
    }

    public async Task LogAsync(BotLogMessage logMessage)
    {
        BotLogMessage botLogMessage = new()
        {
            Message = logMessage.Message,
            Severity = logMessage.Severity,
            Source = logMessage.Source
        };

        await LogToDatabase(botLogMessage);
        await LogToDiscord(botLogMessage);
    }

    private async Task LogToDatabase(BotLogMessage logMessage)
    {
        using GalaxyBotContext dbContext = _dbContextFactory.CreateDbContext();

        await dbContext.LogMessages.AddAsync(logMessage);
        await dbContext.SaveChangesAsync();
    } 

    private async Task LogToDiscord(BotLogMessage logMessage)
    {
        Color embedColor = logMessage.Severity switch
        {
            LogSeverity.Critical => Color.DarkRed,
            LogSeverity.Error => Color.Red,
            LogSeverity.Warning => Color.DarkOrange,
            LogSeverity.Info => Color.LightGrey,
            _ => Color.Default
        };

        Embed embed = new EmbedBuilder()
            .WithTitle($"[{logMessage.Severity.ToString().ToUpper()}]")
            .WithColor(embedColor)
            .WithDescription(logMessage.Message)
            .WithCurrentTimestamp()
            .Build();

        SocketTextChannel logChannel = _client.GetTextChannel(_logChannelId);
        await logChannel.SendMessageAsync(embed: embed);
    }
}
