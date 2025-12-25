namespace ConsoleAppHost
{
    public sealed class ApplicationSettings
    {
        /// <summary>
        /// Configuration section name for <see cref="ApplicationSettings"/>.
        /// </summary>
        public const string SECTION_NAME = nameof(ApplicationSettings);

        public bool Verbose { get; set; }
        public string? Message { get; set; } = "Hello from Host!";
    }
}
