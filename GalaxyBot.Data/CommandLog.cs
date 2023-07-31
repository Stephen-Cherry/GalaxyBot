namespace GalaxyBot.Data;

public class CommandLog
{
    public int Id { get; set; }
    public required string Name { get; set; }
    public required User User { get; set; }
    public DateTime UsedAt { get; set; }
}
