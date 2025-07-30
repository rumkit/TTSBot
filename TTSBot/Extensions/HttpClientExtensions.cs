using System.Net.Http.Headers;
using System.Text;

namespace TTSBot.Extensions;

public static class HttpClientExtensions
{
    public static void UseBasicAuthentication(this HttpClient client, string username, string password)
    {
        var authString = Convert.ToBase64String(Encoding.ASCII.GetBytes($"{username}:{password}"));
        client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Basic", authString);
    }
}