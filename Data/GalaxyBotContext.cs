using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;

namespace GalaxyBot.Data;

public class GalaxyBotContext : DbContext
{
    private readonly IConfiguration _configuration;
    public DbSet<CommandLog> CommandLogs { get; set; }
    public DbSet<User> Users { get; set; }

    public GalaxyBotContext(IConfiguration configuration)
    {
        _configuration = configuration;
    }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        optionsBuilder.UseSqlite(_configuration.GetConnectionString("GalaxyBotDatabase"));
    }
}