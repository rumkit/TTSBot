public class UriHandler(HttpClient client)
{
    public HandlerResult Handle(string messageText)
    {
        if (Uri.TryCreate(messageText, UriKind.Absolute, out var inputUri))
        {
            return HandlerResult.Success();
        }

        return HandlerResult.Error(
            "Blimey! That link be missin' or twisted like a kraken's tentacle. Toss me a proper magnet or webpage, lest ye be marooned!");
    }
}