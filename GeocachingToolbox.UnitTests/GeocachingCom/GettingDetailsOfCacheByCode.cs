using GeocachingToolbox.GeocachingCom;
using Machine.Specifications;
using Rhino.Mocks;
using System;
using System.Collections.Generic;

namespace GeocachingToolbox.UnitTests.GeocachingCom
{
    [Subject("Getting details of a geocache by its code")]
    public class GettingDetailsOfCacheByCode
    {
        private static GCClient _gcClient;
        private static IGCConnector _stubConnector;
        private static GCGeocache _subject;
        private static GCGeocache _expectedResult;
        private static List<Log> _expectedLogs;

        Establish context = () =>
        {
            _stubConnector = MockRepository.GenerateMock<IGCConnector>();
            _gcClient = new GCClient(_stubConnector);
            _stubConnector.Expect(x => x.GetPage("geocache/GC5V392"))
                .ReturnContentOf(@"GeocachingCom\WebpageContents\GeocacheDetails.html").Repeat.Once();

            _subject = new GCGeocache
            {
                Code = "GC5V392"
            };

            _expectedLogs = new List<Log>
            {
                new Log
                {
                    Date = new DateTime(2016, 7, 27),
                    Comment =
                        @"Trouvée facilement. Je ne m'attendais pas à trouver une végétation si dense ici au dessus.mplc",
                    LogType = GeocacheLogType.Found,
                    Username = "didier-b52"
                },
                 new Log
                {
                    Date = new DateTime(2016, 7, 4),
                    Comment =
                        "We used the opportunity of being in Welkenraedt this morning to look for a few caches in the area. This was our second one. An interesting place, thanks to the very precise coordinates we found the cache quickly. Thanks for hiding the cache here and showing us this place.\nTFTC Geckohouse",
                    LogType = GeocacheLogType.Found,
                    Username = "geckohouse"
                },
                   new Log
                {
                    Date = new DateTime(2016, 6, 15),
                    Comment =
                        @"Lieu intéressant,on a appris quelque chose. Belle cache.",
                    LogType = GeocacheLogType.Found,
                    Username = "renardnoir"
                },
            };

            _expectedResult = new GCGeocache
            {
                Code = "GC5V392",
                Name = "Biało-czerwoni",
                Type = GeocacheType.Traditional,
                Status = GeocacheStatus.Published,
                Size = GeocacheSize.Micro,
                Difficulty = 2.0f,
                IsDetailed = true,
                Terrain = 1.5f,
                IsPremium = false,
                DateHidden = new DateTime(2015, 05, 17),
                ShortDescription = "<p>Tradycyjna skrytka mikro. Weź ze sobą coś do pisania.</p>",
                Description = "<p>Znajdujący się w tym miejscu ogródek skalny z fontanną to instalacja \"Polska\" autorstwa Piotra Szwabe. Zasadniczą częścią instalacji jest sześć biało-czerwonych manekinów. Każdy odpowiada jednej literze z nazwy naszego kraju.</p>",
                Hint = "magnetyk",
                Owner = new GCUser("ratunku&koralik", -1),
                Logs = _expectedLogs
            };
            _expectedResult.SetWaypoint(new Location(54, 23.565M, 18, 35.752M));
        };

        private Because of = () =>
        {
            _gcClient._dateFormat = "dd/MM/yyyy";
            _gcClient.GetGeocacheDetailsAsync(_subject).Await();
        };

        It should_return_an_object_with_geocache_details = () =>
            _subject.ShouldEqualRecursively(_expectedResult);
        It should_call_connectors_GetPage_method_with_proper_address = () =>
            _stubConnector.VerifyAllExpectations();
    }
}
