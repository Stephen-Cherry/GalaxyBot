using Discord.Commands;
using Discord.WebSocket;
using GalaxyBot.SlashCommands;
using Microsoft.VisualBasic;

namespace GalaxyBot.Handlers;

public class SlashCommandHandler
{
    private readonly DiscordSocketClient _client;
    private readonly Dictionary<string, Func<SocketSlashCommand, Task>> _commands;

    public SlashCommandHandler(DiscordSocketClient client)
    {
        _client = client;
        _client.SlashCommandExecuted += HandleSlashCommand;
        _commands = CreateCommandDictionary();
    }

    private async Task HandleSlashCommand(SocketSlashCommand slashCommand)
    {
        try
        {
            await _commands
            .Where(command => command.Key == slashCommand.CommandName)
            .Select(command => command.Value)
            .First()
            .Invoke(slashCommand);
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex.ToString());
        }
    }

    private static Dictionary<string, Func<SocketSlashCommand, Task>> CreateCommandDictionary()
    {
        return new Dictionary<string, Func<SocketSlashCommand, Task>>()
        {
            {"source", Source.RunCommand},
            {"help", Help.RunCommand},
        };
    }
}