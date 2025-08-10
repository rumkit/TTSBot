using System.Text.RegularExpressions;
using TTSBot.Services;

namespace TTSBot.Misc;

public static partial class M3uParser
{
    private const string FilePrefix = "#EXTM3U";
    [GeneratedRegex("#EXTINF:(?<size>-1|\\d+)(?<attributes> [^,\\r\\n]*)?,?(?<name>.*)?")]
    private static partial Regex HeaderLineRegex();

    public static TorrentFileInfo[]? Parse(string fileContents)
    {
        var lines = fileContents.Split("\n");
        var results = new List<TorrentFileInfo>();
        if(lines[0].Trim() != FilePrefix)
            return null;

        for (var i = 1; i < lines.Length; i+=2)
        {
            var match = HeaderLineRegex().Match(lines[i].Trim());
            
            if (!match.Success)
                break;

            if (!int.TryParse(match.Groups["size"].ValueSpan, out var size))
                break;
            
            results.Add(new TorrentFileInfo
            {
                Name = match.Groups["name"].Value,
                Length = size,
                Uri = new Uri(lines[i+1])
            });
        }

        return results.ToArray();
    }
}