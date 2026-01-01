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
* Leveraging **GitHub Copilot** for code generation and changes.
  * One thing it has trouble with is cleaning up **ALL** unused `using` statements.
  * The AI had trouble with letting the framework generate the help message.
* **Requires .NET 10 SDK.** A `global.json` file is included to pin the SDK for contributors; install .NET 10 locally or via your package manager: https://dotnet.microsoft.com/download

> **Markdown style:** Use `*` for unordered list markers and `~~~` (tilde fences) for fenced code blocks. These are enforced by `.markdownlint.json` (MD004: `asterisk`, MD048: `tilde`).

# 📝 TODO

## ✅ CI Checklist

These CI-related tasks are tracked here for visibility and can be used as a quick checklist for reviewers:

* [x] Add GitHub Actions workflow to run tests on push/PR (~github workflow created: `.github/workflows/ci.yml`~)
* [ ] Verify workflow runs tests on push/PR
* [ ] Add status badge to README
* [ ] Close issue and document CI details

# 📜 Some History

At work we've got some command-line programs _(console apps)_ and they are using "brute force" command-line parsing.  This **really** makes it hard to add options and can be fragile if you're dealing with all the _tokenizing_, etc.

I wanted to get away from the "brute force" parsing so I tried to use the default `Host` command-line parsing, but it does **NOT** handle boolean `-flag` options.  You **have** to supply a value if you want it to be **true** _(ex: `-flag true`)_.

So I decided to use `System.CommandLine` since it handles this case, but unfortunately there is **no** option binding.  I ended up creating a parser and bound it to a single settings class with the intention of making it more generic later.  But you know how it goes...  There is never enough time.

With Christmas break coming up I thought I'd create this parser from scratch and team up with **GitHub Copilot**.  All totalled up I'd say I spent 10 hours _(maybe a bit more)_ and sacrificed some Diablo play time.  I'm glad I already did the project at work that bound options to a settings class so I could guide the AI in the right direction.