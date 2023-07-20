using Cronos;
using Discord.WebSocket;
using Microsoft.Extensions.Configuration;

namespace GalaxyBot.Services;
public class BuffReminderService
{
    private static readonly List<int> validUpdateHoursUtc = new() { 0, 1, 2, 3, 4 };
    private static bool hasUpdated = false;
    private readonly DiscordSocketClient _client;
    private readonly IConfiguration _configuration;
    private readonly CronExpression _cronExpression;
    private readonly TimeZoneInfo _centralTimeZone;

    public BuffReminderService(DiscordSocketClient client, IConfiguration configuration)
    {
        _client = client;
        _configuration = configuration;

        _cronExpression = CronExpression.Parse("@midnight");
        _centralTimeZone = TimeZoneInfo.FindSystemTimeZoneById("Central Standard Time");

        TaskSchedulerService.ScheduleJob(_cronExpression, _centralTimeZone, async () =>
        {
            if (hasUpdated)
            {
                hasUpdated = false;
            }
            else
            {
                bool isValidBuffChannelId = !ulong.TryParse(_configuration.GetValue<string>(Constants.BUFF_CHANNEL_KEY), out var buffChannelId);
                if (!isValidBuffChannelId)
                {
                    throw new Exception("Missing BuffChannelId environment variable");
                }

                if (_client.GetChannel(buffChannelId) is not SocketTextChannel buffChannel)
                {
                    throw new Exception("Could not find a channel with the id provided" + buffChannelId);
                }

                await buffChannel.SendMessageAsync("@here, I have not seen the Buff Cat lately.  Please honor me with its presence if the buffs have been updated.");
            }
        });

        _client.MessageReceived += (SocketMessage userMessage) =>
        {
            bool hasBuffCat = userMessage.CleanContent.Contains(Constants.BUFF_CAT_EMOTE);
            bool isBot = userMessage.Author.IsBot;
            if (hasBuffCat
                && !isBot)
            {
                userMessage.Channel.SendMessageAsync("Praise be to the buff cat!");
                bool isUpdateHour = validUpdateHoursUtc.Contains(DateTime.UtcNow.Hour);
                if (isUpdateHour)
                {
                    hasUpdated = true;
                }
            }
            return Task.CompletedTask;
        };
    }

}