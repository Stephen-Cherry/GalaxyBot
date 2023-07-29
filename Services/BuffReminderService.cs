namespace GalaxyBot.Services;

public class BuffReminderService
{
    private static bool hasUpdated = false;
    private readonly DiscordSocketClient _client;
    private readonly ulong _buffChannelId;
    private readonly IConfiguration _configuration;

    public BuffReminderService(DiscordSocketClient client, IConfiguration configuration)
    {
        _client = client;
        _configuration = configuration;
        bool successfulBuffChannelLookup = ulong.TryParse(
            _configuration.GetValue<string>(Constants.BUFF_CHANNEL_KEY),
            out _buffChannelId
        );
        if (!successfulBuffChannelLookup)
            throw new ArgumentException(Constants.BUFF_CHANNEL_KEY);

        CronExpression cronExpression = CronExpression.Parse("@midnight");
        TimeZoneInfo centralTimeZone = TimeZoneInfo.FindSystemTimeZoneById("Central Standard Time");

        TaskSchedulerService.ScheduleJob(
            cronExpression,
            centralTimeZone,
            async () =>
            {
                if (hasUpdated)
                {
                    hasUpdated = false;
                }
                else
                {
                    if (_client.GetChannel(_buffChannelId) is not SocketTextChannel buffChannel)
                    {
                        throw new Exception(
                            "Could not find a channel with the id provided" + _buffChannelId
                        );
                    }

                    await buffChannel.SendMessageAsync(
                        "@here, I have not seen the Buff Cat lately.  Please honor me with its presence if the buffs have been updated."
                    );
                }
            }
        );

        _client.MessageReceived += (SocketMessage userMessage) =>
        {
            bool hasBuffCat = userMessage.CleanContent.Contains(Constants.BUFF_CAT_EMOTE);
            bool isBot = userMessage.Author.IsBot;
            if (hasBuffCat && !isBot)
            {
                userMessage.Channel.SendMessageAsync("Praise be to the buff cat!");
                bool isUpdateHour = Constants.VALID_BUFF_UPDATE_HOURS_UTC.Contains(
                    DateTime.UtcNow.Hour
                );
                if (isUpdateHour)
                {
                    hasUpdated = true;
                }
            }
            return Task.CompletedTask;
        };
    }
}
