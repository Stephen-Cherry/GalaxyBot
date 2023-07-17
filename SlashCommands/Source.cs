using Discord;
using Discord.WebSocket;

namespace GalaxyBot.SlashCommands;

public class Source : ISlashCommand
{
    private Source() { }

    public static SlashCommandProperties? CreateCommand()
    {
        return new SlashCommandBuilder()
        .WithName("source")
        .WithDescription("Responds with a link to the source code of the GalaxyBot.")
        .Build();
    }

    public static async Task RunCommand(SocketSlashCommand command)
    {
        await command.RespondAsync("https://www.github.com/Narolith/GalaxyBotV2");
    }
}