using Telegram.Bot.Requests;
using Telegram.Bot.Types;
using TTSBot.Commands;
using TTSBot.Tests.TestUtils;
using TTSBot.Tests.TestUtils.Assertions;

namespace TTSBot.Tests.Commands;

public class AddCommandProcessorTests
{
    [Test]
    public async Task Process_WhenSuccess_ShouldPutReactionAndReturnEmptyResult()
    {
        const long chatId = 4242;
        const int messageId = 42;
        var processor = new AddCommandProcessor(new SuccessHandler());
        var mockClient = new MockTelegramClient();
        var update = new Update
        {
            Message = new Message { Chat = new Chat {Id = chatId}, Id = messageId}
        };
        var result = await processor.ProcessAsync(BotRequestContextFactory.Create(update: update, client: mockClient));
        var sendReactionRequest = mockClient.LastRequest as SetMessageReactionRequest;

        await Assert.That(result).IsEmpty();
        await Assert.That(sendReactionRequest).IsNotNull();
        await Assert.That(sendReactionRequest.ChatId).IsEqualTo(chatId);
        await Assert.That(sendReactionRequest.MessageId).IsEqualTo(messageId);
        await Assert.That(sendReactionRequest.Reaction).IsEquivalentTo([new ReactionTypeEmoji { Emoji = "👍" } as ReactionType]);
    }
    
    [Test]
    public async Task Process_WhenFail_ShouldReturnReplyResult()
    {
        var processor = new AddCommandProcessor(new ErrorHandler());
        var result = await processor.ProcessAsync(BotRequestContextFactory.Create());

        await Assert.That(result).IsReply();
        await Assert.That(result).HasMessage("ErrorMessage");   
    }
}

file class SuccessHandler() : AddCommandHandler(httpClient: null, logger: null, tsService: null)
{
    protected override Task<HandlerResult<string>> HandleInternalAsync(string input)
    {
        return Task.FromResult(HandlerResult.Success("SuccessMessage"));
    }
}

file class ErrorHandler() : AddCommandHandler(httpClient: null, logger: null, tsService: null)
{
    protected override Task<HandlerResult<string>> HandleInternalAsync(string input)
    {
        return Task.FromResult(HandlerResult.Error("ErrorMessage"));
    }
}