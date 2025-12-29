using System.CommandLine;
using System.CommandLine.Parsing;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.Reflection;
using System.Text;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;

namespace CommandLineOptions
{
    /// <summary>
    /// Builds System.CommandLine options from a POCO settings class and binds parsed args back into an instance.
    /// Supports string, bool, numeric primitives and enum (nullable variants too).
    /// </summary>
    public class OptionsParser
    {
        private readonly ILogger<OptionsParser> _logger;

        [ExcludeFromCodeCoverage]
        public OptionsParser(ILogger<OptionsParser>? logger = null)
        {
            _logger = logger ?? NullLogger<OptionsParser>.Instance;
        }

        /// <summary>
        /// Build a RootCommand that exposes options for each public, instance property on <typeparamref name="TSettings"/>.
        /// Option names are kebab-cased from property names (e.g. "MyProperty" -> "--my-property").
        /// </summary>
        public RootCommand BuildRootCommand<TSettings>() where TSettings : new()
        {
            var root = new RootCommand();

            foreach (var descriptor in CreateOptionDescriptors(typeof(TSettings)))
            {
                root.Add(descriptor.Option);
            }

            return root;
        }

        /// <summary>
        /// Parse args into a new instance of <typeparamref name="TSettings"/>.
        /// Supports Dictionary<string, string> properties for parsing key=value token pairs.
        /// </summary>
        public TSettings Parse<TSettings>(string[] args) where TSettings : new()
        {
            var descriptors = CreateOptionDescriptors(typeof(TSettings));
            var root = new RootCommand();

            foreach (var descriptor in descriptors)
            {
                root.Add(descriptor.Option);
            }

            var instance = new TSettings();
            var parseResult = root.Parse(args);

            foreach (var descriptor in descriptors)
            {
                var result = parseResult.GetResult(descriptor.Option);

                if (result is not null)
                {
                    // An option was provided for this property.

                    if (descriptor.ValueType == typeof(Dictionary<string, string>))
                    {
                        var dict = ParseTokensIntoDictionary(result.Tokens);
                        descriptor.Prop.SetValue(instance, dict);
                    }
                    else
                    {
                        var parsed = Convert.ChangeType(result.GetValueOrDefault<object>(), descriptor.ValueType, CultureInfo.InvariantCulture);
                        descriptor.Prop.SetValue(instance, parsed);
                    }
                }
            }

            return instance;
        }

        //----==== PRIVATE ====--------------------------------------------------------------------

        private List<OptionDescriptor> CreateOptionDescriptors(Type settingsType)
        {
            var list = new List<OptionDescriptor>();

            foreach (var prop in settingsType.GetProperties(BindingFlags.Public | BindingFlags.Instance))
            {
                if (!prop.CanRead || !prop.CanWrite)
                {
                    continue;
                }

                var valueType = Nullable.GetUnderlyingType(prop.PropertyType) ?? prop.PropertyType;
                var kebab = ToKebabCase(prop.Name);
                var option = CreateOptionForType(valueType, kebab);

                option.Aliases.Add(kebab);

                list.Add(new OptionDescriptor(prop, option, valueType));
            }

            _logger.LogDebug("CreateOptionDescriptors<{Type}>: {Count} options", settingsType.FullName, list.Count);

            return list;
        }

        private Option CreateOptionForType(Type valueType, string kebabName)
        {
            var primary = "--" + kebabName;
            if (valueType.IsEnum)
            {
                var optionType = typeof(Option<>).MakeGenericType(valueType);
                return (Option)Activator.CreateInstance(optionType, new object[] { primary })!;
            }

            if (valueType == typeof(bool))
            {
                return new Option<bool>(primary);
            }

            if (valueType == typeof(string))
            {
                return new Option<string>(primary);
            }

            if (valueType == typeof(int))
            {
                return new Option<int>(primary);
            }

            if (valueType == typeof(long))
            {
                return new Option<long>(primary);
            }

            if (valueType == typeof(double))
            {
                return new Option<double>(primary);
            }

            if (valueType == typeof(Dictionary<string, string>))
            {
                return new Option<Dictionary<string, string>>(primary) { AllowMultipleArgumentsPerToken = true };
            }

            // Fallback: bind as string.
            return new Option<string>(primary);
        }

        private static string ToKebabCase(string name)
        {
            if (string.IsNullOrEmpty(name))
            {
                return name;
            }

            var sb = new StringBuilder(name.Length + 5);

            for (int index = 0; index < name.Length; index++)
            {
                var c = name[index];

                if (char.IsUpper(c))
                {
                    if (index > 0)
                    {
                        sb.Append('-');
                    }

                    sb.Append(char.ToLowerInvariant(c));
                }
                else
                {
                    sb.Append(c);
                }
            }

            return sb.ToString();
        }

        private static Dictionary<string, string> ParseTokensIntoDictionary(IReadOnlyList<Token> tokenList)
        {
            var dict = new Dictionary<string, string>();

            // Each Token could be a list of key=value pairs

            foreach (var tokens in tokenList)
            {
                var pairs = tokens.Value.Split(new[] { ' ', '\t' }, StringSplitOptions.RemoveEmptyEntries);

                foreach (var pair in pairs)
                {
                    var parts = pair.Split('=', 2);
                    
                    if (parts.Length == 2)
                    {
                        dict[parts[0].Trim()] = parts[1].Trim();
                    }
                }
            }

            return dict;
        }
    }
}