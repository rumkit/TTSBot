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

var builder = BotApplication.CreateBuilder();
builder.Services.AddHttpClient();
builder.Services.AddTransient<CommandHandler>();
var bot = builder.Build();
    
bot.HandleCommand("/start", (CommandHandler handler) => handler.HandleStart().Result);

bot.Handle(async (string messageText, CommandHandler handler, BotRequestContext context) =>
{
   var result = handler.HandleMessage(messageText);
   if (result.IsSuccess)
   {
       await context.Client.SetMessageReaction(context.ChatId, context.Update.Message.MessageId,
           new[] { new ReactionTypeEmoji() { Emoji = "👍" } });
       return Results.Empty;
   }
   return Results.Message(result.ErrorMessage);
}).FilterUpdateType(UpdateType.Message);


bot.Run();