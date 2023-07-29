namespace GalaxyBot.SlashCommands;

public class Help : InteractionModuleBase<SocketInteractionContext>
{
    [SlashCommand("help", "Responds with a link to the wiki for GalaxyBot.")]
    public async Task Command()
    {
        await RespondAsync("https://github.com/Narolith/GalaxyBotv2/wiki");
    }
}
