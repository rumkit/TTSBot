using System.Text.Json.Serialization;

namespace TTSBot.Services;

public class AddNewTorrentRequest
{
    public string Action => "add";
    public string Title { get; init; } = "";
    public string Link { get; init; } = "";
    
    [JsonPropertyName("save_to_db")]
    public bool SaveToDb => true;
}