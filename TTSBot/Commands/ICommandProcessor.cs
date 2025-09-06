using MinimalTelegramBot;
using MinimalTelegramBot.Results;

namespace TTSBot.Commands;

public interface ICommandProcessor
{
    Task<IResult> ProcessAsync(BotRequestContext context);
}