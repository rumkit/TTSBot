using Telegram.Bot.Requests;
using Telegram.Bot.Types;
using Telegram.Bot.Types.ReplyMarkups;
using TTSBot.Commands;
using TTSBot.Tests.TestUtils;
using TTSBot.Tests.TestUtils.Assertions;

namespace TTSBot.Tests.Commands;

public class GetPlaylistCommandProcessorTests
{
    [Test]
    public async Task Process_WhenSuccess_ShouldReturnMessageResultAndCompleteCallback()
    {
        var handler = new SuccessHandler();
        var processor = new GetPlaylistCommandProcessor(handler);
        var mockClient = new MockTelegramClient();
        var update = new Update
        {
            CallbackQuery = new CallbackQuery() { Data = "one:two three", Id = "myQueryId"}
        };
        var result = await processor.ProcessAsync(BotRequestContextFactory.Create(update: update, client: mockClient));
        var sendReactionRequest = mockClient.LastRequest as AnswerCallbackQueryRequest;
        
        await Assert.That(result).IsMessage();
        var keyboard = result.GetKeyboard() as InlineKeyboardMarkup;
        await Assert.That(keyboard).IsNotNull();
        // two buttons in total
        await Assert.That(keyboard.InlineKeyboard).HasCount(2);
        // first row has one button
        await Assert.That(keyboard.InlineKeyboard.First()).HasCount(1);
        // button has correct url
        await Assert.That(keyboard.InlineKeyboard.First().Single().Url).IsEqualTo("http://example.com/info1");
        // second row has one button
        await Assert.That(keyboard.InlineKeyboard.Last()).HasCount(1);
        // button has correct url
        await Assert.That(keyboard.InlineKeyboard.Last().Single().Url).IsEqualTo("http://example.com/info2");
        
        await Assert.That(handler.LastInput).IsEqualTo("two three");
        await Assert.That(sendReactionRequest).IsNotNull();
        await Assert.That(sendReactionRequest.CallbackQueryId).IsEqualTo("myQueryId");
    }
    
    [Test]
    public async Task Process_WhenFail_ShouldReturnMessageResult()
    {
        var update = new Update
        {
           CallbackQuery = new CallbackQuery() { Data = "one:two three"}
        };
        var handler = new ErrorHandler();
        var processor = new GetPlaylistCommandProcessor(handler);
        var result = await processor.ProcessAsync(BotRequestContextFactory.Create(update: update));

        await Assert.That(result).IsMessage();
        await Assert.That(result).HasMessage("ErrorMessage");
        await Assert.That(handler.LastInput).IsEqualTo("two three");
    }
}

file class SuccessHandler() : GetPlaylistCommandHandler(logger: null, tsService: null)
{
    public string LastInput { get; private set; } 
    protected override Task<HandlerResult<TorrentFileInfo[]>> HandleInternalAsync(string input)
    {
        LastInput = input;
        return Task.FromResult(HandlerResult<TorrentFileInfo[]>.Success(
            [ 
                new TorrentFileInfo {Name = "Info1", Uri = new Uri("http://example.com/info1"), Length = 42} ,
                new TorrentFileInfo {Name = "Info2", Uri = new Uri("http://example.com/info2"), Length = 4242} 
            ]
            ));
    }
}

file class ErrorHandler() : GetPlaylistCommandHandler(logger: null, tsService: null)
{
    public string LastInput { get; private set; } 
    protected override Task<HandlerResult<TorrentFileInfo[]>> HandleInternalAsync(string input)
    {
        LastInput = input;
        return Task.FromResult(HandlerResult<TorrentFileInfo[]>.Error("ErrorMessage"));
    }
}