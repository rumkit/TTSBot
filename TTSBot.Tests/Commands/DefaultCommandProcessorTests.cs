using TTSBot.Commands;
using TTSBot.Tests.TestUtils;
using TTSBot.Tests.TestUtils.Assertions;

namespace TTSBot.Tests.Commands;

public class DefaultCommandProcessorTests
{
    [Test]
    public async Task Process_WhenSuccess_ShouldReturnMessageResult()
    {
        var processor = new DefaultCommandProcessor(new SuccessHandler());
        var result = await processor.ProcessAsync(BotRequestContextFactory.Create());

        await Assert.That(result).IsMessage();
        await Assert.That(result).HasMessage("SuccessMessage");  
    }
    
    [Test]
    public async Task Process_WhenFail_ShouldReturnReplyResult()
    {
        var processor = new DefaultCommandProcessor(new ErrorHandler());
        var result = await processor.ProcessAsync(BotRequestContextFactory.Create());

        await Assert.That(result).IsReply();
        await Assert.That(result).HasMessage("ErrorMessage");   
    }
}

file class SuccessHandler : CommandHandlerBase
{
    protected override Task<HandlerResult<string>> HandleInternalAsync(string input)
    {
        return Task.FromResult(HandlerResult.Success("SuccessMessage"));
    }
}

file class ErrorHandler : CommandHandlerBase
{
    protected override Task<HandlerResult<string>> HandleInternalAsync(string input)
    {
        return Task.FromResult(HandlerResult.Error("ErrorMessage"));
    }
}