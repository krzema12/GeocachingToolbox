using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace GeocachingToolbox.Opencaching
{
    public interface IOCConnector
    {
        bool UseSsl { get; set; }

        void SetConsumerKeyAndSecret(string consumerKey, string consumerSecret);
        void SetTokens(string token, string tokenSecret);
        string GetURL(string method, Dictionary<string, string> args = null);
        Task<string> GetResponse(string url,CancellationToken ct = default(CancellationToken));
    }
}
