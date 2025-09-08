using System.Reflection;
using MinimalTelegramBot;
using Telegram.Bot;
using Telegram.Bot.Types;

namespace TTSBot.Tests.TestUtils;

public static class BotRequestContextFactory
{
    public static BotRequestContext Create(
        IServiceProvider serviceProvider = null, 
        Update update = null,
        ITelegramBotClient client = null)
    {
        update ??= new Update();
        var constructorArgs = new object[] { serviceProvider, update, client };
        var type = typeof(BotRequestContext);
        var constructor = type.GetConstructors(BindingFlags.Instance | BindingFlags.NonPublic).First();
        return (BotRequestContext)constructor.Invoke(constructorArgs);
    }
}