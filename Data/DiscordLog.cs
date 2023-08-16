namespace GalaxyBot.Data;

public class DiscordLog
{
    public int Id { get; set; }
    public LogType Type { get; set; }
    public LogSeverity Severity { get; set; }
    public required string Message { get; set; }
    public DateTimeOffset CreatedAt { get; set; }
}
