using CommandLineOptions;

namespace ConsoleAppHost
{
    public sealed class ApplicationSettings
    {
        /// <summary>
        /// Configuration section name for <see cref="ApplicationSettings"/>.
        /// </summary>
        public const string SECTION_NAME = nameof(ApplicationSettings);

        [CommandLineOption("-c", "Number of times to loop (0 = infinite)", "--count", "--loop-count")]
        public int LoopCount { get; set; }

        [CommandLineOption("-m", "Custom message to display", "--message")]
        public string? Message { get; set; } = "Hello from ApplicationSettings Class!";

        [CommandLineOption("-v", "Enable verbose logging", "--verbose")]
        public bool Verbose { get; set; }
    }
}
