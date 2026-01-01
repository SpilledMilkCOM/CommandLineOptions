namespace CommandLineOptions
{
    [AttributeUsage(AttributeTargets.Property, AllowMultiple = false, Inherited = true)]
    public sealed class CommandLineOptionAttribute : Attribute
    {
        public CommandLineOptionAttribute(string name, string? description = null, params string[] aliases)
        {
            Name = name ?? throw new ArgumentNullException(nameof(name));
            Description = description;
            Aliases = aliases ?? Array.Empty<string>();
        }

        public string Name { get; }

        public string? Description { get; }

        public IReadOnlyList<string> Aliases { get; }
    }
}
