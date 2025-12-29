namespace CommandLineOptions.Tests
{
    public class TestSimpleSettings
    {
        public long BigCount { get; set; }
        public bool Enabled { get; set; }
        // public TestEnum Color { get; set; } = TestEnum.Red;
        public int Count { get; set; }
        public string? Name { get; set; } = "Default";
        public double Percent { get; set; }
    }
}