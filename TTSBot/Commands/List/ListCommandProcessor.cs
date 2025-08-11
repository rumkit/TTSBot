using MinimalTelegramBot.Results;
using Telegram.Bot.Types.ReplyMarkups;

namespace TTSBot.Commands;

public class ListCommandProcessor : ICommandProcessor
{
    public static Delegate ProcessCommand { get; } = async (ListCommandHandler handler) =>
    {
        var result = await handler.TryHandleAsync("");

        if (!result.IsSuccess)
            return Results.MessageReply(result.ErrorMessage);

        var keyboard = new InlineKeyboardMarkup(result.Result.Select(info =>
            new[] { InlineKeyboardButton.WithCallbackData(info.Title, $"get-playlist:{info.Hash}") }
        ));
        return Results.Message("A fine catch from the server seas!", keyboard);
    };
}