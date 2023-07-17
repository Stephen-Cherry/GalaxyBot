using Discord;
using Discord.WebSocket;

namespace GalaxyBot.SlashCommands;

public interface ISlashCommand
{
    public abstract static SlashCommandProperties? CreateCommand();
    public abstract static Task RunCommand(SocketSlashCommand command);
}