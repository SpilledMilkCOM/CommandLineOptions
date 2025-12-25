using System;

namespace ConsoleAppHost.Tests.Utilities
{
    internal sealed class NullScope : IDisposable
    {
        public static NullScope Instance { get; } = new NullScope();
        public void Dispose() { }
    }
}
