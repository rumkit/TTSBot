using MinimalTelegramBot;
using Telegram.Bot;
using Telegram.Bot.Types;
using TTSBot.Extensions;
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
        var context = CreateContext(update: update);
        
        var result = context.GetMessageAndChatId();

        await Assert.That(result.chatId).IsEqualTo(456);
        await Assert.That(result.messageId).IsEqualTo(123);
    }
    
    [Test]
    public async Task GetMessageAndChatId_WhenUpdateDoesNotContainMessage_ShouldThrowException()
    {
        var context = CreateContext();

        await Assert.That(() => context.GetMessageAndChatId()).Throws<InvalidOperationException>();
    }

    internal static BotRequestContext CreateContext(IServiceProvider serviceProvider = null, Update update = null, ITelegramBotClient client = null)
    {
        update ??= new Update();
        var constructorArgs = new object[] { serviceProvider, update, client };
        var type = typeof(BotRequestContext);
        var constructor = type.GetConstructors(BindingFlags.Instance | BindingFlags.NonPublic).First();
        return (BotRequestContext)constructor.Invoke(constructorArgs);
    }
}