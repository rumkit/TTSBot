using System.Text.RegularExpressions;
using System.Web;
using Microsoft.Extensions.Logging;
using TTSBot.Services;

namespace TTSBot.Commands;

public partial class AddCommandHandler(HttpClient httpClient, ILogger<AddCommandHandler> logger, TorrServerService tsService)
    : CommandHandlerBase
{
    private static readonly string[] KnownUriSchemes = ["magnet", Uri.UriSchemeHttp, Uri.UriSchemeHttps];

    protected override async Task<HandlerResult<string>> HandleInternalAsync(string messageText)
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
        return HandlerResult.Error(
            "🚫 Blimey! That link be missin' or twisted like a kraken's tentacle. Toss me a proper magnet or webpage, lest ye be marooned!");
    }

    [GeneratedRegex(@"href=""(?<magnetLink>magnet:\?xt=urn:btih:[a-zA-Z0-9]+\S*)""")]
    private static partial Regex MagnetRegex();

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