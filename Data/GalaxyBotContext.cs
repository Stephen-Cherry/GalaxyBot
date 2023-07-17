using Microsoft.EntityFrameworkCore;

namespace GalaxyBot.Data;

public class GalaxyBotContext : DbContext
{
    public DbSet<CommandLog> CommandLogs { get; set; }
    public DbSet<User> Users { get; set; }

    public GalaxyBotContext(DbContextOptions<GalaxyBotContext> options) : base(options)
    {
    }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        var folder = Environment.SpecialFolder.ApplicationData;
        var folderPath = Environment.GetFolderPath(folder);
        var path = Path.Join(folderPath, "GalaxyBot.db");
        Console.WriteLine(path);
        optionsBuilder.UseSqlite($"Data Source={path}");
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
    }
}