namespace GalaxyBot.Data;

public class GalaxyBotContext : DbContext
{
    public DbSet<DiscordLog> DiscordLogs { get; set; }
    public string DbPath { get; }

    public GalaxyBotContext()
    {
        var folder = Environment.SpecialFolder.LocalApplicationData;
        var path = Environment.GetFolderPath(folder);
        DbPath = Path.Join(path, "GalaxyBot.db"); 
    }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder) =>
        optionsBuilder.UseSqlite($"Data Source={DbPath}");
}
