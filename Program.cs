using Discord;
using Discord.WebSocket;
using GalaxyBot.Extensions;
using GalaxyBot.Handlers;
using GalaxyBot.Services;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

IHost host = Host.CreateDefaultBuilder(args)
        .SetAppConfiguration()
        .SetAppServices()
        .Build();

DiscordSocketClient client = host.Services.GetRequiredService<DiscordSocketClient>();
BuffReminderService buffReminderService = host.Services.GetRequiredService<BuffReminderService>();
InteractionHandler interactionHandler = host.Services.GetRequiredService<InteractionHandler>();

string token = host.Services.GetRequiredService<IConfiguration>().GetValue<string>("DiscordToken")
               ?? throw new Exception("Missing token");

client.Ready += async () =>
{
    Console.WriteLine("Registering Commands");
    await interactionHandler.InitializeAsync();

    Console.WriteLine($"Successfully logged in as {client.CurrentUser.Username}");
};

await client.LoginAsync(TokenType.Bot, token);
await client.StartAsync();

await Task.Delay(Timeout.Infinite);