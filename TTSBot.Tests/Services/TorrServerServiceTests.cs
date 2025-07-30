using System.Text.Json;
using TTSBot.Services;
using TTSBot.Tests.TestUtils;

#pragma warning disable CS8604 // Possible null reference argument.
#pragma warning disable CS8602 // Dereference of a possibly null reference.

namespace TTSBot.Tests.Services;

public class TorrServerServiceTests
{
    [Test]
    public async Task AddNewTorrentAsync_ShouldSendValidPost()
    {
        using var httpClient = MockHttpClient.Create();
        var torrServerService = new TorrServerService(httpClient);

        var result = await torrServerService.AddNewTorrentAsync("displayName", "magnetLink");
        var request = httpClient.LastRequest;
        var requestBody = JsonSerializer.Deserialize<AddNewTorrentRequest>(httpClient.LastContent, JsonSerializerOptions.Web);
        
        await Assert.That(result).IsTrue();
        
        await Assert.That(request.RequestUri).IsEqualTo(new Uri("http://localhost/torrents"));
        await Assert.That(request.Content.Headers.ContentType.ToString()).StartsWith("application/json");
        
        await Assert.That(requestBody.Action).IsEqualTo("add");
        await Assert.That(requestBody.Title).IsEqualTo("displayName");
        await Assert.That(requestBody.Link).IsEqualTo("magnetLink");
    }
}