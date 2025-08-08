using System.Net.Http.Json;

namespace TTSBot.Services;

public class TorrServerService(HttpClient httpClient)
{
    public async Task<TorrentInfo[]?> GetListAsync()
    {
        TorrentInfo[]? result = null;
        var request = new GetTorrentsListRequest();
        using var content = JsonContent.Create(request);
        var response = await httpClient.PostAsync("torrents", content);

        if (response.IsSuccessStatusCode)
        {
            result = await response.Content.ReadFromJsonAsync<TorrentInfo[]>();
        }

        return result;
    }
    
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

    public async Task<string?> GetPlayListAsync(string hash)
    {
        await using var responseStream = await httpClient.GetStreamAsync($"playlist?hash={hash}");
        using var reader = new StreamReader(responseStream);
        return await reader.ReadToEndAsync();
    }
}