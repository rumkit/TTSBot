using Microsoft.Extensions.Logging;

namespace TTSBot.Commands;

public class CommandHandler(HttpClient client, ILogger<CommandHandler> logger)
{
    public HandlerResult HandleMessage(string messageText)
    {
        if (Uri.TryCreate(messageText, UriKind.Absolute, out var inputUri))
        {
            return HandlerResult.Success();
        }

        logger.LogError("Cannot parse the URI from the user's message");
        return HandlerResult.Error(
            "Blimey! That link be missin' or twisted like a kraken's tentacle. Toss me a proper magnet or webpage, lest ye be marooned!");
    }

    public HandlerResult<string> HandleStart()
    {
        return HandlerResult<string>.Success(
            "☠️ Ahoy there, ye salty sea dog!" + Environment.NewLine +
            "I be Captain WebHook, scourge of the digital seas and master of magnet treasure. " +
            "Send me the magnet link or a webpage containin’ the link—I’ll haul it into the TorrServer faster than you can say “Yo ho ho!”"
            );
    }
}