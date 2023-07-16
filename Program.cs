using Discord;
using Discord.Interactions;
using Discord.WebSocket;
using GalaxyBot.Services;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

IHost host = Host.CreateDefaultBuilder(args)
.UseEnvironment("Development")
.ConfigureAppConfiguration((hostingContext, configuration) =>
{
    if (hostingContext.HostingEnvironment.IsDevelopment())
    {
        configuration.AddUserSecrets<Program>();
    }
    else
    {
        configuration.AddEnvironmentVariables();
    }
})
.ConfigureServices((_, services) => services
        .AddSingleton(new DiscordSocketConfig()
        {
            GatewayIntents = GatewayIntents.GuildMembers | GatewayIntents.AllUnprivileged | GatewayIntents.MessageContent
        })
        .AddSingleton<DiscordSocketClient>()
        .AddSingleton(new InteractionServiceConfig())
        .AddSingleton<InteractionService>()
        .AddSingleton<LoggingService>()
        .AddSingleton<BuffReminderService>())
        .Build();


DiscordSocketClient client = host.Services.GetRequiredService<DiscordSocketClient>();
BuffReminderService buffReminderService = host.Services.GetRequiredService<BuffReminderService>();

client.Ready += () =>
{
    Console.WriteLine($"Successfully logged in as {client.CurrentUser.Username}");
    return Task.CompletedTask;
};

string token = host.Services.GetRequiredService<IConfiguration>().GetValue<string>("DiscordToken")
               ?? throw new Exception("Missing token");

await client.LoginAsync(TokenType.Bot, token);
await client.StartAsync();

DateTime executionTime = DateTime.UtcNow.Date + new TimeSpan(5, 0, 0);
if (DateTime.UtcNow.Hour >= 0)
{
    executionTime.AddDays(1);
}
buffReminderService.ScheduleJob(executionTime);

await Task.Delay(Timeout.Infinite);