using Cronos;
using Discord.WebSocket;
using Microsoft.Extensions.Configuration;

namespace GalaxyBot.Services;
public class BuffReminderService
{
    private static bool hasUpdated = false;
    private readonly DiscordSocketClient _client;
    private readonly IConfiguration _configuration;

    public BuffReminderService(DiscordSocketClient client, IConfiguration configuration)
    {
        _client = client;
        _configuration = configuration;

        CronExpression cronExpression = CronExpression.Parse("@midnight");
        TimeZoneInfo centralTimeZone = TimeZoneInfo.FindSystemTimeZoneById("Central Standard Time");

        TaskSchedulerService.ScheduleJob(cronExpression, centralTimeZone, async () =>
        {
            if (hasUpdated)
            {
                hasUpdated = false;
            }
            else
            {
                bool isValidBuffChannelId = ulong.TryParse(_configuration.GetValue<string>(Constants.BUFF_CHANNEL_KEY), out var buffChannelId);
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
                bool isUpdateHour = Constants.VALID_BUFF_UPDATE_HOURS_UTC.Contains(DateTime.UtcNow.Hour);
                if (isUpdateHour)
                {
                    hasUpdated = true;
                }
            }
            return Task.CompletedTask;
        };
    }

}