using Discord;
using Discord.WebSocket;
using GalaxyBot.Extensions;
using GalaxyBot.Services;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace GalaxyBot;

public class Bot
{
    private readonly IHost _host;

    public Bot(string[] args)
    {
        _host = CreateApp(args);
    }

    private static IHost CreateApp(string[] args)
    {
        return Host.CreateDefaultBuilder(args)
        .SetAppConfiguration()
        .SetAppServices()
        .Build();
    }

    public async Task StartApp()
    {
        DiscordSocketClient client = _host.Services.GetRequiredService<DiscordSocketClient>();
        BuffReminderService buffReminderService = _host.Services.GetRequiredService<BuffReminderService>();

        string token = _host.Services.GetRequiredService<IConfiguration>().GetValue<string>("DiscordToken")
                       ?? throw new Exception("Missing token");

        client.Ready += () =>
        {
            Console.WriteLine($"Successfully logged in as {client.CurrentUser.Username}");
            return Task.CompletedTask;
        };


        await client.LoginAsync(TokenType.Bot, token);
        await client.StartAsync();

        DateTime buffReminderExecutionTime = DateTime.UtcNow.Date + new TimeSpan(5, 0, 0);
        if (DateTime.UtcNow.Hour >= 0)
        {
            buffReminderExecutionTime.AddDays(1);
        }
        buffReminderService.ScheduleJob(buffReminderExecutionTime);
    }
}