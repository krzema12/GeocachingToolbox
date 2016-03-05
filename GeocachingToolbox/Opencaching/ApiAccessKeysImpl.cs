using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GeocachingToolbox.Opencaching
{
	public class ApiAccessKeysImpl : ApiAccessKeys
	{
        public ApiAccessKeysImpl()
        {

        }
        public ApiAccessKeysImpl(string consumerKey, string consumerSecret)
        {
            ConsumerKey = consumerKey;
            ConsumerSecret = consumerSecret;
        }

        // Paste your consumer key and consumer secret here to start developing! See http://opencaching.pl/okapi/signup.html.
        public string ConsumerKey { get; }
		public string ConsumerSecret { get; }
	}
}
