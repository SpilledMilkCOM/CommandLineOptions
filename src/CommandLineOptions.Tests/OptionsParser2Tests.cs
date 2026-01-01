using Microsoft.Extensions.Logging.Abstractions;
using System.CommandLine;

namespace CommandLineOptions.Tests
{
    [TestClass]
    public class OptionsParser2Tests
    {
        public TestContext? TestContext { get; set; }

        [TestMethod]
        public void OptionsParser_BuildRootCommand_UsesAttributeForBigCount()
        {
            var parser = ConstructTestObject();
            var root = parser.BuildRootCommand<TestSimpleSettings2>();

            var big = root.Children.OfType<Option>().First(o => o.Name == "-b");

            Assert.AreEqual("A big count value", big.Description);

            var aliases = big.Aliases.ToArray();

            var aliasList = string.Join(",", aliases);

            Assert.IsTrue(aliases.Contains("-big"), $"aliases: {aliasList}");
            Assert.IsTrue(aliases.Contains("--big-count"), $"aliases: {aliasList}");
            Assert.IsFalse(aliases.Contains("big-count"), $"Kebab-case should not be auto-added when attribute exists. aliases: {aliasList}");
            Assert.AreEqual(2, aliases.Length, $"Should have only attribute-defined aliases. aliases: {aliasList}");
        }

        [TestMethod]
        public void OptionsParser_BuildRootCommand_UsesAttributeForEnabled()
        {
            var parser = ConstructTestObject();
            var root = parser.BuildRootCommand<TestSimpleSettings2>();

            var enabled = root.Children.OfType<Option>().First(o => o.Name == "-e");

            Assert.AreEqual("Enable the feature", enabled.Description);

            var aliases = enabled.Aliases.ToArray();
            var aliasList = string.Join(",", aliases);

            Assert.IsTrue(aliases.Contains("-en"), $"aliases: {aliasList}");
            Assert.IsTrue(aliases.Contains("--enabled"), $"aliases: {aliasList}");
            Assert.IsFalse(aliases.Contains("enabled"), $"Kebab-case should not be auto-added when attribute exists. aliases: {aliasList}");
            Assert.AreEqual(2, aliases.Length, $"Should have only attribute-defined aliases. aliases: {aliasList}");
        }

        [DataTestMethod]
        [DataRow("-e")]
        [DataRow("-en")]
        [DataRow("--enabled")]
        public void OptionsParser_Parse_SetsBoolProperty_WithAttribute(string optionName)
        {
            var parser = ConstructTestObject();
            var args = new[] { optionName };

            var result = parser.Parse<TestSimpleSettings2>(args);

            Assert.IsTrue(result.Enabled);
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
