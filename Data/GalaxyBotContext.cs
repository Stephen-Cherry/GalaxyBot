namespace GalaxyBot.Data;

public class GalaxyBotContext : DbContext
{
    public DbSet<BotLogMessage> LogMessages { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder) =>
        optionsBuilder.UseSqlite($"Data Source=GalaxyBot.db");
}
