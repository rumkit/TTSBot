using System.Net;
using System.Net.Http.Json;
using TTSBot.Commands;
using TTSBot.Services;
using TTSBot.Tests.TestUtils;

namespace TTSBot.Tests.Commands;

public class ListCommandHandlerTests
{
    [Test]
    public async Task Handle_WhenTheListIsNotEmpty_ShouldReturnSuccess()
    {
        TorrentInfo[] torrentInfos = [new() {Hash = "1", Title = "title1"}, new() {Hash = "2", Title = "title2"}];
        using var responseContent = JsonContent.Create(torrentInfos);
        using var response = new HttpResponseMessage(HttpStatusCode.OK) {Content = responseContent};
        using var tsClient = MockHttpClient.Create(_ => response); 
        
        var handler = new ListCommandHandler(new MockLogger<ListCommandHandler>(), new TorrServerService(tsClient));
        var result = await handler.TryHandleAsync(string.Empty);
        
        await Assert.That(result.IsSuccess).IsTrue();
        await Assert.That(result.Result).IsEquivalentTo(torrentInfos);
    }
    
    [Test]
    public async Task Handle_WhenTheListIsEmpty_ShouldReturnError()
    {
        using var tsClient = MockHttpClient.Create();
        var handler = new ListCommandHandler(new MockLogger<ListCommandHandler>(), new TorrServerService(tsClient));

        var result = await handler.TryHandleAsync(string.Empty);
        
        await Assert.That(result.IsSuccess).IsFalse();
        await Assert.That(result.ErrorMessage).StartsWith("🚫 Blasted barnacles! ");
    }
}