using TTSBot.Commands;

namespace TTSBot.Tests.Commands;

public class HandlerResultTests
{
    [Test]
    public async Task Success_ShouldReturnValidResult()
    {
        var result = HandlerResult.Success();

        await Assert.That(result.IsSuccess).IsTrue();
    }
    
    [Test]
    public async Task SuccessT_ShouldReturnValidResultWithResultData()
    {
        const string internalResult = "resulting string";
        var result = HandlerResult<string>.Success(internalResult);

        await Assert.That(result.IsSuccess).IsTrue();
        await Assert.That(result.Result).IsEqualTo(internalResult);
    }
    
    [Test]
    public async Task Error_ShouldReturnValidResultWithErrorMessage()
    {
        const string errorMessage = "Some stupid error";
        var result = HandlerResult.Error(errorMessage);

        await Assert.That(result.IsSuccess).IsFalse();
        await Assert.That(result.ErrorMessage).IsEqualTo(errorMessage);
    }
    
    [Test]
    public async Task ErrorT_ShouldReturnValidResultWithErrorMessage()
    {
        const string errorMessage = "Some stupid error";
        var result = HandlerResult<string>.Error(errorMessage);

        await Assert.That(result.IsSuccess).IsFalse();
        await Assert.That(result.ErrorMessage).IsEqualTo(errorMessage);
    }
}