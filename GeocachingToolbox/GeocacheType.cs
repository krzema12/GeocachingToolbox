using System;

namespace GeocachingToolbox
{
    public enum GeocacheType
    {
        [GCGeocacheType("")]
        Unknown,
        [GCGeocacheType("32bc9333-5e52-4957-b0f6-5a2c8fc7b257")]
        Traditional,
        [GCGeocacheType("40861821-1835-4e11-b666-8d41064d03fe")]
        Mystery,
        [GCGeocacheType("a5f6d0ad-d2f2-4011-8c14-940a9ebf3c74")]
        Multicache,
        [GCGeocacheType("0544fa55-772d-4e5c-96a9-36a51ebcf5c9")]
        Whereigo,
        [GCGeocacheType("4bdd8fb2-d7bc-453f-a9c5-968563b15d24")]
        LetterboxHybrid,
        [GCGeocacheType("c66f5cf3-9523-4549-b8dd-759cd2f18db8")]
        Earthcache
    }

    public class GCGeocacheTypeAttribute : Attribute
    {
        public string GUID;
        public GCGeocacheTypeAttribute(string guid) { GUID = guid; }
    }
}
