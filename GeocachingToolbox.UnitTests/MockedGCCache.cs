using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using GeocachingToolbox.GeocachingCom;

namespace GeocachingToolbox.UnitTests
{
    class MockedGCCache : GCGeocache
    {
        public string Data { get; }

        public MockedGCCache(string code)
        {
            Data = ReadContent($"GeocachingCom\\WebpageContents\\{code}\\{code}.html");
        }

        public static string ReadContent(string filePath)
        {
            var assembly = Assembly.GetExecutingAssembly();
            var dottedFilePath = filePath.Replace('\\', '.');
            var resourceName = "GeocachingToolbox.UnitTests." + dottedFilePath;
            string content = "";

            try
            {
                using (Stream stream = assembly.GetManifestResourceStream(resourceName))
                using (StreamReader reader = new StreamReader(stream))
                {
                    content = reader.ReadToEnd();
                }
            }
            catch (ArgumentNullException)
            {
                throw new FileNotFoundException("Make sure that the file '" + filePath
                                                + "' exists and has 'Build Action' set to 'Embedded Resource'.");
            }
            return content;
        }


    }
}
