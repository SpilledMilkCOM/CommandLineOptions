using System;

namespace CommandLineOptions.Tests
{
    public class TestSimpleSettings
    {
        public string? Name { get; set; } = "Default";
        public bool Enabled { get; set; }
        public int Count { get; set; }
        public TestEnum Color { get; set; } = TestEnum.Red;
    }
} 