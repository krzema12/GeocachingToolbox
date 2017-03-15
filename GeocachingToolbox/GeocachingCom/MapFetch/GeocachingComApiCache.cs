using System.Runtime.Serialization;

namespace GeocachingToolbox.GeocachingCom.MapFetch
{
    [DataContract]
    public class GeocachingComApiCache
    {
        [DataMember]
        public string i { get; set; }

        [DataMember]
        public string n { get; set; }
    }
}