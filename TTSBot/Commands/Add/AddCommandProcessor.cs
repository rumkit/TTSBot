using MinimalTelegramBot;
using MinimalTelegramBot.Results;
using Telegram.Bot;
using Telegram.Bot.Types;
using TTSBot.Extensions;

namespace TTSBot.Commands;

public class AddCommandProcessor(AddCommandHandler handler) : ICommandProcessor
{
    public async Task<IResult> ProcessAsync(BotRequestContext context)
    {
        var result = await handler.TryHandleAsync(context.MessageText ?? string.Empty);

        if (!result.IsSuccess)
            return Results.MessageReply(result.ErrorMessage);

        var (chatId, messageId) = context.GetMessageAndChatId();
        await context.Client.SetMessageReaction(chatId, messageId,
            [new ReactionTypeEmoji { Emoji = "👍" }]);
        return Results.Empty;
    }
}