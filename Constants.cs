namespace GalaxyBot;

public static class Constants
{
    public static readonly string CONNECTION_STRING = "GalaxyBotDatabase";
    public static readonly string BUFF_CHANNEL_KEY = "BuffChannelId";
    public static readonly string BUFF_CAT_EMOTE = ":BuffCat:";
    public static readonly List<int> VALID_BUFF_UPDATE_HOURS_UTC = new() { 0, 1, 2, 3, 4 };
}
