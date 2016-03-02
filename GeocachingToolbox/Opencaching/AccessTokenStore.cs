using PCLStorage;

namespace GeocachingToolbox.Opencaching
{
    public class AccessTokenStore : IAccessTokenStore
    {
        public bool Populated { get; set; }
        public string Token { get; set; }
        public string TokenSecret { get; set; }
        private readonly IFile TokensFile;

        public AccessTokenStore()
        {
            IFolder rootFolder = FileSystem.Current.LocalStorage;

            IFolder geoCachingToolboxFolder =
                rootFolder.CreateFolderAsync("Geocaching Toolbox", CreationCollisionOption.OpenIfExists).Result;
            TokensFile = geoCachingToolboxFolder.CreateFileAsync("Tokens.txt", CreationCollisionOption.OpenIfExists).Result;

            string read = TokensFile.ReadAllTextAsync().Result;
            if (!string.IsNullOrWhiteSpace(read))
            {
                var split = read.Split(':');
                if (split != null && split.Length == 2)
                {
                    Token = split[0];
                    TokenSecret = split[1];
                    Populated = true;
                }
            }
        }

        public void SetValues(string token, string tokenSecret)
        {
            Token = token;
            TokenSecret = tokenSecret;
            TokensFile.WriteAllTextAsync($"{Token}:{TokenSecret}").Wait();
            Populated = true;
        }
    }
}
