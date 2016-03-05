using GeocachingToolbox;
using GeocachingToolbox.GeocachingCom;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Examples
{
    class GeocachingComExamples
    {
        private GCClient client;
        private const string Login = "YourLoginHere";
        private const string Password = "YourPasswordHere";

        public void Run()
        {
            LogIn();
            DisplayUserInfo();
            ListNewestFoundGeocaches();
            GetDetailsForNewestFoundGeocache();

            Console.Write("\n\n");
            Console.WriteLine("Press enter to close this window...");
            Console.Read();
        }

        private void LogIn()
        {
            client = new GCClient();

            try
            {
                client.Login(Login, Password);
            }
            catch (IncorrectCredentialsException e)
            {
                Console.WriteLine("You probably didn't enter your real credentials several lines above...");
                throw e;
            }
        }

        private void DisplayUserInfo()
        {
            Console.WriteLine("Hello {0}! So far you've found {1} geocaches.", client.User.Name, client.User.FoundGeocachesCount);
        }

        private void ListNewestFoundGeocaches()
        {
            Console.WriteLine("Here are some geocaches you've recently found:");

            var foundLogsNewestTen = client.GetFoundGeocaches<GCLog>().Take(10);

            foreach (var log in foundLogsNewestTen)
            {
                var cache = log.Thing as GCGeocache;
                Console.WriteLine("({0}) {1}", cache.Status, cache.Name);
            }
        }

        private void GetDetailsForNewestFoundGeocache()
        {
            var newestFoundLog = client.GetFoundGeocaches<GCLog>().FirstOrDefault();

            if (newestFoundLog == null)
            {
                return;
            }

            var newestFoundCache = newestFoundLog.Thing as GCGeocache;
            client.GetGeocacheDetails<GCGeocache>(newestFoundCache);

            Console.WriteLine("Name:               {0}", newestFoundCache.Name);
            Console.WriteLine("Type:               {0}", newestFoundCache.Type);
            Console.WriteLine("Difficulty/Terrain: {0}/{1}", newestFoundCache.Difficulty, newestFoundCache.Terrain);
            Console.WriteLine("Status:             {0}", newestFoundCache.Status);
            Console.WriteLine("Is premium:         {0}", newestFoundCache.IsPremium);
            Console.WriteLine("Owner:              {0}", newestFoundCache.Owner.Name);
            Console.WriteLine("Hint:               {0}", newestFoundCache.Hint);
            Console.WriteLine("Description (beginning):\n{0}", newestFoundCache.Description.Substring(0, 500));
        }
    }
}
