using MinimalTelegramBot;
using MinimalTelegramBot.Results;

namespace TTSBot.Commands;

public class DefaultCommandProcessor(CommandHandlerBase handler) : ICommandProcessor
{
    public async Task<IResult> ProcessAsync(BotRequestContext context)
    {
        var result = await handler.TryHandleAsync(context.MessageText ?? string.Empty);

        return result.IsSuccess ? 
            Results.Message(result.Result) : 
            Results.MessageReply(result.ErrorMessage);
    }
}