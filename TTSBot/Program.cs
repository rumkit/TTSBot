using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Options;
using MinimalTelegramBot.Builder;
using MinimalTelegramBot.Handling;
using MinimalTelegramBot.Pipeline;
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
builder.Services.Scan(scan => scan
    .FromAssemblyOf<ICommandHandler>()
    .AddClasses(classes => classes.AssignableTo<ICommandHandler>())
    .AsSelfWithInterfaces()
    .WithScopedLifetime());
builder.Services.Decorate<IGetPlaylistCommandHandler, GetPlaylistRewriteDecorator>();
builder.Services.AddScoped<ChatIdFilterMiddleware>();

var bot = builder.Build();
bot.UsePipe<ChatIdFilterMiddleware>();

// Setup bot commands
bot.HandleCommandWith<StartCommandHandler>("/start");
bot.HandleCommandWith<HelpCommandHandler>("/help");
bot.AttachCommandProcessor<ListCommandProcessor>("/list");
bot.AttachCommandProcessor<AddCommandProcessor>("/add");
bot.HandleCallbackDataPrefix("get-playlist", GetPlaylistCommandProcessor.ProcessCommand);

bot.Run();