using System.Text.RegularExpressions;
using System.Web;
using Microsoft.Extensions.Logging;
using TTSBot.Misc;
using TTSBot.Services;

namespace TTSBot.Commands;

public partial class CommandHandler(
    HttpClient httpClient, 
    ILogger<CommandHandler> logger, 
    TorrServerService tsService)
{
    private const string ServerCommunicationErrorText = "🚫 Blasted barnacles! I can’t reach the TorrServer—’tis either sunk or sailin’ in the wrong waters";
    private static readonly string[] KnownUriSchemes = ["magnet", Uri.UriSchemeHttp, Uri.UriSchemeHttps];
    [GeneratedRegex(@"href=""(?<magnetLink>magnet:\?xt=urn:btih:[a-zA-Z0-9]+\S*)""")]
    private static partial Regex MagnetRegex();
    
    public async Task<HandlerResult<TorrentFileInfo[]>> HandleGetPlaylistAsync(string hash)
    {
        logger.LogTrace("Requesting playlist for torrent: {hash}", hash);
        var playlist = await tsService.GetPlayListAsync(hash);
        
        if (string.IsNullOrEmpty(playlist))
            return HandlerResult<TorrentFileInfo[]>.Error("Empty playlist error");
        
        var fileInfos = M3uParser.Parse(playlist);
        if(fileInfos == null || fileInfos.Length == 0)
            return HandlerResult<TorrentFileInfo[]>.Error("No fileInfos error");
        
        return HandlerResult<TorrentFileInfo[]>.Success(fileInfos);
    }

    public async Task<HandlerResult<TorrentInfo[]>> HandleListAsync()
    {
        logger.LogTrace("Requesting list from the server");
        var torrents = await tsService.GetListAsync();
        
        return torrents != null 
            ? HandlerResult<TorrentInfo[]>.Success(torrents)
            : HandlerResult<TorrentInfo[]>.Error(ServerCommunicationErrorText);
    }
    
    public async Task<HandlerResult> HandleAddAsync(string messageText)
    {
        logger.LogTrace("Add command invoked with the following message: {message}", messageText);
        if (TryFindUri(messageText, out var uri))
        {
            logger.LogTrace("First URI found: {uri}", uri);
            var magnetLink = await ExtractMagnetLink(uri);
            if (magnetLink is not null)
            {
                logger.LogInformation("Got magnet link: {magnetLink}", magnetLink);
                var query = HttpUtility.HtmlDecode(magnetLink.Query);
                var displayName = HttpUtility.ParseQueryString(query).Get("dn");
                // Empty title should result in the server filling it in from the filename 
                displayName = string.IsNullOrEmpty(displayName) ? string.Empty : HttpUtility.UrlDecode(displayName);

                var addResult = await tsService.AddNewTorrentAsync(displayName, magnetLink.AbsoluteUri);

                if (addResult)
                {
                    logger.LogInformation("TorrServer query successful. The magnet link was added to downloads.");
                    return HandlerResult.Success();
                }

                logger.LogError("TorrServer query failed");
                return HandlerResult.Error(ServerCommunicationErrorText);
            }
        }

        logger.LogError("Cannot parse the URI from the user's message");
        return HandlerResult.Error("🚫 Blimey! That link be missin' or twisted like a kraken's tentacle. Toss me a proper magnet or webpage, lest ye be marooned!");
    }

    public HandlerResult<string> HandleHelp()
        => HandlerResult<string>.Success("Ahoy! Drop a magnet link or a link to a web page with one inside, and I’ll toss it straight to the TorrServer like cannon-fire");


    public HandlerResult<string> HandleStart()
        => HandlerResult<string>.Success(
            "☠️ Ahoy there, ye salty sea dog!" + Environment.NewLine +
            "I be Captain WebHook, scourge of the digital seas and master of magnet treasure. " +
            "Send me the magnet link or a webpage containin’ the link—I’ll haul it into the TorrServer faster than you can say “Yo ho ho!”"
        );

    private bool TryFindUri(string messageText, out Uri uri)
    {
        foreach (var part in messageText.Split([' ', '\n', '\r'], StringSplitOptions.RemoveEmptyEntries))
        {
            if (Uri.TryCreate(part, UriKind.Absolute, out var result) && KnownUriSchemes.Contains(result.Scheme))
            {
                uri = result;
                return true;
            }
        }

        uri = null!;
        return false;
    }

    private async Task<Uri?> ExtractMagnetLink(Uri uri)
    {
        if (uri.Scheme == "magnet")
            return uri;

        if (uri.Scheme == Uri.UriSchemeHttp || uri.Scheme == Uri.UriSchemeHttps)
        {
            var response = await httpClient.GetAsync(uri);
            if (!response.IsSuccessStatusCode)
            {
                logger.LogError("Cannot get the webpage. Response code: {responseCode}", response.StatusCode);
                return null;
            }

            var webPage = await response.Content.ReadAsStringAsync();

            var match = MagnetRegex().Match(webPage);
            if (match.Success)
                return new Uri(match.Groups["magnetLink"].Value);

            logger.LogError("Cannot find the magnet link in the webpage.");
            return null;
        }

        logger.LogCritical("Unknown uri scheme: {uriScheme}", uri.Scheme);
        return null;
    }
}