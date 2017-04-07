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

        private static Dictionary<string, string> dateTimeFormatsByLanguage = new Dictionary<string, string>();
        private static Dictionary<string, string> dateFormatsByLanguage = new Dictionary<string, string>();
        private static Dictionary<string, string> timeFormatsByLanguage = new Dictionary<string, string>();

        private static Regex regexPlaceholder = new Regex(@"\{(\d)\}", RegexOptions.Compiled);

        static Translator()
        {
            jsonSettings.ContractResolver = new CamelCasePropertyNamesContractResolver();
            jsonSettings.Formatting = Formatting.None;
        }

        public static void AddLanguage(string language, LanguageOptions options)
        {
            optionsByLanguage[language] = options;
            jsonOptionsByLanguage[language] = JsonConvert.SerializeObject(options, jsonSettings);

            dateTimeFormatsByLanguage[language] = options.DateTimeFormat;
            dateFormatsByLanguage[language] = options.DateFormat;
            timeFormatsByLanguage[language] = options.TimeFormat;

            caseSensitiveExpressionsByLanguage[language] = new Dictionary<string, string>();
            caseInsensitiveExpressionsByLanguage[language] = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);

            foreach (var expression in options.Expressions)
            {
                caseSensitiveExpressionsByLanguage[language].Add(expression.Key, expression.Value);
                caseInsensitiveExpressionsByLanguage[language].Add(expression.Key, expression.Value);
            }
        }

        public static void AddLanguageFromResource(string language, string resourceName)
        {
            AddLanguageFromResource(language, resourceName, Assembly.GetEntryAssembly());
        }

        public static void AddLanguageFromResource(string language, string resourceName, Assembly assembly)
        {
            using (var resourceStream = assembly.GetManifestResourceStream(resourceName))
            using (var streamReader = new StreamReader(resourceStream, Encoding.UTF8))
            {
                var resourceContent = streamReader.ReadToEnd();
                var languageOptions = JsonConvert.DeserializeObject<LanguageOptions>(resourceContent, jsonSettings);

                AddLanguage(language, languageOptions);
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

            if (expressions.ContainsKey(expression) && expressions[expression] != string.Empty)
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
            return dateTimeFormatsByLanguage[GetCurrentLanguageKey()];
        }

        public static string GetDateFormat()
        {
            return dateFormatsByLanguage[GetCurrentLanguageKey()];
        }

        public static string GetTimeFormat()
        {
            return timeFormatsByLanguage[GetCurrentLanguageKey()];
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
