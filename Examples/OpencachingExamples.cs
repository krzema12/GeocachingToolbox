using System;
using GeocachingToolbox.Opencaching;
using System.Linq;

namespace Examples
{
    class OpencachingExamples
    {
        private OCClient client;

        public void Run()
        {
            Authenticate();
            DisplayUserInfo();
            ListNewestFoundGeocaches();
            GetDetailsForNewestFoundGeocache();

            Console.Write("\n\n");
            Console.WriteLine("Press enter to close this window...");
            Console.Read();
        }

        private void Authenticate()
        {
            client = new OCClient("http://opencaching.pl/okapi/");

            if (client.NeedsAuthorization)
            {
                Console.Write("Please open this link in your browser: " + client.GetAuthorizationUrl () + " and enter pin here:");
                string pin = Console.ReadLine();
                client.EnterAuthorizationPin(pin);
            }

            client.Connect();
        }

        private void DisplayUserInfo()
        {
            Console.WriteLine("Hello {0}! So far you've found {1} geocaches.", client.User.Name, client.User.FoundGeocachesCount);
        }

        private void ListNewestFoundGeocaches()
        {
            Console.WriteLine("Here are some geocaches you've recently found:");

            var foundLogsNewestTen = client.GetFoundGeocaches<OCLog>().Take(10);

            foreach (var log in foundLogsNewestTen)
            {
                var cache = log.Thing as OCGeocache;
                Console.WriteLine("{0}", cache.Code);
            }
        }

        private void GetDetailsForNewestFoundGeocache()
        {
            var newestFoundLog = client.GetFoundGeocaches<OCLog>().FirstOrDefault();

            if (newestFoundLog == null)
            {
                return;
            }

            var newestFoundCache = newestFoundLog.Thing as OCGeocache;
            client.GetGeocacheDetails<OCGeocache>(newestFoundCache);

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
