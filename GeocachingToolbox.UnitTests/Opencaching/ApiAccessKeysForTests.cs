namespace GeocachingToolbox.Opencaching
{
    class ApiAccessKeysForTests : ApiAccessKeys
    {
        public string ConsumerKey
        {
            get
            {
                return "ConsumerKeyForTests";
            }
        }

        public string ConsumerSecret
        {
            get
            {
                return "ConsumerSecretForTests";
            }
        }
    }
}
