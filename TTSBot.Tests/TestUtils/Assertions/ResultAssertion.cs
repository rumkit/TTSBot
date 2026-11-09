using MinimalTelegramBot.Results;
using TUnit.Assertions.Core;

namespace TTSBot.Tests.TestUtils.Assertions;

/// <summary>
/// Assertion on a bot <see cref="IResult"/>. The check returns null when it passes,
/// or the failure description when it doesn't.
/// </summary>
public class ResultAssertion(AssertionContext<IResult> context, Func<IResult?, string?> check, string expectation)
    : Assertion<IResult>(context)
{
    protected override Task<AssertionResult> CheckAsync(EvaluationMetadata<IResult> metadata)
    {
        if (metadata.Exception is not null)
            return Task.FromResult(AssertionResult.Failed($"threw {metadata.Exception.GetType().Name}"));

        var failure = check(metadata.Value);
        return Task.FromResult(failure is null ? AssertionResult.Passed : AssertionResult.Failed(failure));
    }

    protected override string GetExpectation() => expectation;
}
