using Discord;
using Discord.Interactions;
using Discord.WebSocket;
using GalaxyBot.Data;
using GalaxyBot.Handlers;
using GalaxyBot.Services;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

IHost host = Host.CreateDefaultBuilder(args)
.ConfigureAppConfiguration((hostingContext, configuration) =>
        {
            bool isDevelopmentEnvironment = hostingContext.HostingEnvironment.IsDevelopment();
            _ = isDevelopmentEnvironment
                ? configuration.AddUserSecrets<Program>()
                : configuration.AddEnvironmentVariables();
        })
 .ConfigureServices((hostingContext, services) =>
        {
            services
            .AddDbContextFactory<GalaxyBotContext>()
            .AddSingleton(new DiscordSocketConfig()
            {
                GatewayIntents = GatewayIntents.GuildMembers
                                 | GatewayIntents.AllUnprivileged
                                 | GatewayIntents.MessageContent
            })
            .AddSingleton<DiscordSocketClient>()
            .AddSingleton(new InteractionServiceConfig())
            .AddSingleton<InteractionService>()
            .AddSingleton<LoggingService>()
            .AddSingleton<BuffReminderService>()
            .AddSingleton<InteractionHandler>();
        })
        .Build();

DiscordSocketClient client = host.Services.GetRequiredService<DiscordSocketClient>();
InteractionHandler interactionHandler = host.Services.GetRequiredService<InteractionHandler>();

// Initializing services not called elsewhere.
_ = host.Services.GetRequiredService<BuffReminderService>();

string token = host.Services.GetRequiredService<IConfiguration>().GetValue<string>("DiscordToken")
               ?? throw new Exception("Missing token");

client.Ready += async () =>
    await interactionHandler.InitializeAsync();

await client.LoginAsync(TokenType.Bot, token);
await client.StartAsync();

await Task.Delay(Timeout.Infinite);