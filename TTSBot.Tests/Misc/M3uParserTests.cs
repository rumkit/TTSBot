using TTSBot.Misc;

namespace TTSBot.Tests.Misc;

public class M3uParserTests
{
    public const string ValidContent = """
                                        #EXTM3U
                                        #EXTINF:0,name0
                                        https://example.com/file0.mp4
                                        #EXTINF:42
                                        ftp://example.com/file1.avi
                                        #EXTINF:-1 logo="cover.jpg",name2
                                        https://example.com/file2.mp3
                                        
                                        """;
    
    [Test]
    public async Task Parse_WhenStringHasValidFormat_ShouldReturnListOfFiles()
    {
        var result = M3uParser.Parse(ValidContent);
        await Assert.That(result).IsNotNull().And.HasCount(3);
        await Assert.That(result[0].Name).IsEqualTo("name0");
        await Assert.That(result[1].Name).IsEqualTo(string.Empty);
        await Assert.That(result[2].Name).IsEqualTo("name2");
        await Assert.That(result[0].Uri).IsEqualTo(new Uri("https://example.com/file0.mp4"));
        await Assert.That(result[1].Uri).IsEqualTo(new Uri("ftp://example.com/file1.avi"));
        await Assert.That(result[2].Uri).IsEqualTo(new Uri("https://example.com/file2.mp3"));
    }
    
    [Test]
    [Arguments("")]
    [Arguments("not#EXTM3U")]
    public async Task Parse_WhenStringHasInvalidFormat_ShouldReturnNull(string data)
    {
        var result = M3uParser.Parse(data);
        await Assert.That(result).IsNull();
    }
    
    [Test]
    [Arguments("#EXTM3U")]
    [Arguments("#EXTM3U\r\n#EXTINF:line1\r\nline2")]
    public async Task Parse_WhenStringHasNoValidFileDescriptors_ShouldReturnEmptyArray(string data)
    {
        var result = M3uParser.Parse(data);
        await Assert.That(result).IsNotNull().And.IsEmpty();
    }
}