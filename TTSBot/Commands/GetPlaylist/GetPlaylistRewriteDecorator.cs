using Microsoft.Extensions.Options;
using TTSBot.Misc;
using TTSBot.Services;

namespace TTSBot.Commands;

public class GetPlaylistRewriteDecorator(IOptions<TorrServerOptions> options, IGetPlaylistCommandHandler handler) 
    : CommandHandlerBase<TorrentFileInfo[]>, IGetPlaylistCommandHandler
{
    protected override async Task<HandlerResult<TorrentFileInfo[]>> HandleInternalAsync(string input)
    {
        // if the bot uses local uri (e.g. docker internal) to access the server, all its links will have the same local host
        // the method rewrites host and scheme to output the links that are accessible from WAN
        var rewriteUrl = options.Value.RewriteUrl;
        var originalResult = await handler.TryHandleAsync(input);
        
        if(rewriteUrl is null || !originalResult.IsSuccess)
            return originalResult;

        var newResult = RewriteUrl(new Uri(rewriteUrl), originalResult.Result).ToArray();
        return HandlerResult<TorrentFileInfo[]>.Success(newResult);
    }
    
    private static IEnumerable<TorrentFileInfo> RewriteUrl(Uri rewriteUrl, TorrentFileInfo[] filesInfo)
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