namespace GalaxyBot;

public static class Constants
{
    public static readonly string BUFF_CHANNEL_ID = "BUFF_CHANNEL_ID";
    public static readonly string BUFF_CAT_EMOTE = ":BuffCat:";
    public static readonly string LOG_CHANNEL_ID = "LOG_CHANNEL_ID";
    public static readonly string TOKEN = "TOKEN";
    public static readonly CronExpression CRON_DAILY_MIDNIGHT_EXPRESSION = CronExpression.Parse("@midnight");
    public static readonly TimeZoneInfo CENTRAL_TIMEZONE_INFO = TimeZoneInfo.FindSystemTimeZoneById("Central Standard Time");
    public static readonly ReadOnlyCollection<int> BUFF_HOURS = new List<int>() { 17, 18, 19, 20, 21, 22, 23, 0, 1, 2, 3, 4 }.AsReadOnly();
}
