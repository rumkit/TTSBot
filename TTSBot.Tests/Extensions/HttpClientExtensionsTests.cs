using System.Net.Http.Headers;
using System.Text;
using TTSBot.Extensions;

namespace TTSBot.Tests.Extensions;

public class HttpClientExtensionsTests
{
    [Test]
    public async Task UseBasicAuthentication_ShouldSetAuthenticationHeader()
    {
        using var client = new HttpClient();
        const string username = "testuser";
        const string password = "testpass";
        var expectedAuthString = Convert.ToBase64String(Encoding.ASCII.GetBytes($"{username}:{password}"));
        
        client.UseBasicAuthentication(username, password);
        
        await Assert.That(client.DefaultRequestHeaders.Authorization).IsNotNull();
        await Assert.That(client.DefaultRequestHeaders.Authorization.Scheme).IsEqualTo("Basic");
        await Assert.That(client.DefaultRequestHeaders.Authorization.Parameter).IsEqualTo(expectedAuthString);
    }

    [Test]
    public async Task UseBasicAuthentication_WithExistingAuthorizationHeader_ShouldOverwrite()
    {
        using var client = new HttpClient();
        client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", "existing-token");
        
        const string username = "testuser";
        const string password = "testpass";
        var expectedAuthString = Convert.ToBase64String(Encoding.ASCII.GetBytes($"{username}:{password}"));
        
        client.UseBasicAuthentication(username, password);
        
        await Assert.That(client.DefaultRequestHeaders.Authorization).IsNotNull();
        await Assert.That(client.DefaultRequestHeaders.Authorization.Scheme).IsEqualTo("Basic");
        await Assert.That(client.DefaultRequestHeaders.Authorization.Parameter).IsEqualTo(expectedAuthString);
    }
}