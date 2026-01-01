# ⚙️ Command Line Options 🍢

A command line parser that leverages Microsoft's **System.CommandLine** and binds the parsed options to an "`IOptions`" settings class.

🍢 Not using any attributes (parsing defaults to "kebab style" options)

~~~C#
public class SampleSettings {
    public int LoopCount { get; set; }
    public string Message { get; set; }
}

// sample.exe --loop-count 5 --message 'Hello World'

var cmdLineSettings = parser.Parse<SampleSettings>(args);

var loopCount = cmdLineSettings.LoopCount;    // 5
var message = cmdLineSettings.Message;        // Hello World
~~~

Override the default behavior with attributes.

~~~C#
public class SampleSettings {
    [CommandLineOption("-c", "Number of iterations to run", "-count", "--loop-count")]
    public int LoopCount { get; set; }
    [CommandLineOption("-m", "Message to display", "--message")]
    public string Message { get; set; }
}

// sample.exe -c 5 -m 'Hello World'

var cmdLineSettings = parser.Parse<SampleSettings>(args);

var loopCount = cmdLineSettings.LoopCount;    // 5
var message = cmdLineSettings.Message;        // Hello World
~~~

⚠️ NOTE: Take a look at the test files:
* 📄 [TestSimpleSettings.cs](./src/CommandLineOptions.Tests/TestSimpleSettings.cs)
* 📄 [TestSimpleSettings2.cs](./src/CommandLineOptions.Tests/TestSimpleSettings2.cs)

# 🛠️ Setup

* Installed **C# Dev Kit** (published by Microsoft)_.
  * Used ".NET: New Project" from the search bar.
* Leveraging **Github Copilot** for code generation and changes.
  * One thing it has trouble with is cleaning up **ALL** unused `using` statements.
* **Requires .NET 10 SDK.** A `global.json` file is included to pin the SDK for contributors; install .NET 10 locally or via your package manager: https://dotnet.microsoft.com/download

> **Markdown style:** Use `*` for unordered list markers and `~~~` (tilde fences) for fenced code blocks. These are enforced by `.markdownlint.json` (MD004: `asterisk`, MD048: `tilde`).

# 📝 TODO

## ✅ CI Checklist

These CI-related tasks are tracked here for visibility and can be used as a quick checklist for reviewers:

* [x] Add GitHub Actions workflow to run tests on push/PR (~github workflow created: `.github/workflows/ci.yml`~)
* [ ] Verify workflow runs tests on push/PR
* [ ] Add status badge to README
* [ ] Close issue and document CI details