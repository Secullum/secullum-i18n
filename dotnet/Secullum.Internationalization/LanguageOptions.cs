using System.Collections.Generic;

namespace Secullum.Internationalization
{
    public class LanguageOptions
    {
        public Dictionary<string, string> Expressions { get; set; }
        public string DateTimeFormat { get; set; }
        public string DateFormat { get; set; }
        public string TimeFormat { get; set; }
    }
}
