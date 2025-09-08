using TTSBot.Commands;
using TTSBot.Services;
using TTSBot.Tests.TestUtils;
using TTSBot.Tests.TestUtils.Assertions;

namespace TTSBot.Tests.Commands;

public class ListCommandProcessorTests
{
    [Test]
    public async Task Process_WhenSuccess_ShouldReturnMessageResult()
    {
        var processor = new ListCommandProcessor(new SuccessHandler());
        var result = await processor.ProcessAsync(BotRequestContextFactory.Create());

        await Assert.That(result).IsMessage();
        await Assert.That(result).HasMessage("A fine catch from the server seas!");
    }
    
    [Test]
    public async Task Process_WhenFail_ShouldReturnReplyResult()
    {
        var processor = new ListCommandProcessor(new ErrorHandler());
        var result = await processor.ProcessAsync(BotRequestContextFactory.Create());

        await Assert.That(result).IsReply();
        await Assert.That(result).HasMessage("ErrorMessage");
    }
}

file class SuccessHandler() : ListCommandHandler(logger: null, tsService: null)
{
    protected override Task<HandlerResult<TorrentInfo[]>> HandleInternalAsync(string input)
    {
        return Task.FromResult(HandlerResult<TorrentInfo[]>.Success([]));
    }
}

file class ErrorHandler() : ListCommandHandler(logger: null, tsService:null)
{
    protected override Task<HandlerResult<TorrentInfo[]>> HandleInternalAsync(string input)
    {
        return Task.FromResult(HandlerResult<TorrentInfo[]>.Error("ErrorMessage"));
    }
}