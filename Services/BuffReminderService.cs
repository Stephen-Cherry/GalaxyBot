namespace GalaxyBot.Services;

public class BuffReminderService
{
    private readonly DiscordSocketClient _client;
    private bool HasUpdated { get; set; }

    public BuffReminderService(DiscordSocketClient client, IConfiguration configuration)
    {
        _client = client;
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
            if (userMessage.Author.IsBot)
            {
                return;
            }

            bool hasBuffCatEmote = userMessage.CleanContent.Contains(Constants.BUFF_CAT_EMOTE);

            if (!hasBuffCatEmote)
            {
                return;
            }

            await userMessage.Channel.SendMessageAsync("Praise be to the buff cat!");
            bool isABuffUpdateHour = DateTimeOffset.UtcNow.Hour < 5;
            bool isUpdateChannel = userMessage.Channel.Id.ToString() == Constants.BUFF_CHANNEL_ID;

            if (isABuffUpdateHour && isUpdateChannel)
            {
                HasUpdated = true;
            }
        };
    }

    private async Task SendReminderMessage()
    {
        SocketTextChannel buffChannel = ClientResourceRetrieverService.GetTextChannel(
            _client,
            Constants.BUFF_CHANNEL_ID
        );
        await buffChannel.SendMessageAsync(
            "@here, I have not seen the Buff Cat lately.  Please honor me with its presence if the buffs have been updated."
        );
    }
}
