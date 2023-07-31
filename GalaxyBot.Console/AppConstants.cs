namespace GalaxyBot.App;

public static class AppConstants
{
    public static readonly string BUFF_CHANNEL_ID = "BUFF_CHANNEL_ID";
    public static readonly string BUFF_CAT_EMOTE = ":BuffCat:";
    public static readonly List<int> VALID_BUFF_UPDATE_HOURS_UTC = new() { 0, 1, 2, 3, 4 };
    public static readonly string TOKEN = "TOKEN";
    public static readonly string TIME_ZONE = "Central Standard Time";
    public static readonly string CRON_MIDNIGHT = "@midnight";
}
