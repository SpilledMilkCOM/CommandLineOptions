using System;
using System.Collections.Concurrent;
using Microsoft.Extensions.Logging;

namespace ConsoleAppHost.Tests.Utilities
{
    internal sealed class InMemoryLogger : ILogger
    {
        private readonly ConcurrentQueue<string> _logs;
        public InMemoryLogger(ConcurrentQueue<string> logs) => _logs = logs;
        IDisposable ILogger.BeginScope<TState>(TState state) => NullScope.Instance;
        public bool IsEnabled(LogLevel logLevel) => true;
        public void Log<TState>(LogLevel logLevel, EventId eventId, TState state, Exception? exception, Func<TState, Exception?, string> formatter)
        {
            _logs.Enqueue(formatter(state, exception));
        }
    }
}
