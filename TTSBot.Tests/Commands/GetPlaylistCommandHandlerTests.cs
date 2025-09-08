using System.Net;
using TTSBot.Commands;
using TTSBot.Services;
using TTSBot.Tests.TestUtils;

namespace TTSBot.Tests.Commands;

public class GetPlaylistCommandHandlerTests
{
    [Test]
    public async Task Handle_WhenErrorResponseCode_ShouldReturnError()
    {
        using var response = new HttpResponseMessage(HttpStatusCode.BadRequest);
        using var tsClient = MockHttpClient.Create(_ => response);

        var handler = new GetPlaylistCommandHandler(new MockLogger<GetPlaylistCommandHandler>(), new TorrServerService(tsClient));
        var result = await handler.TryHandleAsync("hash");

        await Assert.That(result.IsSuccess).IsFalse();
        await Assert.That(result.ErrorMessage).StartsWith("🚫 Blasted barnacles! ");
    }
    
    [Test]
    public async Task Handle_WhenPlaylistHasInvalidFormat_ShouldReturnError()
    {
        using var responseContent = new StringContent("#NOTHING_IN_HERE_JUST_US_CRABS");

        using var response = new HttpResponseMessage(HttpStatusCode.OK) { Content = responseContent };
        using var tsClient = MockHttpClient.Create(_ => response);

        var handler = new GetPlaylistCommandHandler(new MockLogger<GetPlaylistCommandHandler>(), new TorrServerService(tsClient));
        var result = await handler.TryHandleAsync("hash");

        await Assert.That(result.IsSuccess).IsFalse();
        await Assert.That(result.ErrorMessage).StartsWith("Arrr, I fetched the playlist");
    }

    [Test]
    public async Task Handle_WhenPlaylistProvided_ShouldReturnSuccess()
    {
        using var responseContent = new StringContent(M3uParserTests.ValidContent);
        using var response = new HttpResponseMessage(HttpStatusCode.OK) { Content = responseContent };
        using var tsClient = MockHttpClient.Create(_ => response);

        var handler = new GetPlaylistCommandHandler(new MockLogger<GetPlaylistCommandHandler>(), new TorrServerService(tsClient));
        var result = await handler.TryHandleAsync("hash");

        await Assert.That(result.IsSuccess).IsTrue();
        await Assert.That(result.Result).HasCount(3);
    }
}