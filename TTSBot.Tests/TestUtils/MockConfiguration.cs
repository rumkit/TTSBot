using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Primitives;

namespace TTSBot.Tests.TestUtils;

public class MockConfiguration : IConfiguration, IConfigurationRoot
{
    public Dictionary<string, string> Items { get; } = new();
    
    public IEnumerable<IConfigurationSection> GetChildren()
    {
        throw new NotImplementedException();
    }

    public IChangeToken GetReloadToken()
    {
        throw new NotImplementedException();
    }

    public IConfigurationSection GetSection(string key)
    {
        return new ConfigurationSection(this, key);
    }

    public string? this[string key]
    {
        get => Items.ContainsKey(key) ? Items[key] : null;
        set => Items[key] = value;
    }

    public void Reload()
    {
        throw new NotImplementedException();
    }

    public IEnumerable<IConfigurationProvider> Providers => [];
}