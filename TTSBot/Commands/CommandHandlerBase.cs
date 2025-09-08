namespace TTSBot.Commands;

public abstract class CommandHandlerBase<TIn, TOut> : ICommandHandler
{
    protected const string ServerCommunicationErrorText = "🚫 Blasted barnacles! I can’t reach the TorrServer—’tis either sunk or sailin’ in the wrong waters";
    protected abstract Task<HandlerResult<TOut>> HandleInternalAsync(TIn input);

    public async Task<HandlerResult<TOut>> TryHandleAsync(TIn input)
    {
        HandlerResult<TOut> result;
        try
        {
            result = await HandleInternalAsync(input);
        }
        catch (Exception e)
        {
            result = HandlerResult<TOut>.Error($"Uncaught exception {e.Message}");
        }

        return result;
    }
}

public abstract class CommandHandlerBase<TOut> : CommandHandlerBase<string, TOut>;
public abstract class CommandHandlerBase : CommandHandlerBase<string, string>;

// Marker interface
public interface ICommandHandler;
