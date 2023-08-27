namespace GalaxyBot.Extensions;

public static class IConfigurationExtensions
{
    public static List<string> ValidateBotSecrets(this IConfiguration configuration)
    {
        List<string> invalidSecrets = new();

        if (configuration.GetValue<string>(Constants.TOKEN) == null)
        {
            invalidSecrets.Add(Constants.TOKEN);
        }
        if (GetNullableUInt32(configuration, Constants.BUFF_CHANNEL_ID) == null)
        {
            invalidSecrets.Add(Constants.BUFF_CHANNEL_ID);
        }
        if (GetNullableUInt32(configuration, Constants.LOG_CHANNEL_ID) == null)
        {
            invalidSecrets.Add(Constants.LOG_CHANNEL_ID);
        }

        return invalidSecrets;
    }

    private static ulong? GetNullableUInt32(IConfiguration configuration, string key)
    {
        try
        {
            return configuration.GetValue<ulong?>(key);
        }
        catch (InvalidOperationException)
        {
            return null;
        }
    }
}
