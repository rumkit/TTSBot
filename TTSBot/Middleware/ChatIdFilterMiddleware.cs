using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using MinimalTelegramBot;
using MinimalTelegramBot.Pipeline;

namespace TTSBot.Middleware;

public class ChatIdFilterMiddleware(IConfiguration configuration, ILogger<ChatIdFilterMiddleware> logger) : IPipe
{
    public async Task InvokeAsync(BotRequestContext context, BotRequestDelegate next)
    {
        var allowedChatId = configuration.GetValue<long?>("Telegram:AllowedChatId");
        if (!allowedChatId.HasValue || context.ChatId == allowedChatId.Value)
            await next(context);
        else
        {
            logger.LogWarning(
                "ChatId filter is enabled and chat with id: {chatId} is not allowed. The request will be terminated.",
                context.ChatId);
        }
    }
}