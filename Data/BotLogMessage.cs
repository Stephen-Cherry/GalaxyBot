namespace GalaxyBot.Data;

public class BotLogMessage
{
    public int Id { get; set; }
    public required string Message { get; set; }
    public required LogSeverity Severity { get; set; }
    public string? Source { get; set; }
}
