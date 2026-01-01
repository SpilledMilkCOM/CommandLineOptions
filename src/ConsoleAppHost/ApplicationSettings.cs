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

        /// <summary>
        /// Applies command-line overrides to this instance, only if they differ from defaults.
        /// </summary>
        /// <param name="commandLineSettings">The settings parsed from command-line arguments.</param>
        public void ApplyCommandLineOverrides(ApplicationSettings commandLineSettings)
        {
            var defaultSettings = new ApplicationSettings();

            if (commandLineSettings.LoopCount != defaultSettings.LoopCount)
            {
                LoopCount = commandLineSettings.LoopCount;
            }

            if (commandLineSettings.Message != defaultSettings.Message)
            {
                Message = commandLineSettings.Message;
            }

            if (commandLineSettings.Verbose != defaultSettings.Verbose)
            {
                Verbose = commandLineSettings.Verbose;
            }
        }
    }
}
