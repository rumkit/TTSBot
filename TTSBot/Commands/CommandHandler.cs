using System.Text.RegularExpressions;
using Microsoft.Extensions.Logging;
using TTSBot.Services;

namespace TTSBot.Commands;

public partial class CommandHandler(HttpClient httpClient, ILogger<CommandHandler> logger, TorrServerService tsService)
{
    public async Task<HandlerResult> HandleMessageAsync(string messageText)
    {
        if (TryFindFirstUri(messageText, out var uri))
        {
            var magnetLink = await ExtractMagnetLink(uri);
            if (magnetLink is not null)
            {
                logger.LogInformation("Got magnet link: {magnetLink}", magnetLink);
                // here we'll send the link to the TorrServer and process it's response
                await tsService.AddNewTorrentAsync("not_parsed_name", magnetLink.ToString());
                
                return HandlerResult.Success();   
            }
        }

        logger.LogError("Cannot parse the URI from the user's message");
        return HandlerResult.Error(
            "Blimey! That link be missin' or twisted like a kraken's tentacle. Toss me a proper magnet or webpage, lest ye be marooned!");
    }

    public HandlerResult<string> HandleHelp()
    {
        return HandlerResult<string>.Success("Help text");
    }

    public HandlerResult<string> HandleStart()
    {
        return HandlerResult<string>.Success(
            "☠️ Ahoy there, ye salty sea dog!" + Environment.NewLine +
            "I be Captain WebHook, scourge of the digital seas and master of magnet treasure. " +
            "Send me the magnet link or a webpage containin’ the link—I’ll haul it into the TorrServer faster than you can say “Yo ho ho!”"
            );
    }

    private bool TryFindFirstUri(string messageText, out Uri uri)
    {
        foreach (var part in messageText.Split(" "))
        {
            if (Uri.TryCreate(part, UriKind.Absolute, out var result))
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
    
    [GeneratedRegex(@"href=""(?<magnetLink>magnet:\?xt=urn:btih:[a-zA-Z0-9]+\S*"")")]
    private static partial Regex MagnetRegex();
}