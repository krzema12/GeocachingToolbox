using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Examples
{
    public class GeoCachingToolboxSettings
    {
        public string OCConsumerKey { get; set; }
        public string OCConsumerSecret { get; set; }
        public string OCToken { get; set; }
        public string OCTokenSecret { get; set; }
        public string GCLogin { get; set; }
        public string GCPassword { get; set; }

        public GeoCachingToolboxSettings()
        {
            OCConsumerKey = "insert your consumer key";
            OCConsumerSecret = "insert your consumer secret";
            GCLogin = "insert your geocaching.com login";
            GCPassword = "insert your geocaching.com password";
        }
    }
}
