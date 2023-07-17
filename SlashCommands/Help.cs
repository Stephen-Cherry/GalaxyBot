using Discord;
using Discord.WebSocket;

namespace GalaxyBot.SlashCommands;

public class Help : ISlashCommand
{
    public static SlashCommandProperties? CreateCommand()
    {
        return new SlashCommandBuilder()
        .WithName("help")
        .WithDescription("Responds with a link to the wiki for GalaxyBot.")
        .Build();

    }

    public static async Task RunCommand(SocketSlashCommand command)
    {
        await command.RespondAsync("https://github.com/Narolith/GalaxyBotv2/wiki");
    }
}