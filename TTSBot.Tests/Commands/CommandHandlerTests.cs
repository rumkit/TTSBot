using Microsoft.Extensions.Logging;
using TTSBot.Commands;
using TTSBot.Services;

#pragma warning disable CS8625 // Cannot convert null literal to non-nullable reference type.

namespace TTSBot.Tests.Commands;

public class CommandHandlerTests
{
    private readonly CommandHandler _commandHandler;

    public CommandHandlerTests()
    {
        var httpClient = new HttpClient();
        //todo: update with some sort of mock
        ILogger<CommandHandler> mockLogger = null;
        TorrServerService tsService = new (new HttpClient());
        _commandHandler = new CommandHandler(httpClient, mockLogger, tsService);
    }

    [Test]
    public async Task HandleMessage_WithValidUri_ShouldReturnSuccess()
    {
        // Arrange
        var validUrl = "https://example.com";

        // Act
        var result = await _commandHandler.HandleMessageAsync(validUrl);

        // Assert
        await Assert.That(result.IsSuccess).IsTrue();
        await Assert.That(result.ErrorMessage).IsEmpty();
    }

    [Test]
    public async Task HandleMessage_WithInvalidUri_ShouldReturnError()
    {
        // Arrange
        var invalidUrl = "not-a-valid-url";

        // Act
        var result = await _commandHandler.HandleMessageAsync(invalidUrl);

        // Assert
        await Assert.That(result.IsSuccess).IsFalse();
        await Assert.That(result.ErrorMessage).IsNotEmpty();
        await Assert.That(result.ErrorMessage).StartsWith("Blimey! That link be missin'");
    }

    [Test]
    public async Task HandleStart_ShouldReturnSuccessWithWelcomeMessage()
    {
        // Act
        var result = _commandHandler.HandleStart();

        // Assert
        await Assert.That(result.IsSuccess).IsTrue();
        await Assert.That(result.Result).IsNotEmpty();
        await Assert.That(result.Result).StartsWith("☠️ Ahoy there, ye salty sea dog!");
    }

    [Test]
    public async Task HandleMessage_WithNullInput_ShouldReturnError()
    {
        // Act
        var result = await _commandHandler.HandleMessageAsync(null);

        // Assert
        await Assert.That(result.IsSuccess).IsFalse();
        await Assert.That(result.ErrorMessage).IsNotEmpty();
    }

    [Test]
    public async Task HandleMessage_WithEmptyInput_ShouldReturnError()
    {
        // Act
        var result = await _commandHandler.HandleMessageAsync(string.Empty);

        // Assert
        await Assert.That(result.IsSuccess).IsFalse();
        await Assert.That(result.ErrorMessage).IsNotEmpty();
    }
}