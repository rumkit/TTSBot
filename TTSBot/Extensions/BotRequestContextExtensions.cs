using MinimalTelegramBot;

namespace TTSBot.Extensions;

public static class BotRequestContextExtensions
{
    public static (long chatId, int messageId) GetMessageAndChatId(this BotRequestContext context)
    {
        if(context.Update.Message is null)
            throw new InvalidOperationException("The update does not contain a message");
        
        var chatId = context.ChatId;
        var messageId = context.Update.Message.MessageId;
        
        return (chatId, messageId);
    }
}