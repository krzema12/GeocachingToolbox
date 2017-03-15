namespace GeocachingToolbox.Opencaching
{
    public class ApiAccessKeysImpl : ApiAccessKeys
    {
        public ApiAccessKeysImpl(string consumerKey, string consumerSecret)
        {
            ConsumerKey = consumerKey;
            ConsumerSecret = consumerSecret;
        }

        public ApiAccessKeysImpl()
        {
            
        }
        public string ConsumerKey { get; }
        public string ConsumerSecret { get; }
    }
}
