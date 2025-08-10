using Microsoft.Extensions.Options;

namespace TTSBot.Tests.TestUtils;

public class MockOptions<T> : IOptions<T> where T : class
{
    public MockOptions(T value) : this()
    {
        Value = value;
    }

    public MockOptions() { }

    public T Value { get; }
}