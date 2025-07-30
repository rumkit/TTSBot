using System.Net;

namespace TTSBot.Tests.TestUtils;

public class MockHttpClientTests
{
    [Test]
    public async Task SendAsync_ShouldReturnOk()
    {
        using var httpClient = MockHttpClient.Create();
        
        var result = await httpClient.GetAsync("http://localhost");
        
        await Assert.That(result.IsSuccessStatusCode).IsTrue();
    }
    
    [Test]
    public async Task SendAsync_ShouldSaveLastRequest()
    {
        using var content = new StringContent("test");
        using var httpRequest = new HttpRequestMessage(HttpMethod.Put, "http://localhost") {Content = content};
        using var httpClient = MockHttpClient.Create();

        await httpClient.SendAsync(httpRequest);

        await Assert.That(httpClient.LastRequest).IsEqualTo(httpRequest);
        await Assert.That(httpClient.LastContent).IsEqualTo(await content.ReadAsStringAsync());
    }
    
    [Test]
    public async Task SendAsync_WhenCustomHandlerSetup_ShouldUseCustomHandler()
    {
        using var httpClient = MockHttpClient.Create(_ => new HttpResponseMessage(HttpStatusCode.UnavailableForLegalReasons));
        
        var response = await httpClient.GetAsync("http://localhost");
        
        await Assert.That(response.StatusCode).IsEqualTo(HttpStatusCode.UnavailableForLegalReasons);
    }
}