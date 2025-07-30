namespace TTSBot.Misc;

public class InvalidConfigurationException : ApplicationException
{
    public string ConfigurationKey { get; set; }
    
    public InvalidConfigurationException(string configurationKey)
    {
        ConfigurationKey = configurationKey;
    }

    public InvalidConfigurationException(string configurationKey, string message) : base(message)
    {
        ConfigurationKey = configurationKey;
    }

    public InvalidConfigurationException(string configurationKey, string message, Exception inner) : base(message, inner)
    {
        ConfigurationKey = configurationKey;
    }
}