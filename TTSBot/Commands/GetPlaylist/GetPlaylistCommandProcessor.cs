using MinimalTelegramBot;
using MinimalTelegramBot.Results;
using Telegram.Bot;
using Telegram.Bot.Types.ReplyMarkups;

namespace TTSBot.Commands;

public class GetPlaylistCommandProcessor : ICommandProcessor
{
    public static Delegate ProcessCommand { get; } = 
        async (string callbackData, IGetPlaylistCommandHandler handler, BotRequestContext context) =>
    {
        var result = await handler.TryHandleAsync(callbackData.Split(":")[1]);

        if (!result.IsSuccess)
            return Results.Message(result.ErrorMessage);

        var keyboard = new InlineKeyboardMarkup(
            result.Result.Select(info => new[] { InlineKeyboardButton.WithUrl(info.Name, info.Uri.AbsoluteUri) }
            ));

        if (context.Update?.CallbackQuery is not null)
            await context.Client.AnswerCallbackQuery(context.Update.CallbackQuery.Id);

        return Results.Message("Here be the links. Use ’em wisely, matey.", keyboard);
    };
}