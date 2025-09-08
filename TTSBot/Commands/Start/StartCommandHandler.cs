namespace TTSBot.Commands;

public class StartCommandHandler : CommandHandlerBase
{
    protected override Task<HandlerResult<string>> HandleInternalAsync(string input) => Task.FromResult(Handle());
    
    private HandlerResult<string> Handle()
        => HandlerResult<string>.Success(
            "☠️ Ahoy there, ye salty sea dog!" + Environment.NewLine +
            "I be Captain WebHook, scourge of the digital seas and master of magnet treasure. " +
            "Send me the magnet link or a webpage containin’ the link—I’ll haul it into the TorrServer faster than you can say “Yo ho ho!”"
        );
}