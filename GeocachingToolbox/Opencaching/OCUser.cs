namespace GeocachingToolbox.Opencaching
{
    public class OCUser : User
    {
        public string Uuid { get; protected set; }

        public OCUser(string nick, int cachesFound, string uuid)
        {
            Name = nick;
            FoundGeocachesCount = cachesFound;
            Uuid = uuid;
        }
    }
}
