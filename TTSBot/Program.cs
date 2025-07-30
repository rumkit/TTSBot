using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using MinimalTelegramBot;
using MinimalTelegramBot.Builder;
using MinimalTelegramBot.Handling;
using MinimalTelegramBot.Handling.Filters;
using MinimalTelegramBot.Results;
using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;
using TTSBot.Commands;
using TTSBot.Extensions;
using TTSBot.Misc;
using TTSBot.Services;

var builder = BotApplication.CreateBuilder();

// Setup services
builder.Services.AddLogging();
builder.Services.AddHttpClient();
builder.Services.AddHttpClient<TorrServerService>((serviceProvider, client) =>
{
    var configuration = serviceProvider.GetRequiredService<IConfiguration>();
    var baseAddress = configuration.GetValue<string>("TorrServer:Url") ??
                      throw new InvalidConfigurationException("TorrServer:Url", "TorrServer url is not set");
    var user = configuration.GetValue<string>("TorrServer:User") ??
                   throw new InvalidConfigurationException("TorrServer:User", "TorrServer user is not set");
    var password = configuration.GetValue<string>("TorrServer:Password") ??
                   throw new InvalidConfigurationException("TorrServer:Password", "TorrServer password is not set");
    client.BaseAddress = new Uri(baseAddress);
    client.UseBasicAuthentication(user, password);
});
builder.Services.AddScoped<CommandHandler>();
var bot = builder.Build();

// Setup bot commands
bot.HandleCommand("/start", (CommandHandler handler) => handler.HandleStart().Result);

bot.HandleCommand("/help", (CommandHandler handler) => handler.HandleHelp().Result);

bot.Handle(async (string messageText, CommandHandler handler, BotRequestContext context) =>
{
    var result = await handler.HandleMessageAsync(messageText);
    
    if (result.IsSuccess)
    {
        var (chatId, messageId) = context.GetMessageAndChatId();
        await context.Client.SetMessageReaction(chatId, messageId,
            [new ReactionTypeEmoji() { Emoji = "👍" }]);
        return Results.Empty;
    }
    
    return Results.MessageReply(result.ErrorMessage);
}).FilterUpdateType(UpdateType.Message);


bot.Run();