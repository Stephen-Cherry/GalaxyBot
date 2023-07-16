using Discord;
using Discord.Commands;
using Discord.Interactions;
using Discord.WebSocket;

namespace GalaxyBot.Services;
public class LoggingService
{
    public LoggingService(DiscordSocketClient client, InteractionService interactionService)
    {
        client.Log += LogAsync;
        interactionService.Log += LogAsync;
    }

    private Task LogAsync(LogMessage message)
    {
        if (message.Exception is CommandException commandException)
        {
            Console.WriteLine($"[Command/{message.Severity}] {commandException.Command.Aliases[0]}"
            + $" failed to execute in {commandException.Context.Channel}.");
        }
        else
        {
            Console.WriteLine($"[General]/{message.Severity}] {message}");
        }
        return Task.CompletedTask;
    }
}