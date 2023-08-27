namespace GalaxyBot.Services;

public class BuffReminderService
{
    private readonly DiscordSocketClient _client;
    private bool _hasUpdated;
    private readonly ulong _buffChannelId;

    public bool HasUpdated { get { return _hasUpdated; } }

    public BuffReminderService(DiscordSocketClient client, IConfiguration configuration)
    {
        _client = client;
        _hasUpdated = false;
        _buffChannelId = configuration.GetValue<ulong>(Constants.BUFF_CHANNEL_ID);
    }

    public void StartService()
    {
        TaskSchedulerService.ScheduleJob(
            Constants.CRON_DAILY_MIDNIGHT_EXPRESSION,
            Constants.CENTRAL_TIMEZONE_INFO,
            NightlyBuffCheck
        );

        _client.MessageReceived += HandleMessageReceived;
    }

    private async Task NightlyBuffCheck()
    {
        if (_hasUpdated)
        {
            _hasUpdated = false;
        }
        else await SendReminderMessage();
    }

    private async Task HandleMessageReceived(SocketMessage userMessage)
    {
        if (IsBuffCatMessage(userMessage))
        {
            await userMessage.Channel.SendMessageAsync("Praise be to the buff cat!");
        }
        if (IsValidBuffUpdateMessage(userMessage, DateTimeOffset.UtcNow.Hour))
        {
            _hasUpdated = true;
        }
    }

    private async Task SendReminderMessage()
    {
        SocketTextChannel buffChannel = _client.GetTextChannel(_buffChannelId);
        await buffChannel.SendMessageAsync(
            "@here, I have not seen the Buff Cat lately.  Please honor me with its presence if the buffs have been updated."
        );
    }

    public bool IsValidBuffUpdateMessage(IMessage userMessage, int utcHour)
    {
        if (userMessage.Author.IsBot)
        {
            return false;
        }
        if (!IsBuffCatMessage(userMessage))
        {
            return false;
        }

        bool isABuffUpdateHour = Constants.BUFF_HOURS.Contains(utcHour);
        bool isUpdateChannel = userMessage.Channel.Id == _buffChannelId;
        if (!isABuffUpdateHour || !isUpdateChannel)
        {
            return false;
        }

        return true;
    }

    private static bool IsBuffCatMessage(IMessage userMessage)
        => userMessage.CleanContent.Contains(Constants.BUFF_CAT_EMOTE);
}
