using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Options;
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
builder.Services.Configure<TorrServerOptions>(builder.Configuration.GetSection(TorrServerOptions.SectionName));
builder.Services.AddHttpClient<TorrServerService>((serviceProvider, client) =>
{
    var options = serviceProvider.GetRequiredService<IOptions<TorrServerOptions>>().Value;
    var baseAddress = options.Url ?? throw new InvalidConfigurationException("TorrServer:Url");
    var user = options.User ?? throw new InvalidConfigurationException("TorrServer:User");
    var password = options.Password ?? throw new InvalidConfigurationException("TorrServer:Password");
    client.BaseAddress = new Uri(baseAddress);
    client.UseBasicAuthentication(user, password);
})
.ConfigurePrimaryHttpMessageHandler((serviceProvider) =>
{
    var options = serviceProvider.GetRequiredService<IOptions<TorrServerOptions>>().Value;
    var handler = new HttpClientHandler();
    if (options.UseSelfSignedCert)
        handler.ServerCertificateCustomValidationCallback = (_, _, _, _) => true;
    return handler;
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

    if (!result.IsSuccess)
        return Results.MessageReply(result.ErrorMessage);
    
    var keyboard = new InlineKeyboardMarkup(result.Result.Select(
        info => new []{ InlineKeyboardButton.WithCallbackData(info.Title, $"{playlistCallbackPrefix}:{info.Hash}")}
    ));
    return Results.Message("A fine catch from the server seas!", keyboard);
});


bot.HandleCallbackDataPrefix(playlistCallbackPrefix, async (string callbackData, CommandHandler handler, BotRequestContext context) =>
{
    var result = await handler.HandleGetPlaylistAsync(callbackData.Split(":")[1]);

    if (!result.IsSuccess)
        return Results.Message(result.ErrorMessage);
    
    var keyboard = new InlineKeyboardMarkup(result.Result.Select(
        info => new []{ InlineKeyboardButton.WithUrl(info.Name, info.Uri.AbsoluteUri)}
    ));
        
    if(context.Update?.CallbackQuery is not null)
        await context.Client.AnswerCallbackQuery(context.Update.CallbackQuery.Id);
    return Results.Message("Here be the links. Use ’em wisely, matey.", keyboard);
});

bot.HandleCommand("/add", async (string messageText, CommandHandler handler, BotRequestContext context) =>
{
    var result = await handler.HandleAddAsync(messageText);

    if (!result.IsSuccess)
        return Results.MessageReply(result.ErrorMessage);
    
    var (chatId, messageId) = context.GetMessageAndChatId();
    await context.Client.SetMessageReaction(chatId, messageId,
        [new ReactionTypeEmoji { Emoji = "👍" }]);
    return Results.Empty;
});


bot.Run();