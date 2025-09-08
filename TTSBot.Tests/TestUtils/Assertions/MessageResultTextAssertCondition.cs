using MinimalTelegramBot.Results;
using TUnit.Assertions.AssertConditions;

namespace TTSBot.Tests.TestUtils.Assertions;

public class MessageResultTextAssertCondition(string expectedMessageText) : ValueAssertCondition<IResult>
{
    protected override AssertionResult Passes(IResult? actualValue)
    {
        var actualTypeName = actualValue?.GetType().Name;
        if(actualTypeName != "MessageResult")
            return AssertionResult.Fail($"Has actual type {actualTypeName}");

        var actualMessageText = actualValue.GetMessageText();
        if(actualMessageText != expectedMessageText)
            return AssertionResult.Fail($"MessageResult has text: {actualMessageText}");
        
        return AssertionResult.Passed;
    }

    protected override string GetFailureMessage(IResult? actualValue) => $"to have message set to: {expectedMessageText}";
}