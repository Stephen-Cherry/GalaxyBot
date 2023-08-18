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
    private readonly string _buffCat = ":BuffCat:";

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
    public void BotMessage()
    {
        // Arrange
        _message.Author.IsBot.Returns(true);
        _message.CleanContent.Returns(_buffCat);
        _message.Channel.Id.Returns(_buffChannelId);

        // Act
        bool isValid = _buffReminderService.IsValidBuffUpdateMessage(_message, 0);

        // Assert
        Assert.AreEqual(false, isValid);
    }

    [TestMethod]
    public void UserMessageNoCat()
    {
        // Arrange
        _message.Author.IsBot.Returns(false);
        _message.CleanContent.Returns("Random Message");
        _message.Channel.Id.Returns(_buffChannelId);

        // Act
        bool isValid = _buffReminderService.IsValidBuffUpdateMessage(_message, 0);

        // Assert
        Assert.AreEqual(false, isValid);
    }

    [TestMethod]
    public void UserMessageCatOutHours()
    {
        // Arrange
        _message.Author.IsBot.Returns(false);
        _message.CleanContent.Returns(_buffCat);
        _message.Channel.Id.Returns(_buffChannelId);

        // Act
        bool isValid = _buffReminderService.IsValidBuffUpdateMessage(_message, 8);

        // Assert
        Assert.AreEqual(false, isValid);
    }

    [TestMethod]
    public void UserMessageCatInHoursOutChannel()
    {
        // Arrange
        _message.Author.IsBot.Returns(false);
        _message.CleanContent.Returns(_buffCat);
        _message.Channel.Id.Returns(_buffChannelId + 1);

        // Act
        bool isValid = _buffReminderService.IsValidBuffUpdateMessage(_message, 0);

        // Assert
        Assert.AreEqual(false, isValid);
    }

    [TestMethod]
    public void UserMessageCatInHoursInChannel()
    {
        // Arrange
        _message.Author.IsBot.Returns(false);
        _message.CleanContent.Returns(_buffCat);
        _message.Channel.Id.Returns(_buffChannelId);

        // Act
        bool isValid = _buffReminderService.IsValidBuffUpdateMessage(_message, 0);

        // Assert
        Assert.AreEqual(true, isValid);
    }
}
