using System.Text.RegularExpressions;

namespace GeocachingToolbox
{
    class TextUtils
    {
        public static string Parse(string input, string regex, int group = 1, bool trim = true,RegexOptions regexOptions = RegexOptions.IgnoreCase)
        {
            Regex r = new Regex(regex, regexOptions);
            var regexResult = r.Match(input);
            var groupValue = regexResult.Groups[group].Value;
            if (trim)
            {
                groupValue = groupValue.Trim(' ', '\n').Trim();
            }
            return groupValue;
        }
        public static string ReplaceBRTag(string str)
        {
            string Msg = Regex.Replace(str, @"<br\s*[\/]?>", "\n");
            return Msg;
        }
    }
}
