namespace GalaxyBot.Services;

public class LoggingService
{
    private readonly IDbContextFactory<GalaxyBotContext> _dbContextFactory;
    private readonly DiscordSocketClient _client;
    private readonly IConfiguration _configuration;
    private ulong LogChannelId { get; init; }

    public LoggingService(
        DiscordSocketClient client,
        InteractionService interactionService,
        IDbContextFactory<GalaxyBotContext> dbContextFactory,
        IConfiguration configuration
    )
    {
        _client = client;
        _dbContextFactory = dbContextFactory;
        _configuration = configuration;

        client.Log += LogAsync;
        interactionService.Log += LogAsync;

        string? channelId = _configuration.GetValue<string>(Constants.LOG_CHANNEL_ID);
        ArgumentException.ThrowIfNullOrEmpty(channelId, nameof(channelId));
        bool isValidChannelId = ulong.TryParse(channelId, out ulong logChannelId);
        if (!isValidChannelId) throw new ArgumentException($"{channelId} is not a valid ulong");
        LogChannelId = logChannelId;
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
        {
            discordLog.Type = LogType.Command;
        }
        else
        {
            discordLog.Type = LogType.General;
        }

        await dbContext.DiscordLogs.AddAsync(discordLog);
        await dbContext.SaveChangesAsync();
    }

    public async Task LogSlashCommand(SocketSlashCommand slashCommand)
    {
        using GalaxyBotContext dbContext = _dbContextFactory.CreateDbContext();

        User? interactionUser = dbContext.Users.FirstOrDefault(
            user => user.UserName == slashCommand.User.Username
        );

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

    public async Task LogToDiscord(Embed embed)
    {
        SocketTextChannel logChannel = ClientResourceRetrieverService.GetTextChannel(
            _client,
            LogChannelId
        );
        await logChannel.SendMessageAsync(embed: embed);
    }

    public async Task LogToDiscord(LogLevel logLevel, LogType logType, string message)
    {
        Color embedColor = logLevel switch
        {
            LogLevel.Error => Color.Red,
            LogLevel.Info => Color.LightGrey,
            _ => Color.Default
        };

        EmbedBuilder embedBuilder = new();
        embedBuilder.WithTitle(
            $"[{logLevel.ToString().ToUpper()}]/[{logType.ToString().ToUpper()}]"
        );
        embedBuilder.WithColor(embedColor);
        embedBuilder.WithDescription(message);
        embedBuilder.WithCurrentTimestamp();
        await LogToDiscord(embedBuilder.Build());
    }
}
