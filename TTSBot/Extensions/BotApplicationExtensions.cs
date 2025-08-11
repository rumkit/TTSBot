using MinimalTelegramBot.Builder;
using MinimalTelegramBot.Handling;
using TTSBot.Commands;

namespace TTSBot.Extensions;

public static class BotApplicationExtensions
{
    public static HandlerBuilder AttachCommandProcessor<T>(this BotApplication bot, string command) where T : ICommandProcessor
    {
        return bot.HandleCommand(command, T.ProcessCommand);
    }

    public static HandlerBuilder HandleCommandWith<T>(this BotApplication bot, string command)
        where T : CommandHandlerBase
    {
        var @delegate = (string message, T handler) => handler.TryHandleAsync(message);
        return bot.HandleCommand(command, @delegate);
    }
}