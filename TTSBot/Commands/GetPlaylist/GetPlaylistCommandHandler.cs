using Microsoft.Extensions.Logging;
using TTSBot.Misc;
using TTSBot.Services;

namespace TTSBot.Commands;

public interface IGetPlaylistCommandHandler
{
    Task<HandlerResult<TorrentFileInfo[]>> TryHandleAsync(string hash);   
}

public class GetPlaylistCommandHandler(ILogger<GetPlaylistCommandHandler> logger, TorrServerService tsService)
    : CommandHandlerBase<TorrentFileInfo[]>, IGetPlaylistCommandHandler
{
    protected override async Task<HandlerResult<TorrentFileInfo[]>> HandleInternalAsync(string hash)
    {
        logger.LogTrace("Requesting playlist for torrent: {hash}", hash);
        var playlist = await tsService.GetPlayListAsync(hash);
        
        if (string.IsNullOrEmpty(playlist))
            return HandlerResult<TorrentFileInfo[]>.Error(ServerCommunicationErrorText);
        
        var fileInfos = M3uParser.Parse(playlist);
        logger.LogInformation("Fetched playlist with {entriesAmount} entries", fileInfos?.Length ?? 0);
        if(fileInfos == null || fileInfos.Length == 0)
            return HandlerResult<TorrentFileInfo[]>.Error("Arrr, I fetched the playlist—but it’s emptier than a rum barrel at dawn! Nothin’ useful aboard, matey");
        
        return HandlerResult<TorrentFileInfo[]>.Success(fileInfos);
    }
}