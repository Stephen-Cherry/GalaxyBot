namespace GalaxyBot.Services;

public static class ClientResourceRetrieverService
{
    public static SocketChannel GetChannel(DiscordSocketClient client, ulong channelId)
    {
        return client.GetChannel(channelId);
    }

    public static SocketChannel GetChannel(DiscordSocketClient client, string channelIdString)
    {
        bool success = ulong.TryParse(channelIdString, out var channelId);
        if (!success)
        {
            throw new ArgumentException(
                $"Could not convert channel id ({channelIdString}) to ulong"
            );
        }
        return GetChannel(client, channelId);
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
