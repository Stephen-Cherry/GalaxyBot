using Discord;
using Discord.WebSocket;
using GalaxyBot.Extensions;
using GalaxyBot.Handlers;
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
        InteractionHandler interactionHandler = _host.Services.GetRequiredService<InteractionHandler>();

        string token = _host.Services.GetRequiredService<IConfiguration>().GetValue<string>("DiscordToken")
                       ?? throw new Exception("Missing token");

        client.Ready += async () =>
        {
            Console.WriteLine("Registering Commands");
            await interactionHandler.InitializeAsync();

            Console.WriteLine($"Successfully logged in as {client.CurrentUser.Username}");
        };

        await client.LoginAsync(TokenType.Bot, token);
        await client.StartAsync();
    }
}