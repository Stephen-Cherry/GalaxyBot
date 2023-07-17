namespace GalaxyBot.Data;
public class CommandLog
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty!;
    public User User { get; set; } = default!;
    public DateTime UsedAt { get; set; }
}