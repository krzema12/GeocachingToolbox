namespace GeocachingToolbox.GeocachingCom
{
    public class GCUser : User
    {
        public GCUser(string nick, int foundCount)
        {
            Name = nick;
            FoundGeocachesCount = foundCount;
        }
    }
}
