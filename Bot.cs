using Discord;
using Discord.WebSocket;
using GalaxyBot.Extensions;
using GalaxyBot.Handlers;
using GalaxyBot.Services;
using GalaxyBot.SlashCommands;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace GalaxyBot;

public class Bot
{
    private readonly IHost _host;
    private static readonly List<string> _commandList = new() { "source" };

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
        SlashCommandHandler slashCommandHandler = _host.Services.GetRequiredService<SlashCommandHandler>();

        string token = _host.Services.GetRequiredService<IConfiguration>().GetValue<string>("DiscordToken")
                       ?? throw new Exception("Missing token");

        client.Ready += async () =>
        {
            Console.WriteLine("Clearing Unsupported Commands");
            var commands = await client.GetGlobalApplicationCommandsAsync();
            foreach (var command in commands)
            {
                if (!_commandList.Contains(command.Name))
                {
                    await command.DeleteAsync();
                }
            }

            Console.WriteLine("Registering Commands");
            await client.CreateGlobalApplicationCommandAsync(Source.CreateCommand());

            Console.WriteLine($"Successfully logged in as {client.CurrentUser.Username}");
        };

        await client.LoginAsync(TokenType.Bot, token);
        await client.StartAsync();

        DateTime utcTodayAt0500 = DateTime.UtcNow.Date + new TimeSpan(5, 0, 0);
        DateTime buffReminderExecutionTime = utcTodayAt0500;
        if (DateTime.UtcNow.Hour >= 0)
        {
            buffReminderExecutionTime.AddDays(1);
        }
        buffReminderService.ScheduleJob(buffReminderExecutionTime);
    }
}