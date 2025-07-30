using System.Net;

namespace TTSBot.Tests.TestUtils;

internal delegate HttpResponseMessage MockRequestHandler(HttpRequestMessage request);
internal class MockHttpClient : HttpClient
{
    public static MockHttpClient Create(MockRequestHandler? requestHandler = null)
    {
        var handler = new TestHttpClientHandler();
        var client = new MockHttpClient(handler);
        handler.internalHanler = request => DefaultRequestHandler(client, request, requestHandler);
        return client;
    }

    private MockHttpClient(HttpClientHandler handler) : base(handler)
    {
        BaseAddress = new Uri("http://localhost");
    }
    
    public HttpRequestMessage? LastRequest { get; private set; }
    public string? LastContent { get; private set; }

    private static HttpResponseMessage DefaultRequestHandler(MockHttpClient client, HttpRequestMessage request, MockRequestHandler? customHandler)
    {
        client.LastRequest = request;
        // content is stored separately as the request stream might be disposed sooner than the httpClient
        client.LastContent = request.Content?.ReadAsStringAsync().GetAwaiter().GetResult();
        return customHandler == null ? new HttpResponseMessage(HttpStatusCode.OK) : customHandler(request);
    }

    private class TestHttpClientHandler() : HttpClientHandler
    {
        internal MockRequestHandler internalHanler;
        protected override Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
        {
            return Task.FromResult(internalHanler(request));
        }
    }
}