using MinimalTelegramBot;
using Telegram.Bot;
using Telegram.Bot.Types;
using TTSBot.Extensions;
using TTSBot.Tests.TestUtils;
using TUnit.Assertions.AssertConditions.Throws;
using BindingFlags = System.Reflection.BindingFlags;

namespace TTSBot.Tests.Extensions;

public class BotRequestContextExtensionsTests
{
    [Test]
    public async Task GetMessageAndChatId_ShouldReturnValidResult()
    {
        var update = new Update
        {
            Message = new Message() { Id = 123, Chat = new Chat() {Id = 456} }
        };
        var context = BotRequestContextFactory.Create(update: update);
        
        var result = context.GetMessageAndChatId();

        await Assert.That(result.chatId).IsEqualTo(456);
        await Assert.That(result.messageId).IsEqualTo(123);
    }
    
    [Test]
    public async Task GetMessageAndChatId_WhenUpdateDoesNotContainMessage_ShouldThrowException()
    {
        var context = BotRequestContextFactory.Create();

        await Assert.That(() => context.GetMessageAndChatId()).Throws<InvalidOperationException>();
    }
}