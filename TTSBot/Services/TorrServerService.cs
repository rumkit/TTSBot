using System.Net.Http.Json;

namespace TTSBot.Services;

public class TorrServerService(HttpClient httpClient)
{
    public async Task<bool> AddNewTorrentAsync(string name, string magnetLink)
    {
        var request = new AddNewTorrentRequest
        {
            Link = magnetLink,
            Title = name
        };
        using var content = JsonContent.Create(request);
        var response = await httpClient.PostAsync("torrents", content);

        return response.IsSuccessStatusCode;
    }
}