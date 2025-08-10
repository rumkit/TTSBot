using System.IO.Compression;
using System.Net;
using System.Net.Http.Json;
using System.Text.Json;
using TTSBot.Commands;
using TTSBot.Services;
using TTSBot.Tests.Misc;
using TTSBot.Tests.TestUtils;

namespace TTSBot.Tests.Commands;

public class CommandHandlerTests
{
    private readonly CommandHandler _commandHandler = new(httpClient: null, new MockLogger<CommandHandler>(), tsService: null);
    
    [Test]
    public async Task HandleGetPlaylist_WhenErrorResponseCode_ShouldReturnError()
    {
        using var httpClient = MockHttpClient.Create();
        using var response = new HttpResponseMessage(HttpStatusCode.BadRequest);
        using var tsClient = MockHttpClient.Create(_ => response);

        var handler = new CommandHandler(httpClient, new MockLogger<CommandHandler>(), new TorrServerService(tsClient));
        var result = await handler.HandleGetPlaylistAsync("hash");

        await Assert.That(result.IsSuccess).IsFalse();
        await Assert.That(result.ErrorMessage).StartsWith("🚫 Blasted barnacles! ");
    }
    
    [Test]
    public async Task HandleGetPlaylist_WhenPlaylistHasInvalidFormat_ShouldReturnError()
    {
        using var httpClient = MockHttpClient.Create();
        using var responseContent = new StringContent("#NOTHING_IN_HERE_JUST_US_CRABS");

        using var response = new HttpResponseMessage(HttpStatusCode.OK) { Content = responseContent };
        using var tsClient = MockHttpClient.Create(_ => response);

        var handler = new CommandHandler(httpClient, new MockLogger<CommandHandler>(), new TorrServerService(tsClient));
        var result = await handler.HandleGetPlaylistAsync("hash");

        await Assert.That(result.IsSuccess).IsFalse();
        await Assert.That(result.ErrorMessage).StartsWith("Arrr, I fetched the playlist");
    }

    [Test]
    public async Task HandleGetPlaylist_WhenPlaylistProvided_ShouldReturnSuccess()
    {
        using var httpClient = MockHttpClient.Create();
        using var responseContent = new StringContent(M3uParserTests.ValidContent);
        using var response = new HttpResponseMessage(HttpStatusCode.OK) { Content = responseContent };
        using var tsClient = MockHttpClient.Create(_ => response);

        var handler = new CommandHandler(httpClient, new MockLogger<CommandHandler>(), new TorrServerService(tsClient));
        var result = await handler.HandleGetPlaylistAsync("hash");

        await Assert.That(result.IsSuccess).IsTrue();
        await Assert.That(result.Result).HasCount(3);
    }

    [Test]
    public async Task HandleList_WhenTheListIsNotEmpty_ShouldReturnSuccess()
    {
        TorrentInfo[] torrentInfos = [new() {Hash = "1", Title = "title1"}, new() {Hash = "2", Title = "title2"}];
        using var httpClient = MockHttpClient.Create();
        using var responseContent = JsonContent.Create(torrentInfos);
        using var response = new HttpResponseMessage(HttpStatusCode.OK) {Content = responseContent};
        using var tsClient = MockHttpClient.Create(_ => response); 
        
        var handler = new CommandHandler(httpClient, new MockLogger<CommandHandler>(), new TorrServerService(tsClient));
        var result = await handler.HandleListAsync();
        
        await Assert.That(result.IsSuccess).IsTrue();
        await Assert.That(result.Result).IsEquivalentTo(torrentInfos);
    }
    
    [Test]
    public async Task HandleList_WhenTheListIsEmpty_ShouldReturnError()
    {
        using var httpClient = MockHttpClient.Create();
        using var tsClient = MockHttpClient.Create();
        var handler = new CommandHandler(httpClient, new MockLogger<CommandHandler>(), new TorrServerService(tsClient));

        var result = await handler.HandleListAsync();
        
        await Assert.That(result.IsSuccess).IsFalse();
        await Assert.That(result.ErrorMessage).StartsWith("🚫 Blasted barnacles! ");
    }
    
