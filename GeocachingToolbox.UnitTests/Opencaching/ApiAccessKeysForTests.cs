using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GeocachingToolbox.Opencaching
{
    class ApiAccessKeysForTests : ApiAccessKeys
    {
        public string ConsumerKey
        {
            get
            {
                return "ConsumerKeyForTests";
            }
        }

        public string ConsumerSecret
        {
            get
            {
                return "ConsumerSecretForTests";
            }
        }
    }
}
