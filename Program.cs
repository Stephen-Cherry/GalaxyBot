using GalaxyBot;

Bot bot = new(args);
await bot.StartApp();
await Task.Delay(Timeout.Infinite);