    [Test]
    [Arguments("magnet:?xt=urn:btih:ACE0FBA5E&amp;dn=filename", "magnet:?xt=urn:btih:ACE0FBA5E&amp;dn=filename", "filename")]
    [Arguments("magnet:?xt=urn:btih:ACE0FBA5E&dn=filename", "magnet:?xt=urn:btih:ACE0FBA5E&dn=filename", "filename")]
    [Arguments("/add@bot_name magnet:?xt=urn:btih:ACE0FBA5E&dn=filename", "magnet:?xt=urn:btih:ACE0FBA5E&dn=filename", "filename")]
    [Arguments("@bot_name magnet:?xt=urn:btih:ACE0FBA5E&dn=F1l3%20name , download this", "magnet:?xt=urn:btih:ACE0FBA5E&dn=F1l3%20name", "F1l3 name")]
    [Arguments("link here\r\n\n\nmagnet:?xt=urn:btih:\n\r\nmagnet:?xt=urn:btih:00", "magnet:?xt=urn:btih:", "")]
    public async Task HandleAdd_WithValidUriPresent_ShouldReturnSuccess(string message, string magnet, string filename)
    {
        using var httpClient = MockHttpClient.Create();
        using var tsClient = MockHttpClient.Create();
        var handler = new CommandHandler(httpClient, new MockLogger<CommandHandler>(), new TorrServerService(tsClient));
        
        var result = await handler.HandleAddAsync(message);
        var requestModel = JsonSerializer.Deserialize<AddNewTorrentRequest>(tsClient.LastContent, JsonSerializerOptions.Web);
        
        await Assert.That(result.IsSuccess).IsTrue();
        await Assert.That(requestModel.Link).IsEqualTo(magnet);
        await Assert.That(requestModel.Title).IsEqualTo(filename);
    }

    [Test]
    [MethodDataSource(nameof(GetSampleHtml))]
    public async Task HandleAdd_WithValidLink_ShouldParseCorrectlyAndReturnSuccess(string fileContent)
    {
        const string magnet =
            @"magnet:?xt=urn:btih:f42f4f3181996ff4954dd5d7f166bc146810f8e3&amp;dn=archlinux-2025.07.01-x86_64.iso";
        using var responseContent = new StringContent(fileContent);
        using var httpClient = MockHttpClient.Create(_ => new HttpResponseMessage(HttpStatusCode.OK){Content = responseContent});
        using var tsClient = MockHttpClient.Create();
        var handler = new CommandHandler(httpClient, new MockLogger<CommandHandler>(), new TorrServerService(tsClient));
        
        var result = await handler.HandleAddAsync("http://example.com");
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

    [Test]
    public async Task HandleAdd_WhenServerIsInaccessible_ShouldReturnError()
    {
        using var httpClient = MockHttpClient.Create();
        using var tsClient = MockHttpClient.Create(_ => new HttpResponseMessage(HttpStatusCode.RequestTimeout));
        var handler = new CommandHandler(httpClient, new MockLogger<CommandHandler>(), new TorrServerService(tsClient));
        
        var result = await handler.HandleAddAsync("magnet:?xt=urn:btih:f4");
        
        await Assert.That(result.IsSuccess).IsFalse();
        await Assert.That(result.ErrorMessage).StartsWith("🚫 Blasted barnacles!");
    }

    [Test]
    public async Task HandleAdd_WhenPageIsInaccessible_ShouldReturnError()
    {
        using var httpClient = MockHttpClient.Create(_ => new HttpResponseMessage(HttpStatusCode.NotFound));
        var handler = new CommandHandler(httpClient, new MockLogger<CommandHandler>(), new TorrServerService(httpClient));
        
        var result = await handler.HandleAddAsync("http://localhost/nosuchpage");

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
    public async Task HandleAdd_WithInvalidFormat_ShouldReturnError(string inputMessage)
    {
        var result = await _commandHandler.HandleAddAsync(inputMessage);

        await Assert.That(result.IsSuccess).IsFalse();
        await Assert.That(result.ErrorMessage).IsNotEmpty();
        await Assert.That(result.ErrorMessage).StartsWith("🚫 Blimey! That link be missin'");
    }

    [Test]
    public async Task HandleStart_ShouldReturnSuccessWithWelcomeMessage()
    {
        var result = _commandHandler.HandleStart();

        await Assert.That(result.IsSuccess).IsTrue();
        await Assert.That(result.Result).IsNotEmpty();
        await Assert.That(result.Result).StartsWith("☠️ Ahoy there, ye salty sea dog!");
    }
    
    [Test]
    public async Task HandleHelp_ShouldReturnSuccessWithHelpMessage()
    {
        var result = _commandHandler.HandleHelp();

        await Assert.That(result.IsSuccess).IsTrue();
        await Assert.That(result.Result).IsNotEmpty();
        await Assert.That(result.Result).StartsWith("Ahoy! Drop a magnet link");
    }
}