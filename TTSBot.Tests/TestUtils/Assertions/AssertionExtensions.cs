using System.Runtime.CompilerServices;
using MinimalTelegramBot.Results;
using TUnit.Assertions.Core;

namespace TTSBot.Tests.TestUtils.Assertions;

public static class AssertionExtensions
{
    public static ResultAssertion IsMessage(this IAssertionSource<IResult> source)
    {
        source.Context.ExpressionBuilder.Append(".IsMessage()");
        return new ResultAssertion(source.Context, r => CheckMessageKind(r, MessageResult.Message),
            $"to be MessageResult of kind {MessageResult.Message}");
    }

    public static ResultAssertion IsReply(this IAssertionSource<IResult> source)
    {
        source.Context.ExpressionBuilder.Append(".IsReply()");
        return new ResultAssertion(source.Context, r => CheckMessageKind(r, MessageResult.Reply),
            $"to be MessageResult of kind {MessageResult.Reply}");
    }

    public static ResultAssertion IsEmpty(this IAssertionSource<IResult> source)
    {
        source.Context.ExpressionBuilder.Append(".IsEmpty()");
        return new ResultAssertion(source.Context, r => CheckTypeName(r, "EmptyResult"),
            "to be of type EmptyResult");
    }

    public static ResultAssertion HasMessage(this IAssertionSource<IResult> source, string message,
        [CallerArgumentExpression(nameof(message))] string? expression = null)
    {
        source.Context.ExpressionBuilder.Append($".HasMessage({expression})");
        return new ResultAssertion(source.Context, r =>
        {
            var typeFailure = CheckTypeName(r, "MessageResult");
            if (typeFailure is not null)
                return typeFailure;

            var actualText = r!.GetMessageText();
            return actualText == message ? null : $"MessageResult has text: {actualText}";
        }, $"to have message set to: {message}");
    }

    private static string? CheckTypeName(IResult? result, string expectedTypeName)
    {
        var actualTypeName = result?.GetType().Name;
        return actualTypeName == expectedTypeName ? null : $"Has actual type {actualTypeName}";
    }

    private static string? CheckMessageKind(IResult? result, MessageResult expectedKind)
    {
        var typeFailure = CheckTypeName(result, "MessageResult");
        if (typeFailure is not null)
            return typeFailure;

        var kind = result!.GetMessageKind();
        return kind == expectedKind ? null : $"MessageResult is of kind {kind}";
    }
}
