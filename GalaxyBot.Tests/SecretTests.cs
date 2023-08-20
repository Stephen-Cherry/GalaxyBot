namespace GalaxyBot.Tests;

[TestClass]
public class SecretsTests {
    [TestMethod]
    public void HasValidSecrets() {
        // Arrange
        Dictionary<string, string?> keyValuePairs = new()
        {
            {"TOKEN", "2590gssdg093jh3-231j5kgsdoij-32jk23lkk" },
            {"BUFF_CHANNEL_ID", "2329560934602" },
            {"LOG_CHANNEL_ID", "7865845636" }
        };
        IConfiguration configuration = new ConfigurationBuilder()
        .AddInMemoryCollection(keyValuePairs)
        .Build();

        // Act
        List<string>? invalidSecrets = configuration.ValidateBotSecrets();

        // Assert
        CollectionAssert.AreEquivalent(invalidSecrets, new List<string>());
    }

    [TestMethod]
    public void HasInvalidBuffChannelId() {
        // Arrange
        Dictionary<string, string?> keyValuePairs = new()
        {
            {"TOKEN", "2590gssdg093jh3-231j5kgsdoij-32jk23lkk" },
            {"BUFF_CHANNEL_ID", "asfg" },
            {"LOG_CHANNEL_ID", "7865845636" }
        };
        IConfiguration configuration = new ConfigurationBuilder()
        .AddInMemoryCollection(keyValuePairs)
        .Build();

        // Act
        List<string>? invalidSecrets = configuration.ValidateBotSecrets();

        // Assert
        CollectionAssert.AreEquivalent(invalidSecrets, new List<string>() { "BUFF_CHANNEL_ID" });
    }

    [TestMethod]
    public void HasInvalidLogChannelId() {
        // Arrange
        Dictionary<string, string?> keyValuePairs = new()
        {
            {"TOKEN", "2590gssdg093jh3-231j5kgsdoij-32jk23lkk" },
            {"BUFF_CHANNEL_ID", "2329560934602" },
            {"LOG_CHANNEL_ID", "asgjdf" }
        };
        IConfiguration configuration = new ConfigurationBuilder()
        .AddInMemoryCollection(keyValuePairs)
        .Build();

        // Act
        List<string>? invalidSecrets = configuration.ValidateBotSecrets();

        // Assert
        CollectionAssert.AreEquivalent(invalidSecrets, new List<string>() { "LOG_CHANNEL_ID" });
    }

    [TestMethod]
    public void HasMissingToken() {
        // Arrange
        Dictionary<string, string?> keyValuePairs = new()
        {
            {"BUFF_CHANNEL_ID", "2329560934602" },
            {"LOG_CHANNEL_ID", "7865845636" }
        };
        IConfiguration configuration = new ConfigurationBuilder()
        .AddInMemoryCollection(keyValuePairs)
        .Build();

        // Act
        List<string>? invalidSecrets = configuration.ValidateBotSecrets();

        // Assert
        CollectionAssert.AreEquivalent(invalidSecrets, new List<string>() { "TOKEN" });
    }

    [TestMethod]
    public void HasMissingBuffChannelId() {
        // Arrange
        Dictionary<string, string?> keyValuePairs = new()
        {
            {"TOKEN", "2590gssdg093jh3-231j5kgsdoij-32jk23lkk" },
            {"LOG_CHANNEL_ID", "7865845636" }
        };
        IConfiguration configuration = new ConfigurationBuilder()
        .AddInMemoryCollection(keyValuePairs)
        .Build();

        // Act
        List<string>? invalidSecrets = configuration.ValidateBotSecrets();

        // Assert
        CollectionAssert.AreEquivalent(invalidSecrets, new List<string>() { "BUFF_CHANNEL_ID" });
    }

    [TestMethod]
    public void HasMissingLogChannelId() {
        // Arrange
        Dictionary<string, string?> keyValuePairs = new()
        {
            {"TOKEN", "2590gssdg093jh3-231j5kgsdoij-32jk23lkk" },
            {"BUFF_CHANNEL_ID", "2329560934602" },
        };
        IConfiguration configuration = new ConfigurationBuilder()
        .AddInMemoryCollection(keyValuePairs)
        .Build();

        // Act
        List<string>? invalidSecrets = configuration.ValidateBotSecrets();

        // Assert
        CollectionAssert.AreEquivalent(invalidSecrets, new List<string>() { "LOG_CHANNEL_ID" });
    }
}