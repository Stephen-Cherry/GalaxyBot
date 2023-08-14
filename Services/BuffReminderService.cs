namespace GalaxyBot.Services;

public class BuffReminderService
{
    private readonly DiscordSocketClient _client;
    private readonly IConfiguration _configuration;
    private ulong BuffChannelId { get; init; }
    private bool HasUpdated { get; set; }

    public BuffReminderService(DiscordSocketClient client, IConfiguration configuration)
    {
        _client = client;
        _configuration = configuration;

        string? channelId = _configuration.GetValue<string>(Constants.BUFF_CHANNEL_ID);
        ArgumentException.ThrowIfNullOrEmpty(channelId, nameof(channelId));
        bool isValidChannelId = ulong.TryParse(channelId, out ulong buffChannelId);
        if (!isValidChannelId) throw new ArgumentException($"{channelId} is not a valid ulong");
        BuffChannelId = buffChannelId;
    }

    public void StartService()
    {
        CronExpression cronExpression = CronExpression.Parse(Constants.CRON_MIDNIGHT);
        TimeZoneInfo centralTimeZone = TimeZoneInfo.FindSystemTimeZoneById(Constants.TIME_ZONE);

        TaskSchedulerService.ScheduleJob(
            cronExpression,
            centralTimeZone,
            job: async () =>
            {
                if (HasUpdated)
                {
                    HasUpdated = false;
                }
                else
                {
                    await SendReminderMessage();
                }
            }
        );

        _client.MessageReceived += async (SocketMessage userMessage) =>
        {
            if (userMessage.Author.IsBot
                || userMessage.Channel.Id != BuffChannelId)
            {
                return;
            }

            bool hasBuffCatEmote = userMessage.CleanContent.Contains(Constants.BUFF_CAT_EMOTE);

            if (hasBuffCatEmote)
            {
                await userMessage.Channel.SendMessageAsync("Praise be to the buff cat!");
                bool isABuffUpdateHour = DateTimeOffset.UtcNow.Hour < 5;

                if (isABuffUpdateHour)
                {
                    HasUpdated = true;
                }
            }
        };
    }

    private async Task SendReminderMessage()
    {
        SocketTextChannel buffChannel = ClientResourceRetrieverService.GetTextChannel(
            _client,
            BuffChannelId
        );
        await buffChannel.SendMessageAsync(
            "@here, I have not seen the Buff Cat lately.  Please honor me with its presence if the buffs have been updated."
        );
    }
}
