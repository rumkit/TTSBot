using MinimalTelegramBot;
using Telegram.Bot.Types;
using TTSBot.Middleware;
using TTSBot.Tests.Extensions;
using TTSBot.Tests.TestUtils;

namespace TTSBot.Tests.Middleware;

public class ChatIdFilterMiddlewareTests
{
    private const long ActualChatId = 42;
    private readonly BotRequestContext _context;
    private readonly MockConfiguration _configuration;
    private readonly ChatIdFilterMiddleware _middleware;
    
    public ChatIdFilterMiddlewareTests()
    {
        _configuration = new MockConfiguration();
        var update = new Update
        {
            Message = new Message { Chat = new Chat {Id = ActualChatId} }
        };
        _context = BotRequestContextExtensionsTests.CreateContext(update: update);
        _middleware = new ChatIdFilterMiddleware(_configuration, new MockLogger<ChatIdFilterMiddleware>());
    }
    
    [Test]
    public async Task InvokeAsync_WhenChatIdIsAllowed_ShouldCallNextDelegate()
    {
        const long allowedChatId = 42;
        _configuration["Telegram:AllowedChatId"] = allowedChatId.ToString();
       
        var nextDelegateCalled = false;
        await _middleware.InvokeAsync(_context, _ => Task.FromResult(nextDelegateCalled = true));
        
        await Assert.That(nextDelegateCalled).IsTrue();
    }
    
    [Test]
    public async Task InvokeAsync_WhenNoAllowedChatIdSpecified_ShouldCallNextDelegate()
    {
        var nextDelegateCalled = false;
        await _middleware.InvokeAsync(_context, _ => Task.FromResult(nextDelegateCalled = true));
        
        await Assert.That(nextDelegateCalled).IsTrue();
    }
    
    [Test]
    public async Task InvokeAsync_WhenChatIdIsNotAllowed_ShouldNotCallNextDelegate()
    {
        const long allowedChatId = 34;
        _configuration["Telegram:AllowedChatId"] = allowedChatId.ToString();
       
        var nextDelegateCalled = false;
        await _middleware.InvokeAsync(_context, _ => Task.FromResult(nextDelegateCalled = true));
        
        await Assert.That(nextDelegateCalled).IsFalse();
    }
}