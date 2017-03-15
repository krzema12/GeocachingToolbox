namespace GeocachingToolbox.Opencaching
{
    public class AccessTokenStore : IAccessTokenStore
    {
        public bool Populated => !string.IsNullOrEmpty(TokenSecret) && !string.IsNullOrEmpty(Token);
        public string Token { get; set; }
        public string TokenSecret { get; set; }

        public AccessTokenStore(string token,string tokenSecret)
        {
            Token = token;
            TokenSecret = tokenSecret;
        }

        public AccessTokenStore()
        {
        }

        public void SetValues(string token, string tokenSecret)
        {
            Token = token;
            TokenSecret = tokenSecret;
        }
    }
}
