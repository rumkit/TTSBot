using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using MinimalTelegramBot;
using MinimalTelegramBot.Builder;
using MinimalTelegramBot.Handling;
using MinimalTelegramBot.Pipeline;
using MinimalTelegramBot.Results;
using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.ReplyMarkups;
using TTSBot.Commands;
using TTSBot.Extensions;
using TTSBot.Middleware;
using TTSBot.Misc;
using TTSBot.Services;

var builder = BotApplication.CreateBuilder();

// Setup services
builder.Services.AddLogging();
builder.Services.AddHttpClient();
builder.Services.AddHttpClient<TorrServerService>((serviceProvider, client) =>
{
    var configuration = serviceProvider.GetRequiredService<IConfiguration>();
    var baseAddress = configuration.GetValue<string>("TorrServer:Url") ?? throw new InvalidConfigurationException("TorrServer:Url");
    var user = configuration.GetValue<string>("TorrServer:User") ?? throw new InvalidConfigurationException("TorrServer:User");
    var password = configuration.GetValue<string>("TorrServer:Password") ?? throw new InvalidConfigurationException("TorrServer:Password");
    client.BaseAddress = new Uri(baseAddress);
    client.UseBasicAuthentication(user, password);
})
.ConfigurePrimaryHttpMessageHandler(() => new HttpClientHandler
{
    //todo: only for local testing
    ServerCertificateCustomValidationCallback = (_, _, _, _) => true
});
builder.Services.AddScoped<CommandHandler>();
builder.Services.AddScoped<ChatIdFilterMiddleware>();

var bot = builder.Build();

bot.UsePipe<ChatIdFilterMiddleware>();

// Setup bot commands
bot.HandleCommand("/start", (CommandHandler handler) => handler.HandleStart().Result);

bot.HandleCommand("/help", (CommandHandler handler) => handler.HandleHelp().Result);

const string playlistCallbackPrefix = "get-playlist";
bot.HandleCommand("/list", async (CommandHandler handler) =>
{
    var result = await handler.HandleListAsync();
    
    if (result.IsSuccess)
    {
        var keyboard = new InlineKeyboardMarkup(result.Result.Select(
            info => new []{ InlineKeyboardButton.WithCallbackData(info.Title, $"{playlistCallbackPrefix}:{info.Hash}")}
            ));
        //todo: update text
        return Results.Message("Look what I found", keyboard);
    }
    
    return Results.MessageReply(result.ErrorMessage);
});


bot.HandleCallbackDataPrefix(playlistCallbackPrefix, async (string callbackData, CommandHandler handler, BotRequestContext context) =>
{
    var result = await handler.HandleGetPlaylistAsync(callbackData.Split(":")[1]);
    
    if (result.IsSuccess)
    {
        var keyboard = new InlineKeyboardMarkup(result.Result.Select(
            info => new []{ InlineKeyboardButton.WithUrl(info.Name, info.Uri.AbsoluteUri)}
        ));
        //todo: update text
        await context.Client.AnswerCallbackQuery(context.Update.CallbackQuery.Id, "Here you go", showAlert: false);
        return Results.Message("Those are the links. Use them wisely", keyboard);
    }
    
    return Results.Message(result.ErrorMessage);
});

bot.HandleCommand("/add", async (string messageText, CommandHandler handler, BotRequestContext context, IConfiguration configuration) =>
{
    var result = await handler.HandleAddAsync(messageText);
    
    if (result.IsSuccess)
    {
        var (chatId, messageId) = context.GetMessageAndChatId();
        await context.Client.SetMessageReaction(chatId, messageId,
            [new ReactionTypeEmoji { Emoji = "👍" }]);
        return Results.Empty;
    }
    
    return Results.MessageReply(result.ErrorMessage);
});


bot.Run();