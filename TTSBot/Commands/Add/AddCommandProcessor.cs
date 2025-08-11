using MinimalTelegramBot;
using MinimalTelegramBot.Results;
using Telegram.Bot;
using Telegram.Bot.Types;
using TTSBot.Extensions;

namespace TTSBot.Commands;

public class AddCommandProcessor : ICommandProcessor
{
    public static Delegate ProcessCommand { get; } =
        async (string messageText, AddCommandHandler handler, BotRequestContext context) =>
        {
            var result = await handler.TryHandleAsync(messageText);

            if (!result.IsSuccess)
                return Results.MessageReply(result.ErrorMessage);

            var (chatId, messageId) = context.GetMessageAndChatId();
            await context.Client.SetMessageReaction(chatId, messageId,
                [new ReactionTypeEmoji { Emoji = "👍" }]);
            return Results.Empty;
        };
}