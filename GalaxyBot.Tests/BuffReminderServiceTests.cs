using Discord;
using Discord.WebSocket;
using GalaxyBot.Services;
using NSubstitute;

namespace GalaxyBot.Tests;

[TestClass]
public class BuffReminderServiceTests
{
    private DiscordSocketClient _client = default!;
    private IMessage _message = default!;
    private IConfiguration _configuration = default!;
    private BuffReminderService _buffReminderService = default!;
    private readonly ulong _buffChannelId = 123456;

    [TestInitialize]
    public void Initialize()
    {
        Dictionary<string, string?> inMemoryConfig = new()
        {
            {Constants.BUFF_CHANNEL_ID, _buffChannelId.ToString() }
        };

        _client = Substitute.ForPartsOf<DiscordSocketClient>();
        _message = Substitute.For<IMessage>();
        _configuration = new ConfigurationBuilder().AddInMemoryCollection(inMemoryConfig).Build();
        _buffReminderService = new BuffReminderService(_client, _configuration);
    }

    [TestMethod]
    [DataRow(true, ":BuffCat:", 123456UL, 0, false)]
    [DataRow(false, ":Random Message:", 123456UL, 0, false)]
    [DataRow(false, ":BuffCat:", 123456UL, 8, false)]
    [DataRow(false, ":BuffCat:", 1234567UL, 0, false)]
    [DataRow(false, ":BuffCat:", 123456UL, 0, true)]
    public void ValidateBuffMessage(bool isBot, string message, ulong channelId, int utcHour, bool expected)
    {
        // Arrange
        _message.Author.IsBot.Returns(isBot);
        _message.CleanContent.Returns(message);
        _message.Channel.Id.Returns(channelId);

        // Act
        bool isValid = _buffReminderService.IsValidBuffUpdateMessage(_message, utcHour);

        // Assert
        Assert.AreEqual(expected, isValid);
    }
}
