using System.CommandLine;
using System.Globalization;
using System.Reflection;

namespace CommandLineOptions
{
    /// <summary>
    /// Builds System.CommandLine options from a POCO settings class and binds parsed args back into an instance.
    /// Supports string, bool, numeric primitives and enum (nullable variants too).
    /// </summary>
    public static class OptionsParser
    {
        /// <summary>
        /// Build a RootCommand that exposes options for each public, instance property on <typeparamref name="TSettings"/>.
        /// Option names are kebab-cased from property names (e.g. "MyProperty" -> "--my-property").
        /// </summary>
        public static RootCommand BuildRootCommand<TSettings>() where TSettings : new()
        {
            var root = new RootCommand();

            foreach (var prop in typeof(TSettings).GetProperties(BindingFlags.Public | BindingFlags.Instance))
            {
                if (!prop.CanRead || !prop.CanWrite)
                {
                    continue; // only bind read/write properties
                }

                var alias = "--" + ToKebabCase(prop.Name);
                var option = CreateOptionForProperty(prop, alias);
                root.Add(option);
            }

            return root;
        }

        /// <summary>
        /// Parse args into a new instance of <typeparamref name="TSettings"/>.
        /// </summary>
        public static TSettings Parse<TSettings>(string[] args) where TSettings : new()
        {
            var root = BuildRootCommand<TSettings>();
            var result = root.Parse(args);

            var instance = new TSettings();

            foreach (var prop in typeof(TSettings).GetProperties(BindingFlags.Public | BindingFlags.Instance))
            {
                if (!prop.CanWrite)
                {
                    continue;
                }

                var alias = "--" + ToKebabCase(prop.Name);
                var option = root.Options.FirstOrDefault(o => o.Aliases.Contains(alias, StringComparer.Ordinal));
                if (option is null)
                {
                    continue;
                }

                var value = GetValueForOption(result, option);
                if (value is null)
                {
                    continue;
                }

                var targetType = Nullable.GetUnderlyingType(prop.PropertyType) ?? prop.PropertyType;

                if (targetType.IsEnum)
                {
                    var enumVal = Enum.Parse(targetType, value.ToString()!, true);
                    prop.SetValue(instance, enumVal);
                }
                else
                {
                    var converted = Convert.ChangeType(value, targetType, CultureInfo.InvariantCulture);
                    prop.SetValue(instance, converted);
                }
            }

            return instance;
        }

        //----==== PRIVATE ====------------------------------------------------------------------------

        private static Option CreateOptionForProperty(PropertyInfo prop, string alias)
        {
            var propType = prop.PropertyType;
            var underlying = Nullable.GetUnderlyingType(propType) ?? propType;

            if (underlying == typeof(bool))
            {
                return new Option<bool>(alias);
            }

            if (underlying == typeof(string))
            {
                return new Option<string>(alias);
            }

            if (underlying == typeof(int))
            {
                return new Option<int>(alias);
            }

            if (underlying == typeof(long))
            {
                return new Option<long>(alias);
            }

            if (underlying == typeof(double))
            {
                return new Option<double>(alias);
            }

            if (underlying.IsEnum)
            {
                // Create Option<EnumType> via reflection
                var optionType = typeof(Option<>).MakeGenericType(underlying);
                return (Option)Activator.CreateInstance(optionType, new object[] { alias })!;
            }

            // Fallback to string option.
            return new Option<string>(alias);
        }

        private static object? GetValueForOption(ParseResult result, Option option)
        {
            var optionType = option.GetType();
            if (!optionType.IsGenericType)
            {
                return null;
            }

            var genericArg = optionType.GetGenericArguments()[0];
            var method = typeof(ParseResult).GetMethods(BindingFlags.Public | BindingFlags.Instance)
                .FirstOrDefault(m => m.Name == "GetValueForOption" && m.IsGenericMethod && m.GetGenericArguments().Length == 1);

            if (method is null)
            {
                return null;
            }
            var generic = method.MakeGenericMethod(genericArg);
            return generic.Invoke(result, new object[] { option });
        }

        private static string ToKebabCase(string name)
        {
            if (string.IsNullOrEmpty(name))
            {
                return name;
            }

            var chars = new List<char>(name.Length + 5);

            for (int i = 0; i < name.Length; i++)
            {
                var c = name[i];

                if (char.IsUpper(c))
                {
                    if (i > 0)
                    {
                        chars.Add('-');
                    }

                    chars.Add(char.ToLowerInvariant(c));
                }
                else
                {
                    chars.Add(c);
                }
            }

            return new string(chars.ToArray());
        }
    }
}
