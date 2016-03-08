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
        const string SettingFile = "GeocachingToolboxSettings.xml";
        static void Main(string[] args)
        {
            // Load configuration
            var config = LoadConfiguration();

            // Geocaching.com 
            var examples = new GeocachingComExamples(config.GCLogin, config.GCPassword);

            // Opencaching
            //ApiAccessKeysImpl apiKeys = new ApiAccessKeysImpl(config.OCConsumerKey, config.OCConsumerSecret);
            //var examples = new OpencachingExamples(apiKeys);

            examples.Run();
        }

        private static GeoCachingToolboxSettings LoadConfiguration()
        {
            GeoCachingToolboxSettings config = null;
            try
            {
                config = SerializationHelper.Deserialize<GeoCachingToolboxSettings>(SettingFile);
            }
            catch (Exception)
            {
                config = new GeoCachingToolboxSettings();
                SerializationHelper.Serialize(SettingFile, config);
            }
            return config;
        }
    }
}
