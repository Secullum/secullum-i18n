using System.Collections.Generic;

namespace Secullum.Internationalization
{
    public class LanguageOptions
    {
        public string Language { get; set; }
        public string DateTimeFormat { get; set; }
        public string DateFormat { get; set; }
        public string TimeFormat { get; set; }
        public string DayMonthFormat { get; set; }
        public Dictionary<string, string> Expressions { get; set; }
    }
}
