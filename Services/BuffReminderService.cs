using Discord.WebSocket;
using Microsoft.Extensions.Configuration;

namespace GalaxyBot.Services;
public class BuffReminderService
{
    private static readonly List<int> validUpdateHours = new() { 0, 1, 2, 3, 4 };
    private static bool hasUpdated = false;
    private readonly DiscordSocketClient _client;
    private readonly IConfiguration _configuration;
    public BuffReminderService(DiscordSocketClient client, IConfiguration configuration)
    {
        _client = client;
        _client.MessageReceived += HandleMessage;
        _configuration = configuration;
    }

    public void ScheduleJob(DateTime executionTime)
    {
        Task.Run(async () =>
        {

            TimeSpan delay = executionTime.Subtract(DateTime.UtcNow);

            if (delay.TotalMilliseconds > 0)
            {
                await Task.Delay(delay);
            }
            if (hasUpdated)
            {
                hasUpdated = false;
            }
            else
            {
                if (!ulong.TryParse(_configuration.GetValue<string>("BuffChannelId"), out var buffChannelId))
                {
                    throw new Exception("Missing BuffChannelId environment variale");
                }

                if (_client.GetChannel(buffChannelId) is not SocketTextChannel buffChannel)
                {
                    throw new Exception("Could not find a channel with the id provided" + buffChannelId);
                }

                await buffChannel.SendMessageAsync("@here, I have not seen the Buff Cat lately.  Please honor me with its presence if the buffs have been updated.");
            }
            ScheduleJob(executionTime.AddDays(1));
        });
    }

    private Task HandleMessage(SocketMessage userMessage)
    {
        if (userMessage.CleanContent.Contains(":BuffCat:")
            && !userMessage.Author.IsBot)
        {
            userMessage.Channel.SendMessageAsync("Praise be to the buff cat!");
            if (validUpdateHours.Contains(DateTime.UtcNow.Hour)) hasUpdated = true;
        }
        return Task.CompletedTask;
    }
}