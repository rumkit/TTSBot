using MinimalTelegramBot.Results;
using TUnit.Assertions.AssertConditions;

namespace TTSBot.Tests.TestUtils.Assertions;

public class MessageResultKindAssertCondition(MessageResult expectedMessageResultKind) : ValueAssertCondition<IResult>
{
    protected override AssertionResult Passes(IResult? actualValue)
    {
        var actualTypeName = actualValue?.GetType().Name;
        if(actualTypeName != "MessageResult")
            return AssertionResult.Fail($"Has actual type {actualTypeName}");

        var kind = actualValue.GetMessageKind();
        if(kind != expectedMessageResultKind)
            return AssertionResult.Fail($"MessageResult is of kind {kind}");
        
        return AssertionResult.Passed;
    }

    protected override string GetFailureMessage(IResult? actualValue) => $"to be MessageResult of kind {expectedMessageResultKind}";
}