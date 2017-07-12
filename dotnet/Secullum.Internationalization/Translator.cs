using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Reflection;
using System.Text;
using System.Text.RegularExpressions;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;

namespace Secullum.Internationalization
{
    public class Translator
    {
        private static Dictionary<string, Dictionary<string, string>> caseSensitiveExpressionsByLanguage = new Dictionary<string, Dictionary<string, string>>();
        private static Dictionary<string, Dictionary<string, string>> caseInsensitiveExpressionsByLanguage = new Dictionary<string, Dictionary<string, string>>();

        private static Dictionary<string, LanguageOptions> optionsByLanguage = new Dictionary<string, LanguageOptions>();
        private static Dictionary<string, string> jsonOptionsByLanguage = new Dictionary<string, string>();
        private static JsonSerializerSettings jsonSettings = new JsonSerializerSettings();

        private static Regex regexPlaceholder = new Regex(@"\{(\d)\}", RegexOptions.Compiled);

        static Translator()
        {
            jsonSettings.ContractResolver = new CamelCasePropertyNamesContractResolver()
            {
                NamingStrategy = new CamelCaseNamingStrategy() { ProcessDictionaryKeys = false }
            };

            jsonSettings.Formatting = Formatting.None;
        }

        public static void AddLanguageFromResource(string resourceName)
        {
            AddLanguageFromResource(resourceName, Assembly.GetEntryAssembly());
        }

        public static void AddLanguageFromResource(string resourceName, Assembly assembly)
        {
            using (var resourceStream = assembly.GetManifestResourceStream(resourceName))
            using (var streamReader = new StreamReader(resourceStream, Encoding.UTF8))
            {
                var resourceContent = streamReader.ReadToEnd();
                var languageOptions = JsonConvert.DeserializeObject<LanguageOptions>(resourceContent, jsonSettings);

                AddLanguage(languageOptions);
            }
        }

        public static void AddLanguage(LanguageOptions options)
        {
            optionsByLanguage[options.Language] = options;
            jsonOptionsByLanguage[options.Language] = JsonConvert.SerializeObject(options, jsonSettings);

            caseSensitiveExpressionsByLanguage[options.Language] = new Dictionary<string, string>();
            caseInsensitiveExpressionsByLanguage[options.Language] = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);

            foreach (var expression in options.Expressions)
            {
                caseSensitiveExpressionsByLanguage[options.Language].Add(expression.Key, expression.Value);
                caseInsensitiveExpressionsByLanguage[options.Language].Add(expression.Key, expression.Value);
            }
        }

        public static string Translate(string expression, params object[] args)
        {
            return Translate(expression, ignoreCase: false, args: args);
        }

        public static string Translate(string expression, bool ignoreCase, params object[] args)
        {
            var expressions = ignoreCase
                ? caseInsensitiveExpressionsByLanguage[GetCurrentLanguageKey()]
                : caseSensitiveExpressionsByLanguage[GetCurrentLanguageKey()];

            var translatedExpresssion = expression;

            if (expressions.ContainsKey(expression) && !string.IsNullOrEmpty(expressions[expression]))
            {
                translatedExpresssion = expressions[expression];
            }

            return regexPlaceholder.Replace(translatedExpresssion, match => {
                var argIndex = int.Parse(match.Groups[1].Value);
                return args[argIndex].ToString();
            });
        }

        public static string GetDateTimeFormat()
        {
            return optionsByLanguage[GetCurrentLanguageKey()].DateTimeFormat;
        }

        public static string GetDateFormat()
        {
            return optionsByLanguage[GetCurrentLanguageKey()].DateFormat;
        }

        public static string GetTimeFormat()
        {
            return optionsByLanguage[GetCurrentLanguageKey()].TimeFormat;
        }

        public static string GetDayMonthFormat()
        {
            return optionsByLanguage[GetCurrentLanguageKey()].DayMonthFormat;
        }

        public static LanguageOptions GetLanguageOptions()
        {
            return optionsByLanguage[GetCurrentLanguageKey()];
        }

        public static string GetLanguageOptionsAsJson()
        {
            return jsonOptionsByLanguage[GetCurrentLanguageKey()];
        }

        private static string GetCurrentLanguageKey()
        {
            return CultureInfo.CurrentCulture.Name.Substring(0, 2).ToLower();
        }
    }
}
