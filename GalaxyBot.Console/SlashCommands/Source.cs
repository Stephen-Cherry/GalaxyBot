namespace GalaxyBot.SlashCommands;

public class Source : InteractionModuleBase<SocketInteractionContext> {
    [SlashCommand("source", "Responds with a link to the source code of the GalaxyBot.")]
    public async Task Command() => await RespondAsync("https://www.github.com/Narolith/GalaxyBotV2");
}
