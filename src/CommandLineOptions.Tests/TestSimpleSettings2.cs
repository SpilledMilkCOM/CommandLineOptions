namespace CommandLineOptions.Tests
{
    public class TestSimpleSettings2
    {

        // If a property has CommandLineOptionAttribute, use that to define the option.

        [CommandLineOption("-b", "A big count value", "-big", "--big-count")]
        public long BigCount { get; set; }

        public TestEnum Color { get; set; } = TestEnum.Red;

        public int Count { get; set; }

        [CommandLineOption("-e", "Enable the feature", "-en", "--enabled")]
        public bool Enabled { get; set; }

        [CommandLineIgnore]
        public string Ignored { get; set; } = "Should be ignored";

        public Dictionary<string, string> KeyValue { get; set; } = [];

        public string? Name { get; set; } = "Default";

        public double Percent { get; set; }
    }
}