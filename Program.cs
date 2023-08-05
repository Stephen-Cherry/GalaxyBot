HostApplicationBuilder builder = Host.CreateApplicationBuilder(args);
builder.Services.AddApplicationServices();

IHost host = builder.Build();

DiscordSocketClient client = host.Services.GetRequiredService<DiscordSocketClient>();
client.Ready += host.Services.StartApplicationServices;

IConfiguration configuration = host.Services.GetRequiredService<IConfiguration>();

string? token = configuration.GetValue<string>(Constants.TOKEN);
ArgumentException.ThrowIfNullOrEmpty(nameof(token));

await client.LoginAsync(TokenType.Bot, token);
await client.StartAsync();

await host.RunAsync();
