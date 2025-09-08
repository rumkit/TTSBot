using MinimalTelegramBot.Results;
using TUnit.Assertions.AssertConditions;

namespace TTSBot.Tests.TestUtils.Assertions;

public class SpecificInternalTypeAssertCondition(string expectedTypeName) : ValueAssertCondition<IResult>
{
    protected override AssertionResult Passes(IResult? actualValue)
    {
        var actualTypeName = actualValue?.GetType().Name;
        return actualTypeName != expectedTypeName ? 
            AssertionResult.Fail($"Has actual type {actualTypeName}") :
            AssertionResult.Passed;
    }

    protected override string GetFailureMessage(IResult? actualValue) => $"to be of type {expectedTypeName}";
}