using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;

namespace GalaxyBot.Data;

public class GalaxyBotContext : DbContext
{
    private readonly string _connectionString;
    public DbSet<CommandLog> CommandLogs { get; set; }
    public DbSet<User> Users { get; set; }

    public GalaxyBotContext(IConfiguration configuration)
    {
        _connectionString = configuration.GetConnectionString(Constants.CONNECTION_STRING)
        ?? string.Empty;
        if (string.IsNullOrEmpty(_connectionString)) throw new NullReferenceException();
    }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        optionsBuilder.UseSqlite(_connectionString);
    }
}