using System.Collections.Concurrent;
using Microsoft.Extensions.Logging;

namespace ConsoleAppHost.Tests.Utilities
{
    internal sealed class InMemoryLoggerProvider : ILoggerProvider
    {
        private readonly ConcurrentQueue<string> _logs;
        public InMemoryLoggerProvider(ConcurrentQueue<string> logs) => _logs = logs;
        public ILogger CreateLogger(string categoryName) => new InMemoryLogger(_logs);
        public void Dispose() { }
    }
}
