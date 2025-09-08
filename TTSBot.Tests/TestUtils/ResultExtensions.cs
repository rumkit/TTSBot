using System.Reflection;
using MinimalTelegramBot.Results;
using Telegram.Bot.Types.ReplyMarkups;

namespace TTSBot.Tests.TestUtils;

public static class ResultExtensions
{
    public static MessageResult GetMessageKind(this IResult result)
    {
        var type = result.GetType();
        if (type.Name != "MessageResult")
            throw new InvalidOperationException("The result is not a MessageResult");

        var edit = type.GetField("_edit", BindingFlags.Instance | BindingFlags.NonPublic)?.GetValue(result);
        if (edit is true)
            return MessageResult.Edit;

        var reply = type.GetField("_reply", BindingFlags.Instance | BindingFlags.NonPublic)?.GetValue(result);
        if (reply is true)
            return MessageResult.Reply;

        return MessageResult.Message;
    }

    public static string? GetMessageText(this IResult result)
    {
        var type = result.GetType();
        if (type.Name != "MessageResult")
            throw new InvalidOperationException("The result is not a MessageResult");

        var text = type.GetField("_message", BindingFlags.Instance | BindingFlags.NonPublic)?.GetValue(result);
        return text?.ToString();
    }

    public static IReplyMarkup? GetKeyboard(this IResult result)
    {
        var type = result.GetType();
        if (type.Name != "MessageResult")
            throw new InvalidOperationException("The result is not a MessageResult");

        var keyboard = type.GetField("_replyMarkup", BindingFlags.Instance | BindingFlags.NonPublic)?.GetValue(result);
        return keyboard as IReplyMarkup;
    }
}

public enum MessageResult
{
    Message,
    Reply,
    Edit,
}