using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Reflection;
using System.Text;
using System.Text.RegularExpressions;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace Secullum.Internationalization
{
    public class Translator
    {
        private static Dictionary<string, Dictionary<string, string>> expressionsByLanguage = new Dictionary<string, Dictionary<string, string>>();
        private static Dictionary<string, string> dateTimeFormatsByLanguage = new Dictionary<string, string>();
        private static Dictionary<string, string> dateFormatsByLanguage = new Dictionary<string, string>();
        private static Dictionary<string, string> timeFormatsByLanguage = new Dictionary<string, string>();
        private static Regex regexPlaceholder = new Regex(@"\{(\d)\}", RegexOptions.Compiled);

        public static void AddResource(string language, string resourceName, Assembly assembly)
        {
            using (var resourceStream = assembly.GetManifestResourceStream(resourceName))
            using (var streamReader = new StreamReader(resourceStream, Encoding.UTF8))
            {
                var resourceContent = JsonConvert.DeserializeObject<JObject>(streamReader.ReadToEnd());

                dateTimeFormatsByLanguage[language] = resourceContent.Value<string>("dateTimeFormat");
                dateFormatsByLanguage[language] = resourceContent.Value<string>("dateFormat");
                timeFormatsByLanguage[language] = resourceContent.Value<string>("timeFormat");
                expressionsByLanguage[language] = new Dictionary<string, string>();

                foreach (JProperty expression in resourceContent["expressions"])
                {
                    expressionsByLanguage[language].Add(expression.Name, expression.Value.ToString());
                }
            }
        }

        public static void AddResource(string language, string resourceName)
        {
            AddResource(language, resourceName, Assembly.GetEntryAssembly());
        }

        public static string Translate(string expression, params object[] args)
        {
            var translatedExpresssion = expression;
            var expressions = expressionsByLanguage[GetCurrentLanguageKey()];

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

        private static string GetCurrentLanguageKey()
        {
            return CultureInfo.CurrentCulture.Name.Substring(0, 2).ToLower();
        }
    }
}
