using System.Collections.Generic;
using System.Runtime.Serialization;

namespace GeocachingToolbox.GeocachingCom.MapFetch
{
    [DataContract]
    public class GeocachingComApiCaches
    {
        [DataMember]
        public string[] grid { get; set; }

        [DataMember]
        public string[] keys { get; set; }

        [DataMember]
        public Dictionary<string, GeocachingComApiCache[]> data { get; set; }
    }
}