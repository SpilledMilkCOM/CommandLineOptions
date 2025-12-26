using System;
using System.CommandLine;
using System.Reflection;

namespace CommandLineOptions
{
    internal sealed record OptionDescriptor(PropertyInfo Prop, Option Option, Type ValueType);
}
