using Microsoft.Extensions.Options;
using TTSBot.Misc;
using TTSBot.Services;

namespace TTSBot.Commands;

public class RewriteHandler(IOptions<TorrServerOptions> options)
{
    // if the bot uses local uri (e.g. docker internal) to access the server, all its links will have the same local host
    // the method rewrites host and scheme to output the links that are accessible from WAN
    public IEnumerable<TorrentFileInfo> HandleRewrite(TorrentFileInfo[] filesInfo)
    {
        var rewriteUrl = options.Value.RewriteUrl;
        return rewriteUrl is null ? filesInfo : HandleRewriteInternal(new Uri(rewriteUrl), filesInfo);
    }

    private IEnumerable<TorrentFileInfo> HandleRewriteInternal(Uri rewriteUrl, TorrentFileInfo[] filesInfo)
    {
        foreach (var fileInfo in filesInfo)
        {
            var localUrl = new Uri(fileInfo.Uri.ToString());
            var processedUrl = new Uri(rewriteUrl, localUrl.PathAndQuery);
            
            yield return new TorrentFileInfo()
            {
                Length = fileInfo.Length,
                Name = fileInfo.Name,
                Uri = processedUrl
            };
        }
    }
}