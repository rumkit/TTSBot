using TTSBot.Commands;
using TTSBot.Misc;
using TTSBot.Services;
using TTSBot.Tests.TestUtils;
using TUnit.Assertions.AssertConditions.Interfaces;
using TUnit.Assertions.AssertionBuilders;

namespace TTSBot.Tests.Commands;

public class RewriteHandlerTests
{
    private static readonly string[] LocalList =
    [
        "http://localhost:8080/stream/1",
        "http://localhost:8080/stream/2?parameter=true",
        "http://localhost:8080/stream/3",
    ];
    
    private const string RewriteUrl = "https://example.com:8443";
    
    [Test]
    public async Task HandleRewrite_WhenRewriteUrlIsNotSet_ShouldReturnSameList()
    {
        var options = new MockOptions<TorrServerOptions>(new TorrServerOptions());
        var fileInfos = LocalList.Select(url => new TorrentFileInfo
        {
            Name = url,
            Uri = new Uri(url),
            Length = 42
        }).ToArray();
        
        var result = new RewriteHandler(options).HandleRewrite(fileInfos).ToArray();
        
        await Assert.That(result).IsEquivalentTo(fileInfos);   
    }

    [Test]
    public async Task HandleRewrite_WhenRewriteUrlIsSet_ShouldReturnNewList()
    {
        var options = new MockOptions<TorrServerOptions>(new TorrServerOptions() { RewriteUrl = RewriteUrl });
        var fileInfos = LocalList.Select(url => new TorrentFileInfo
        {
            Name = url,
            Uri = new Uri(url),
            Length = 42
        }).ToArray();

        var result = new RewriteHandler(options).HandleRewrite(fileInfos).ToArray();

        await Assert.That(result).HasCount(3);
        var names = result.Select(x => x.Name).ToArray();
        await Assert.That(names).IsEquivalentTo(LocalList);
        await Assert.That(result.Select(r => r.Length)).All().Satisfy(x => x.IsEqualTo(42));
        await Assert.That(result.Select(r => r.Uri.ToString())).All().Satisfy(x => x.StartsWith(RewriteUrl));
    }
}