# ⚙️ Command Line Options

A command line parser that leverages Microsoft's **System.CommandLine** and binds the parsed options to an `IOptions` settings class.

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

# 🔧 Running the ConsoleAppHost

You can run the console host with configuration coming from `appsettings.json`, environment variables, and command-line arguments. Examples:

* Run with defaults from `appsettings.json`:

  ~~~bash
  dotnet run --project src/ConsoleAppHost
  ~~~

* Override settings from the command line:

  ~~~bash
  dotnet run --project src/ConsoleAppHost -- --ApplicationSettings:Message "Hi from CLI" --ApplicationSettings:Verbose true
  ~~~

* Or set environment variables (PowerShell example):

  ~~~powershell
  $Env:ApplicationSettings__Message = 'Hi from Env'; dotnet run --project src/ConsoleAppHost
  ~~~