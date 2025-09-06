using MinimalTelegramBot.Results;
using TUnit.Assertions.AssertConditions.Interfaces;
using TUnit.Assertions.AssertionBuilders;
using TUnit.Assertions.Assertions.Generics.Conditions;

namespace TTSBot.Tests.TestUtils.Assertions;

public static class AssertionExtensions
{
    public static InvokableValueAssertionBuilder<IResult> IsMessage(this IValueSource<IResult> valueSource)
    {
        return valueSource.RegisterAssertion(new MessageResultKindAssertCondition(MessageResult.Message), []);
    }
    
    public static InvokableValueAssertionBuilder<IResult> IsReply(this IValueSource<IResult> valueSource)
    {
        return valueSource.RegisterAssertion(new MessageResultKindAssertCondition(MessageResult.Reply), []);
    }
    
    public static InvokableValueAssertionBuilder<IResult> IsEmpty(this IValueSource<IResult> valueSource)
    {
        return valueSource.RegisterAssertion(new SpecificInternalTypeAssertCondition("EmptyResult"), []);
    }
    
    public static InvokableValueAssertionBuilder<IResult> HasMessage(this IValueSource<IResult> valueSource, string message)
    {
        return valueSource.RegisterAssertion(new MessageResultTextAssertCondition(message), []);
    }
}