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

        string? channelId = _configuration.GetValue<string>(AppConstants.BUFF_CHANNEL_ID);
        bool isValidChannelId = ulong.TryParse(channelId, out _buffChannelId);
        if (!isValidChannelId)
        {
            throw new ArgumentException(AppConstants.BUFF_CHANNEL_ID);
        }
    }

    public void StartService()
    {
        CronExpression cronExpression = CronExpression.Parse(AppConstants.CRON_MIDNIGHT);
        TimeZoneInfo centralTimeZone = TimeZoneInfo.FindSystemTimeZoneById(AppConstants.TIME_ZONE);

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
                    await SendReminderMessage();
                }
            }
        );

        _client.MessageReceived += async (SocketMessage userMessage) =>
        {
            bool hasBuffCat = userMessage.CleanContent.Contains(AppConstants.BUFF_CAT_EMOTE);
            bool isBot = userMessage.Author.IsBot;

            if (hasBuffCat && !isBot)
            {
                await userMessage.Channel.SendMessageAsync("Praise be to the buff cat!");
                bool isUpdateHour = AppConstants.VALID_BUFF_UPDATE_HOURS_UTC.Contains(
                    DateTime.UtcNow.Hour
                );
                if (isUpdateHour)
                {
                    hasUpdated = true;
                }
            }
        };
    }

    private async Task SendReminderMessage()
    {
        if (_client.GetChannel(_buffChannelId) is not SocketTextChannel buffChannel)
        {
            throw new NullReferenceException(
                "Could not find a channel with the id provided" + _buffChannelId
            );
        }
        await buffChannel.SendMessageAsync(
            "@here, I have not seen the Buff Cat lately.  Please honor me with its presence if the buffs have been updated."
        );
    }
}
