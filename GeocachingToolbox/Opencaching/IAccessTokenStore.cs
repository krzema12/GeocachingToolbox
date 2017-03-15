namespace GeocachingToolbox.Opencaching
{
    public interface IAccessTokenStore
    {
        bool Populated { get;  }
        string Token { get; set; }
        string TokenSecret { get; set; }

        void SetValues(string token, string tokenSecret);
    }
}
