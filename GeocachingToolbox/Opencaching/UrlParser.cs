using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GeocachingToolbox.Opencaching
{
    public class UrlParser
    {
        public static Dictionary<string, string> Parse(string url)
        {
            var parts = url.Split('&');
            var attributes = new Dictionary<string, string>();

            foreach (var part in parts)
            {
                var keyAndValue = part.Split('=');
                attributes.Add(keyAndValue[0], keyAndValue[1]);
            }

            return attributes;
        }
    }
}
