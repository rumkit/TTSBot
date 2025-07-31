namespace TTSBot.Tests.TestUtils;

public class MockConfigurationTests
{
    [Test]
    public async Task Indexer_WhenHasKey_ShouldReturnValue()
    {
        var configuration = new MockConfiguration();
        configuration["test"] = "value";
        
        await Assert.That(configuration["test"]).IsEqualTo("value");
    }

    [Test]
    public async Task Indexer_WhenHasNoKey_ShouldReturnNull()
    {
        var configuration = new MockConfiguration();
        
        await Assert.That(configuration["test"]).IsNull();   
    }
}