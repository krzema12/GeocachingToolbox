namespace GeocachingToolbox
{
    public abstract class User
    {
        public string Name { get; protected set; }
        public int FoundGeocachesCount { get; protected set; }
    }
}
