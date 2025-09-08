using MinimalTelegramBot;
using MinimalTelegramBot.Builder;
using MinimalTelegramBot.Handling;
using MinimalTelegramBot.Results;
using TTSBot.Commands;

namespace TTSBot.Extensions;

public static class BotApplicationExtensions
{
    public static HandlerBuilder HandleCommandWith<T>(this BotApplication bot, string command)
        where T : ICommandProcessor
    {
        return bot.HandleCommand(command, HandlingDelegate);

        // local function to that will handle the command
        Task<IResult> HandlingDelegate(BotRequestContext context, T processor) => processor.ProcessAsync(context);
    }

    public static HandlerBuilder HandleCallbackDataPrefixWith<T>(this BotApplication bot, string prefix)
        where T : ICommandProcessor
    {
        return bot.HandleCallbackDataPrefix(prefix, HandlingDelegate);

        // local function to that will handle the command
        Task<IResult> HandlingDelegate(BotRequestContext context, T processor) => processor.ProcessAsync(context);
    }

    public static HandlerBuilder HandleCommandWithDefaultProcessor<T>(this BotApplication bot, string command)
        where T : CommandHandlerBase
    {
        return bot.HandleCommand(command, HandlingDelegate);

        Task<IResult> HandlingDelegate(BotRequestContext context, T handler)
        {
            var processor = new DefaultCommandProcessor(handler);
            return processor.ProcessAsync(context);
        }
    }
}