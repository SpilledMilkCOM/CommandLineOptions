using Microsoft.Extensions.Logging.Abstractions;
using System.CommandLine;

namespace CommandLineOptions.Tests
{
    [TestClass]
    public class OptionsParser2Tests
    {
        public TestContext? TestContext { get; set; }

        [TestMethod]
        public void OptionsParser_BuildRootCommand_UsesAttributeMetadata()
        {
            var parser = ConstructTestObject();
            var root = parser.BuildRootCommand<TestSimpleSettings2>();

            var big = root.Children.OfType<Option>().First(o => o.Aliases.Contains("--big-count"));

            Assert.AreEqual("A big count value", big.Description);
            Assert.AreEqual("-b", big.Name);

            var aliases = big.Aliases.ToArray();

            var aliasList = string.Join(",", aliases);

            Assert.IsTrue(aliases.Contains("-big"), $"aliases: {aliasList}");
            Assert.IsTrue(aliases.Contains("--big-count"), $"aliases: {aliasList}");
            Assert.IsFalse(aliases.Contains("big-count"), $"Kebab-case should not be auto-added when attribute exists. aliases: {aliasList}");
        }

        [TestMethod]
        [DataRow("-b", 9_000_000_000)]
        [DataRow("-big", 9_000_000_000)]
        [DataRow("--big-count", 9_000_000_000)]
        public void OptionsParser_Parse_SetsLongProperty_WithAttribute(string arguments, long expectedValue)
        {
            var args = new[] { arguments, expectedValue.ToString() };
            var test = ConstructTestObject();

            var actual = test.Parse<TestSimpleSettings2>(args);

            Assert.AreEqual(expectedValue, actual.BigCount);
            // Defaults are preseved
            Assert.IsFalse(actual.Enabled);
            Assert.AreEqual(0, actual.Count);
            Assert.AreEqual("Should be ignored", actual.Ignored);
            Assert.AreEqual("Default", actual.Name);
            Assert.AreEqual(0, actual.Percent);
            Assert.AreEqual(TestEnum.Red, actual.Color);
            Assert.AreEqual(0, actual.KeyValue.Count);
        }

        //----==== PRIVATE ====--------------------------------------------------------------------

        private OptionsParser ConstructTestObject() => new OptionsParser(NullLogger<OptionsParser>.Instance);
    }
}
