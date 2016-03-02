using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GeocachingToolbox.Opencaching;

namespace Examples
{

    class Program
    {
        static void Main(string[] args)
        {
            // Load configuration
            var config = LoadConfiguration();

            // Geocaching.com 
            //var examples = new GeocachingComExamples(config.GCLogin, config.GCPassword);

            // Opencaching
            ApiAccessKeysImpl apiKeys = new ApiAccessKeysImpl(config.OCConsumerKey, config.OCConsumerSecret);
            var examples = new OpencachingExamples(apiKeys);

            examples.Run();
        }

        private static GeoCachingToolboxConfig LoadConfiguration()
        {
            GeoCachingToolboxConfig config = null;
            try
            {
                config = SerializationHelper.Deserialize<GeoCachingToolboxConfig>("Config.xml");
            }
            catch (Exception)
            {
                config = new GeoCachingToolboxConfig();
                SerializationHelper.Serialize("Config.xml", config);
            }
            return config;
        }
    }

    public class GeoCachingToolboxConfig
    {
        public string OCConsumerKey { get; set; }
        public string OCConsumerSecret { get; set; }
        public string GCLogin { get; set; }
        public string GCPassword { get; set; }

        public GeoCachingToolboxConfig()
        {
            OCConsumerKey = "insert your consumer key";
            OCConsumerSecret = "insert your consumer secret";
            GCLogin = "insert your geocaching.com login";
            GCPassword = "insert your geocaching.com password";
        }
    }

}
