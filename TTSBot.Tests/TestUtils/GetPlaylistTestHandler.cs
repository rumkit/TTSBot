using TTSBot.Commands;

namespace TTSBot.Tests.TestUtils;

public class GetPlaylistTestHandler(HandlerResult<TorrentFileInfo[]> expectedResult) : IGetPlaylistCommandHandler
{
    public string Hash { get; private set; }
    public Task<HandlerResult<TorrentFileInfo[]>> TryHandleAsync(string hash)
    {
        Hash = hash;
        return Task.FromResult(expectedResult);
    }
}