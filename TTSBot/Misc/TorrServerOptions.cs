namespace TTSBot.Misc;

public class TorrServerOptions
{
    public const string SectionName = "TorrServer";
    public string? User { get; init; }
    public string? Password { get; init; }
    public string? Url { get; init; }
    public string? RewriteUrl { get; init; }
    public bool UseSelfSignedCert { get; init; }
}