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
                var attr = prop.GetCustomAttribute<CommandLineOptionAttribute>();
                var kebab = ToKebabCase(prop.Name);

                var optionName = NormalizeOptionName(attr?.Name ?? "--" + kebab);
                var option = CreateOptionForType(valueType, optionName);
                option.Description = attr?.Description;

                option.Aliases.Add(kebab);

                if (attr?.Aliases is not null)
                {
                    foreach (var alias in attr.Aliases)
                    {
                        if (string.IsNullOrWhiteSpace(alias))
                        {
                            continue;
                        }

                        option.Aliases.Add(NormalizeOptionName(alias));
                    }
                }

                list.Add(new OptionDescriptor(prop, option, valueType));
            }

            _logger.LogDebug("CreateOptionDescriptors<{Type}>: {Count} options", settingsType.FullName, list.Count);

            return list;
        }

        private Option CreateOptionForType(Type valueType, string optionName)
        {
            if (valueType.IsEnum)
            {
                var optionType = typeof(Option<>).MakeGenericType(valueType);
                return (Option)Activator.CreateInstance(optionType, new object[] { optionName })!;
            }

            if (valueType == typeof(bool))
            {
                return new Option<bool>(optionName);
            }

            if (valueType == typeof(string))
            {
                return new Option<string>(optionName);
            }

            if (valueType == typeof(int))
            {
                return new Option<int>(optionName);
            }

            if (valueType == typeof(long))
            {
                return new Option<long>(optionName);
            }

            if (valueType == typeof(double))
            {
                return new Option<double>(optionName);
            }

            if (valueType == typeof(Dictionary<string, string>))
            {
                return new Option<Dictionary<string, string>>(optionName) { AllowMultipleArgumentsPerToken = true };
            }

            // Fallback: bind as string.
            return new Option<string>(optionName);
        }

        private static string NormalizeOptionName(string raw)
        {
            if (string.IsNullOrWhiteSpace(raw))
            {
                throw new ArgumentException("Option name cannot be null or whitespace", nameof(raw));
            }

            var trimmed = raw.Trim();

            if (trimmed.StartsWith('-'))
            {
                return trimmed;
            }

            return "--" + trimmed;
        }

        private Dictionary<string, string> ParseTokensIntoDictionary(IReadOnlyList<Token> tokenList)
        {
            var dict = new Dictionary<string, string>();

            foreach (var tokens in tokenList)
            {
                var pairs = tokens.Value.Split(new[] { ' ', '\t' }, StringSplitOptions.RemoveEmptyEntries);

                foreach (var pair in pairs)
                {
                    var idx = pair.IndexOf('=');

                    if (idx <= 0 || idx == pair.Length - 1)
                    {
                        _logger.LogWarning("Ignoring invalid key-value pair with key only: '{Pair}'", pair);
                        continue;
                    }

                    var key = pair.Substring(0, idx).Trim();
                    var value = pair.Substring(idx + 1).Trim();

                    if (key.Length > 0)
                    {
                        dict[key] = value;
                    } else {
                        _logger.LogWarning("Ignoring invalid key-value pair with empty key: '{Pair}'", pair);
                    }
                }
            }

            return dict;
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
    }
}