using TTSBot.Commands;

namespace TTSBot.Tests.Commands;

public class StartCommandHandlerTests
{
    private readonly StartCommandHandler _commandHandler = new ();

    [Test]
    public async Task Handle_ShouldReturnSuccessWithWelcomeMessage()
    {
        var result = await _commandHandler.TryHandleAsync(string.Empty);

        await Assert.That(result.IsSuccess).IsTrue();
        await Assert.That(result.Result).IsNotEmpty();
        await Assert.That(result.Result).StartsWith("☠️ Ahoy there, ye salty sea dog!");
    }
}