namespace TTSBot.Commands;

public class TorrentFileInfo
{
    public required string Name { get; set; }
    public int Length { get; set; }
    public required Uri Uri { get; set; }    
}