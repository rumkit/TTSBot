using System.IO.Compression;
using System.Net;
using System.Text.Json;
using TTSBot.Commands;
using TTSBot.Services;
using TTSBot.Tests.TestUtils;

namespace TTSBot.Tests.Commands;

public class AddCommandHandlerTests
{
    [Test]
    public async Task Handle_WhenServerIsInaccessible_ShouldReturnError()
    {
        using var httpClient = MockHttpClient.Create();
        using var tsClient = MockHttpClient.Create(_ => new HttpResponseMessage(HttpStatusCode.RequestTimeout));
        var handler = new AddCommandHandler(httpClient, new MockLogger<AddCommandHandler>(), new TorrServerService(tsClient));
        
        var result = await handler.TryHandleAsync("magnet:?xt=urn:btih:f4");
        
        await Assert.That(result.IsSuccess).IsFalse();
        await Assert.That(result.ErrorMessage).StartsWith("🚫 Blasted barnacles!");
    }

    [Test]
    public async Task Handle_WhenPageIsInaccessible_ShouldReturnError()
    {
        using var httpClient = MockHttpClient.Create(_ => new HttpResponseMessage(HttpStatusCode.NotFound));
        var handler = new AddCommandHandler(httpClient, new MockLogger<AddCommandHandler>(), new TorrServerService(httpClient));
        
        var result = await handler.TryHandleAsync("http://localhost/nosuchpage");

        await Assert.That(result.IsSuccess).IsFalse();
        await Assert.That(result.ErrorMessage).IsNotEmpty();
        await Assert.That(result.ErrorMessage).StartsWith("🚫 Blimey! That link be missin'");
    }

    [Test]
    [Arguments("")]
    [Arguments("not url here")]
    [Arguments("https//broken-link.example.com")]
    [Arguments("http: // localhost")]
    [Arguments("ftp://example.com")]
    public async Task Handle_WithInvalidFormat_ShouldReturnError(string inputMessage)
    {
        var handler = new AddCommandHandler(httpClient: null, new MockLogger<AddCommandHandler>(), tsService: null);
        var result = await handler.TryHandleAsync(inputMessage);

        await Assert.That(result.IsSuccess).IsFalse();
        await Assert.That(result.ErrorMessage).IsNotEmpty();
        await Assert.That(result.ErrorMessage).StartsWith("🚫 Blimey! That link be missin'");
    }
    
    [Test]
    [Arguments("magnet:?xt=urn:btih:ACE0FBA5E&amp;dn=filename", "magnet:?xt=urn:btih:ACE0FBA5E&amp;dn=filename", "filename")]
    [Arguments("magnet:?xt=urn:btih:ACE0FBA5E&dn=filename", "magnet:?xt=urn:btih:ACE0FBA5E&dn=filename", "filename")]
    [Arguments("/add@bot_name magnet:?xt=urn:btih:ACE0FBA5E&dn=filename", "magnet:?xt=urn:btih:ACE0FBA5E&dn=filename", "filename")]
    [Arguments("@bot_name magnet:?xt=urn:btih:ACE0FBA5E&dn=F1l3%20name , download this", "magnet:?xt=urn:btih:ACE0FBA5E&dn=F1l3%20name", "F1l3 name")]
    [Arguments("link here\r\n\n\nmagnet:?xt=urn:btih:\n\r\nmagnet:?xt=urn:btih:00", "magnet:?xt=urn:btih:", "")]
    public async Task Handle_WithValidUriPresent_ShouldReturnSuccess(string message, string magnet, string filename)
    {
        using var httpClient = MockHttpClient.Create();
        using var tsClient = MockHttpClient.Create();
        var handler = new AddCommandHandler(httpClient, new MockLogger<AddCommandHandler>(), new TorrServerService(tsClient));
        
        var result = await handler.TryHandleAsync(message);
        var requestModel = JsonSerializer.Deserialize<AddNewTorrentRequest>(tsClient.LastContent, JsonSerializerOptions.Web);
        
        await Assert.That(result.IsSuccess).IsTrue();
        await Assert.That(requestModel.Link).IsEqualTo(magnet);
        await Assert.That(requestModel.Title).IsEqualTo(filename);
    }

    [Test]
    [MethodDataSource(nameof(GetSampleHtml))]
    public async Task Handle_WithValidLink_ShouldParseCorrectlyAndReturnSuccess(string fileContent)
    {
        const string magnet =
            @"magnet:?xt=urn:btih:f42f4f3181996ff4954dd5d7f166bc146810f8e3&amp;dn=archlinux-2025.07.01-x86_64.iso";
        using var responseContent = new StringContent(fileContent);
        using var httpClient = MockHttpClient.Create(_ => new HttpResponseMessage(HttpStatusCode.OK){Content = responseContent});
        using var tsClient = MockHttpClient.Create();
        var handler = new AddCommandHandler(httpClient, new MockLogger<AddCommandHandler>(), new TorrServerService(tsClient));
        
        var result = await handler.TryHandleAsync("http://example.com");
        var requestModel = JsonSerializer.Deserialize<AddNewTorrentRequest>(tsClient.LastContent, JsonSerializerOptions.Web);
        
        await Assert.That(result.IsSuccess).IsTrue();
        await Assert.That(requestModel.Link).IsEqualTo(magnet);
        await Assert.That(requestModel.Title).IsEqualTo("archlinux-2025.07.01-x86_64.iso");
    }
    
    public string GetSampleHtml()
    {
        using var fileStream = File.OpenRead(Path.Combine("Data", "sample.html.zip"));
        using var deflateStream = new DeflateStream(fileStream, CompressionMode.Decompress);
        using var reader = new StreamReader(deflateStream);
        return reader.ReadToEnd();
    }
}