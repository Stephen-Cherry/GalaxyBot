HostApplicationBuilder builder = Host.CreateApplicationBuilder(args);
builder.Services.AddDbContextFactory<GalaxyBotContext>();
builder.Services.AddSingleton(
    new DiscordSocketConfig()
    {
        GatewayIntents =
            GatewayIntents.GuildMembers
            | GatewayIntents.AllUnprivileged
            | GatewayIntents.MessageContent
    }
);
builder.Services.AddSingleton<DiscordSocketClient>();
builder.Services.AddSingleton(new InteractionServiceConfig());
builder.Services.AddSingleton<InteractionService>();
builder.Services.AddSingleton<LoggingService>();
builder.Services.AddSingleton<BuffReminderService>();
builder.Services.AddSingleton<InteractionHandler>();

IHost host = builder.Build();

DiscordSocketClient client = host.Services.GetRequiredService<DiscordSocketClient>();
InteractionHandler interactionHandler = host.Services.GetRequiredService<InteractionHandler>();
BuffReminderService buffReminderService = host.Services.GetRequiredService<BuffReminderService>();

string? token = host.Services
    .GetRequiredService<IConfiguration>()
    .GetValue<string>(AppConstants.TOKEN);

client.Ready += async () =>
{
    await interactionHandler.InitializeAsync();
    buffReminderService.StartService();

    GalaxyBotContext context = host.Services
        .GetRequiredService<IDbContextFactory<GalaxyBotContext>>()
        .CreateDbContext();
    Console.WriteLine($"Data saving to {context.DbPath}");
};

await client.LoginAsync(TokenType.Bot, token);
await client.StartAsync();

await host.RunAsync();
