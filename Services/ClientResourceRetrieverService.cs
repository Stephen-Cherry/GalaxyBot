namespace GalaxyBot.Services;

public static class ClientResourceRetrieverService
{
    public static SocketChannel GetChannel(DiscordSocketClient client, ulong channelId)
    {
        return client.GetChannel(channelId);
    }

    public static SocketChannel GetChannel(DiscordSocketClient client, string channelId)
    {
        bool isValidUlong = ulong.TryParse(channelId, out ulong newChannelId);
        if (!isValidUlong) throw new ArgumentException($"{channelId} is not valid");
        return client.GetChannel(newChannelId);
    }

    public static SocketTextChannel GetTextChannel(DiscordSocketClient client, ulong channelId)
    {
        SocketChannel channel = GetChannel(client, channelId);
        if (channel is SocketTextChannel socketTextChannel)
        {
            return socketTextChannel;
        }
        throw new ArgumentException($"Channel is not a text channel");
    }

    public static SocketTextChannel GetTextChannel(DiscordSocketClient client, string channelId)
    {
        SocketChannel channel = GetChannel(client, channelId);
        if (channel is SocketTextChannel socketTextChannel)
        {
            return socketTextChannel;
        }
        throw new ArgumentException($"Channel is not a text channel");
    }
}
