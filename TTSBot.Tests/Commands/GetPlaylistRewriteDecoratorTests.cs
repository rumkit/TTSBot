using TTSBot.Commands;
using TTSBot.Misc;
using TTSBot.Tests.TestUtils;

namespace TTSBot.Tests.Commands;

public class GetPlaylistRewriteDecoratorTests
{
    private static readonly string[] LocalList =
    [
        "http://localhost:8080/stream/1",
        "http://localhost:8080/stream/2?parameter=true",
        "http://localhost:8080/stream/3",
    ];
    
    private const string RewriteUrl = "https://example.com:8443";
    
    [Test]
    public async Task Handle_WhenRewriteUrlIsNotSet_ShouldReturnSameList()
    {
        var options = new MockOptions<TorrServerOptions>(new TorrServerOptions());
        var fileInfos = LocalList.Select(url => new TorrentFileInfo
        {
            Name = url,
            Uri = new Uri(url),
            Length = 42
        }).ToArray();

        var expectedResult = HandlerResult<TorrentFileInfo[]>.Success(fileInfos);
        var testHandler = new GetPlaylistTestHandler(expectedResult);
        var decorator = new GetPlaylistRewriteDecorator(options, testHandler);
        var result = await decorator.TryHandleAsync("hash");
        
        await Assert.That(testHandler.Hash).IsEqualTo("hash");
        await Assert.That(result).IsEquivalentTo(expectedResult);   
    }

    [Test]
    public async Task Handle_WhenRewriteUrlIsSet_ShouldReturnNewList()
    {
        var options = new MockOptions<TorrServerOptions>(new TorrServerOptions() { RewriteUrl = RewriteUrl });
        var fileInfos = LocalList.Select(url => new TorrentFileInfo
        {
            Name = url,
            Uri = new Uri(url),
            Length = 42
        }).ToArray();

        var expectedResult = HandlerResult<TorrentFileInfo[]>.Success(fileInfos);
        var testHandler = new GetPlaylistTestHandler(expectedResult);
        var decorator = new GetPlaylistRewriteDecorator(options, testHandler);
        var result = await decorator.TryHandleAsync("hash");

        await Assert.That(testHandler.Hash).IsEqualTo("hash");
        await Assert.That(result.IsSuccess).IsTrue();
        await Assert.That(result.Result).HasCount(3);
        var names = result.Result.Select(x => x.Name).ToArray();
        await Assert.That(names).IsEquivalentTo(LocalList);
        await Assert.That(result.Result.Select(r => r.Length)).All().Satisfy(x => x.IsEqualTo(42));
        await Assert.That(result.Result.Select(r => r.Uri.ToString())).All().Satisfy(x => x.StartsWith(RewriteUrl));
    }

    [Test]
    public async Task Handle_WhenRewriteUrlIsSetAndBaseHandlerReturnsError_ShouldReturnSameError()
    {
        var options = new MockOptions<TorrServerOptions>(new TorrServerOptions() { RewriteUrl = RewriteUrl });
        var expectedResult = HandlerResult<TorrentFileInfo[]>.Error("error message");
        var testHandler = new GetPlaylistTestHandler(expectedResult);
        var decorator = new GetPlaylistRewriteDecorator(options, testHandler);
        var result = await decorator.TryHandleAsync("hash");

        await Assert.That(testHandler.Hash).IsEqualTo("hash");
        await Assert.That(result).IsEquivalentTo(expectedResult);
    }
}