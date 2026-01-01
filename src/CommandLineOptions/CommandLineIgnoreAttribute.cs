using System;

namespace CommandLineOptions
{
    [AttributeUsage(AttributeTargets.Property, AllowMultiple = false, Inherited = true)]
    public sealed class CommandLineIgnoreAttribute : Attribute
    {
    }
}
