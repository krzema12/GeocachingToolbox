using System;
using GeocachingToolbox.Opencaching;
using System.Linq;
using System.Threading.Tasks;

namespace Examples
{
    class OpencachingExamples
    {
        private OCClient client;
        ApiAccessKeys m_ApiKeys;
        IAccessTokenStore m_AccessTokenStore;

        public OpencachingExamples(ApiAccessKeys apiKeys,IAccessTokenStore accessTokenStore)
        {
            m_ApiKeys = apiKeys;
            m_AccessTokenStore = accessTokenStore;
        }
        public void Run()
        {
            bool isAccessTokenStorePopulated = m_AccessTokenStore.Populated;
            Authenticate();
            DisplayUserInfo();
            ListNewestFoundGeocaches().GetAwaiter().GetResult();
            GetDetailsForNewestFoundGeocache().GetAwaiter().GetResult();

            Console.Write("\n\n");
            if (!isAccessTokenStorePopulated && m_AccessTokenStore.Populated)
            {
                Console.WriteLine("Add these settings to your config file:");
                Console.WriteLine("OCToken => " + m_AccessTokenStore.Token);
                Console.WriteLine("OCTokenSecret => " + m_AccessTokenStore.TokenSecret);
            }

            Console.WriteLine("Press enter to close this window...");
            Console.Read();
        }

        private void Authenticate()
        {
            client = new OCClient("http://opencaching.pl/okapi/", apiAccessKeys: m_ApiKeys,tokenStore: m_AccessTokenStore);

            if (client.NeedsAuthorization)
            {
                Console.Write("Please open this link in your browser: " + client.GetAuthorizationUrl () + " and enter pin here:");
                string pin = Console.ReadLine();
                client.EnterAuthorizationPin(pin).GetAwaiter().GetResult();
            }

            client.Connect().GetAwaiter().GetResult();
        }

        private void DisplayUserInfo()
        {
            Console.WriteLine("Hello {0}! So far you've found {1} geocaches.", client.User.Name, client.User.FoundGeocachesCount);
        }

        private async Task ListNewestFoundGeocaches()
        {
            Console.WriteLine("Here are some geocaches you've recently found:");
            var found = await client.GetFoundGeocachesAsync<OCLog>();
            var foundLogsNewestTen = found.Take(10);

            foreach (var log in foundLogsNewestTen)
            {
                var cache = log.Thing as OCGeocache;
                Console.WriteLine("{0}", cache.Code);
            }
        }

        private async Task GetDetailsForNewestFoundGeocache()
        {
            var found = await client.GetFoundGeocachesAsync<OCLog>();
            var newestFoundLog = found.FirstOrDefault();

            if (newestFoundLog == null)
            {
                return;
            }

            var newestFoundCache = newestFoundLog.Thing as OCGeocache;
            await client.GetGeocacheDetailsAsync<OCGeocache>(newestFoundCache);

            Console.WriteLine("Name:               {0}", newestFoundCache.Name);
            Console.WriteLine("Type:               {0}", newestFoundCache.Type);
            Console.WriteLine("Difficulty/Terrain: {0}/{1}", newestFoundCache.Difficulty, newestFoundCache.Terrain);
            Console.WriteLine("Status:             {0}", newestFoundCache.Status);
            Console.WriteLine("Owner:              {0}", newestFoundCache.Owner.Name);
            Console.WriteLine("Hint:               {0}", newestFoundCache.Hint);
            Console.WriteLine("Description (beginning):\n{0}", newestFoundCache.Description.Substring(0, 500));
        }
    }
}
