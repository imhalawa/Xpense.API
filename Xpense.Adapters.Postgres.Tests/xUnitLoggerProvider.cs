using Microsoft.Extensions.Logging;
using Xunit.Abstractions;

namespace Xpense.Adapters.Postgres.Tests;

public class XUnitLoggerProvider(ITestOutputHelper output) : ILoggerProvider
{
    void IDisposable.Dispose()
    {
    }

    public ILogger CreateLogger(string categoryName)
        => new XUnitLogger(output, categoryName);

    private class XUnitLogger(ITestOutputHelper outHelper, string category) : ILogger
    {
        public IDisposable BeginScope<TState>(TState state) => null!;
        public bool IsEnabled(LogLevel level) => level >= LogLevel.Trace;

        public void Log<TState>(
            LogLevel level, EventId id, TState state, Exception? ex,
            Func<TState, Exception?, string> formatter)
        {
            outHelper.WriteLine($"[{level}] {category}: {formatter(state, ex)}");
        }
    }
}