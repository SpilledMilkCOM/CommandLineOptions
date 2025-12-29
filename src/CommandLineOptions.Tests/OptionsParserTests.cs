using Microsoft.Extensions.Logging.Abstractions;
using System.CommandLine;

namespace CommandLineOptions.Tests
{
    [TestClass]
    public class OptionsParserTests
    {
        public TestContext? TestContext { get; set; }

        [TestMethod]
        public void OptionsParser_Parse_SetsStringProperty()
        {
            var args = new[] { "--name", "Bob" };
            var parser = ConstructTestObject();

            var actual = parser.Parse<TestSimpleSettings>(args);

            Assert.AreEqual("Bob", actual.Name);
            // Defaults are preseved
            Assert.IsFalse(actual.Enabled);
            Assert.AreEqual(0, actual.Count);
            Assert.AreEqual(0L, actual.BigCount);
            Assert.AreEqual(0, actual.Percent);
        }

        [TestMethod]
        public void OptionsParser_Parse_SetsBoolProperty()
        {
            var args = new[] { "--enabled", "true" };
            var test = ConstructTestObject();

            var actual = test.Parse<TestSimpleSettings>(args);

            Assert.IsTrue(actual.Enabled);
            // Defaults are preseved
            Assert.AreEqual(0, actual.Count);
            Assert.AreEqual(0L, actual.BigCount);
            Assert.AreEqual("Default", actual.Name);
            Assert.AreEqual(0, actual.Percent);
        }

        [TestMethod]
        public void OptionsParser_Parse_SetsIntProperty()
        {
            var args = new[] { "--count", "42" };
            var test = ConstructTestObject();

            var actual = test.Parse<TestSimpleSettings>(args);

            Assert.AreEqual(42, actual.Count);
            // Defaults are preseved
            Assert.IsFalse(actual.Enabled);
            Assert.AreEqual(0L, actual.BigCount);
            Assert.AreEqual("Default", actual.Name);
            Assert.AreEqual(0, actual.Percent);
        }

        [TestMethod]
        public void OptionsParser_Parse_SetsLongProperty()
        {
            long value = 9_000_000_000;
            var args = new[] { "--big-count", value.ToString() };
            var test = ConstructTestObject();

            var actual = test.Parse<TestSimpleSettings>(args);

            Assert.AreEqual(value, actual.BigCount);
            // Defaults are preseved
            Assert.IsFalse(actual.Enabled);
            Assert.AreEqual(0, actual.Count);
            Assert.AreEqual("Default", actual.Name);
            Assert.AreEqual(0, actual.Percent);
        }

        [TestMethod]
        public void OptionsParser_Parse_SetsDoubleProperty()
        {
            var args = new[] { "--percent", "95.5" };
            var test = ConstructTestObject();

            var actual = test.Parse<TestSimpleSettings>(args);

            Assert.AreEqual(95.5, actual.Percent);
            // Defaults are preseved
            Assert.AreEqual(0, actual.Count);
            Assert.IsFalse(actual.Enabled);
            Assert.AreEqual(0L, actual.BigCount);
            Assert.AreEqual("Default", actual.Name);
        }

        // [TestMethod]
        // public void OptionsParser_Parse_SetsEnumProperty()
        // {
        //     var args = new[] { "--color", "Blue" };
        //     var test = ConstructTestObject();

        //     var actual = test.Parse<TestSimpleSettings>(args);

        //     Assert.AreEqual(TestEnum.Blue, actual.Color);
        // }

        [TestMethod]
        public void OptionsParser_Parse_UsesDefaultsWhenNoArgs()
        {
            var test = ConstructTestObject();

            var actual = test.Parse<TestSimpleSettings>(Array.Empty<string>());

            Assert.AreEqual("Default", actual.Name);
            Assert.IsFalse(actual.Enabled);
            Assert.AreEqual(0, actual.Count);
            Assert.AreEqual(0L, actual.BigCount);
            Assert.AreEqual(0, actual.Percent);
            // Assert.AreEqual(TestEnum.Red, actual.Color);
        }

        [TestMethod]
        public void OptionsParser_BuildRootCommand_ContainsExpectedOptionAliases()
        {
            var parser = ConstructTestObject();
            var root = parser.BuildRootCommand<TestSimpleSettings>();

            Console.WriteLine("RootCommand methods: " + string.Join(',', typeof(RootCommand).GetMethods(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance).Select(m => m.Name).Distinct()));

            var optionsList = root.Children.OfType<Option>().ToList();
            var aliases = optionsList.SelectMany(o => o.Aliases).ToArray();

            Assert.IsTrue(aliases.Length > 0, "No options were created; aliases empty.");

            var aliasList = string.Join(",", aliases);

            Console.WriteLine("options.debug:" + optionsList.Count);
            Console.WriteLine("aliases=" + aliasList);
            Console.WriteLine("option-aliases=" + string.Join('|', optionsList.Select(o => string.Join('/', o.Aliases))));
            Assert.IsTrue(aliases.Contains("--name") || aliases.Contains("name"), $"Missing --name, actual aliases: {aliasList}");
            Assert.IsTrue(aliases.Contains("--enabled") || aliases.Contains("enabled"), $"Missing --enabled, actual aliases: {aliasList}");
            Assert.IsTrue(aliases.Contains("--count") || aliases.Contains("count"), $"Missing --count, actual aliases: {aliasList}");
            Assert.IsTrue(aliases.Contains("--big-count") || aliases.Contains("big-count"), $"Missing --big-count, actual aliases: {aliasList}");
            Assert.IsTrue(aliases.Contains("--percent") || aliases.Contains("percent"), $"Missing --percent, actual aliases: {aliasList}");
            // Assert.IsTrue(aliases.Contains("--color") || aliases.Contains("color"), $"Missing --color, actual aliases: {aliasList}");
        }

        [TestMethod]
        public void OptionsParser_ReflectionSeesProperties()
        {
            var props = typeof(TestSimpleSettings).GetProperties(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance);

            Assert.AreEqual(5, props.Length, "Expected 5 public instance properties on TestSimpleSettings.");
        }

        //----==== PRIVATE ====--------------------------------------------------------------------

        private OptionsParser ConstructTestObject() => new OptionsParser(NullLogger<OptionsParser>.Instance);
    }
}