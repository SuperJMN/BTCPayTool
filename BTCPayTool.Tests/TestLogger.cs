using BTCPayTool.Misc;
using Microsoft.Extensions.Logging;
using Xunit.Abstractions;

namespace BTCPayTool.Tests;

public class TestLogger(ITestOutputHelper output) : ILogger<ProcessRunner>
{
    public void Log<TState>(LogLevel logLevel, EventId eventId, TState state, Exception? exception, Func<TState, Exception?, string> formatter)
    {
        var format = formatter(state, exception);
        if (format is not "[null]")
        {
            output.WriteLine(format);
        }
    }

    public bool IsEnabled(LogLevel logLevel)
    {
        return true;
    }

    public IDisposable? BeginScope<TState>(TState state) where TState : notnull
    {
        return null;
    }
}