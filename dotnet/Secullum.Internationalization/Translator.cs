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
        private static Dictionary<string, string> dateFormatsByLanguage = new Dictionary<string, string>();
        private static Regex regexPlaceholder = new Regex(@"\{(\d)\}", RegexOptions.Compiled);

        public static void AddResource(string language, string resourceName)
        {
            using (var resourceStream = Assembly.GetEntryAssembly().GetManifestResourceStream(resourceName))
            using (var streamReader = new StreamReader(resourceStream, Encoding.UTF8))
            {
                var resourceContent = JsonConvert.DeserializeObject<JObject>(streamReader.ReadToEnd());

                dateFormatsByLanguage[language] = resourceContent.Value<string>("dateFormat");
                expressionsByLanguage[language] = new Dictionary<string, string>();

                foreach (JProperty expression in resourceContent["expressions"])
                {
                    expressionsByLanguage[language].Add(expression.Name, expression.Value.ToString());
                }
            }
        }

        public static string Translate(string expression, params string[] args)
        {
            var translatedExpresssion = expression;

            if (expressionsByLanguage[GetCurrentLanguageKey()].ContainsKey(expression))
            {
                translatedExpresssion = expressionsByLanguage[GetCurrentLanguageKey()][expression];
            }

            return regexPlaceholder.Replace(translatedExpresssion, match => {
                var argIndex = int.Parse(match.Groups[1].Value);
                return args[argIndex];
            });
        }

        public static string GetDateFormat()
        {
            return dateFormatsByLanguage[GetCurrentLanguageKey()];
        }

        private static string GetCurrentLanguageKey()
        {
            return CultureInfo.CurrentCulture.Name.Substring(0, 2).ToLower();
        }
    }
}
