using System;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using CommandLineOptions;

namespace CommandLineOptions.Tests
{
    [TestClass]
    public class OptionsParserTests
    {

        [TestMethod]
        public void Parse_SetsStringProperty()
        {
            var args = new[] { "--name", "Bob" };
            var s = OptionsParser.Parse<TestSimpleSettings>(args);
            Assert.AreEqual("Bob", s.Name);
        }

        [TestMethod]
        public void Parse_SetsBoolProperty()
        {
            var args = new[] { "--enabled", "true" };
            var s = OptionsParser.Parse<TestSimpleSettings>(args);
            Assert.IsTrue(s.Enabled);
        }

        [TestMethod]
        public void Parse_SetsIntProperty()
        {
            var args = new[] { "--count", "42" };
            var s = OptionsParser.Parse<TestSimpleSettings>(args);
            Assert.AreEqual(42, s.Count);
        }

        [TestMethod]
        public void Parse_SetsEnumProperty()
        {
            var args = new[] { "--color", "Blue" };
            var s = OptionsParser.Parse<TestSimpleSettings>(args);
            Assert.AreEqual(TestEnum.Blue, s.Color);
        }

        [TestMethod]
        public void Parse_UsesDefaultsWhenNoArgs()
        {
            var s = OptionsParser.Parse<TestSimpleSettings>(Array.Empty<string>());
            Assert.AreEqual("Default", s.Name);
            Assert.IsFalse(s.Enabled);
            Assert.AreEqual(0, s.Count);
            Assert.AreEqual(TestEnum.Red, s.Color);
        }

        [TestMethod]
        public void BuildRootCommand_ContainsExpectedOptionAliases()
        {
            var root = OptionsParser.BuildRootCommand<TestSimpleSettings>();
            var aliases = root.Options.SelectMany(o => o.Aliases).ToArray();
            CollectionAssert.Contains(aliases, "--name");
            CollectionAssert.Contains(aliases, "--enabled");
            CollectionAssert.Contains(aliases, "--count");
            CollectionAssert.Contains(aliases, "--color");
        }
    }
}