using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GeocachingToolbox.Opencaching
{
    public class ApiAccessKeysImpl : ApiAccessKeys
    {
        public ApiAccessKeysImpl(string consumerKey, string consumerSecret)
        {
            ConsumerKey = consumerKey;
            ConsumerSecret = consumerSecret;
        }

        public ApiAccessKeysImpl()
        {
            
        }
        public string ConsumerKey { get; }
        public string ConsumerSecret { get; }
    }
}
