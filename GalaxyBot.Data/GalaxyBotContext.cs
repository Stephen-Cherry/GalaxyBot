namespace GalaxyBot.Data;

public class GalaxyBotContext : DbContext
{
    public DbSet<CommandLog> CommandLogs { get; set; }
    public DbSet<DiscordLog> DiscordLogs { get; set; }
    public DbSet<User> Users { get; set; }
    public string DbPath { get; }

    public GalaxyBotContext()
    {
        var folder = Environment.SpecialFolder.LocalApplicationData;
        var path = Environment.GetFolderPath(folder);
        DbPath = Path.Join(path, "GalaxyBot.db"); 
    }

    protected override void OnConfiguring(DbContextOptionsBuilder options) =>
        options.UseSqlite($"Data Source={DbPath}");
}
