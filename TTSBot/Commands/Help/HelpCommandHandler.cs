namespace TTSBot.Commands;

public class HelpCommandHandler : CommandHandlerBase
{
    protected override Task<HandlerResult<string>> HandleInternalAsync(string input)
        => Task.FromResult(Handle());
    
    private HandlerResult<string> Handle() =>
        HandlerResult<string>.Success("Ahoy! Drop a magnet link or a link to a web page with one inside, and I’ll toss it straight to the TorrServer like cannon-fire");
}