namespace GalaxyBot.Extensions;

public static class DiscordSocketClientExtensions
{
    public static SocketTextChannel GetTextChannel(this DiscordSocketClient client, ulong channelId)
    {
        SocketChannel channel = client.GetChannel(channelId);
        if (channel is SocketTextChannel socketTextChannel)
        {
            return socketTextChannel;
        }
        throw new ArgumentException($"Channel is not a text channel");
    }
}
