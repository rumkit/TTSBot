using Microsoft.Extensions.Logging;
using TTSBot.Services;

namespace TTSBot.Commands;

public class ListCommandHandler(ILogger<ListCommandHandler> logger, TorrServerService tsService) 
    : CommandHandlerBase<TorrentInfo[]>
{
    protected override async Task<HandlerResult<TorrentInfo[]>> HandleInternalAsync(string input)
    {
        logger.LogTrace("Requesting list from the server");
        var torrents = await tsService.GetListAsync();

        logger.LogInformation("Fetched torrents info with {entriesAmount} entries", torrents?.Length ?? 0);
        return torrents != null
            ? HandlerResult<TorrentInfo[]>.Success(torrents)
            : HandlerResult<TorrentInfo[]>.Error(ServerCommunicationErrorText);
    }
}