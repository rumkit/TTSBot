using TTSBot.Commands;

namespace TTSBot.Tests.Commands;

public class HelpCommandHandlerTests
{
    private readonly HelpCommandHandler _commandHandler = new ();
    
    [Test]
    public async Task Handle_ShouldReturnSuccessWithHelpMessage()
    {
        var result = await _commandHandler.TryHandleAsync(string.Empty);

        await Assert.That(result.IsSuccess).IsTrue();
        await Assert.That(result.Result).IsNotEmpty();
        await Assert.That(result.Result).StartsWith("Ahoy! Drop a magnet link");
    }
}