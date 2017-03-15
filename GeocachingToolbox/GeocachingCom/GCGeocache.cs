namespace GeocachingToolbox.GeocachingCom
{
    public class GCGeocache : Geocache
    {
        public string DetailsUrl { get; set; }
        public bool IsPremium { get; set; }

        public GCGeocache(string code)
        {
            Code = code;
        }

        public GCGeocache()
        {
            
        }
    }
}
