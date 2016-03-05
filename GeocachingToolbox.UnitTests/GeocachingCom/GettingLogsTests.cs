using System.Collections.Generic;
using GeocachingToolbox.GeocachingCom;
using KellermanSoftware.CompareNetObjects;
using Machine.Specifications;
using Rhino.Mocks;
using System;

namespace GeocachingToolbox.UnitTests.GeocachingCom
{
    [Subject("Getting a nonempty list of logs of found geocaches")]
    public class GetNonemptyListOfFoundGeocaches
    {
        protected static GCClient _gcClient;
        protected static IGCConnector _stubConnector;
        protected static IEnumerable<GCLog> _result;
        protected static IEnumerable<GCLog> _expectedResult;

        Establish context = () =>
        {
            _stubConnector = MockRepository.GenerateMock<IGCConnector>();
            _gcClient = new GCClient(_stubConnector);
            _stubConnector.Expect(x => x.GetPage("my/logs.aspx?s=1&lt=2"))
                .ReturnContentOf(@"GeocachingCom\WebpageContents\NonemptyFoundGeocaches.html").Repeat.Once();

            _expectedResult = new List<GCLog>()
            {
                new GCLog {
                    Date = new DateTime(2015, 3, 19),
                    LogType = GeocacheLogType.Found,
                    IsFavorite = true,
                    Thing = new GCGeocache {
                        Name = "Ron's Park",
                        Type = GeocacheType.Traditional,
                        Status = GeocacheStatus.Published,
                        DetailsUrl = "seek/cache_details.aspx?guid=906186d7-b4bd-4c1a-87c2-af8485f10e16" }
                },
                new GCLog {
                    Date = new DateTime(2015, 3, 13),
                    LogType = GeocacheLogType.Found,
                    Thing = new GCGeocache {
                        Name = "Święta Trójca na Dąbrowie",
                        Type = GeocacheType.Mystery,
                        Status = GeocacheStatus.Published,
                        DetailsUrl = "seek/cache_details.aspx?guid=31e86d80-4b6f-4c9d-a4a1-ec6f578506ed" }
                },
                new GCLog {
                    Date = new DateTime(2015, 1, 13),
                    LogType = GeocacheLogType.Found,
                    Thing = new GCGeocache {
                        Name = "Kosciól na Czarnej",
                        Type = GeocacheType.Multicache,
                        Status = GeocacheStatus.Published,
                        DetailsUrl = "seek/cache_details.aspx?guid=f96a7104-3beb-4495-9a20-15b1b1ef796a" }
                },
                new GCLog {
                    Date = new DateTime(2014, 9, 7),
                    LogType = GeocacheLogType.Found,
                    Thing = new GCGeocache {
                        Name = "Herby Gdańskie",
                        Type = GeocacheType.Whereigo,
                        Status = GeocacheStatus.Published,
                        DetailsUrl = "seek/cache_details.aspx?guid=7eaa4ee0-1181-47c6-972e-84fbd972e3bf" }
                },
                new GCLog {
                    Date = new DateTime(2014, 8, 28),
                    LogType = GeocacheLogType.Found,
                    Thing = new GCGeocache {
                        Name = "Trójmiejski Park Krajobrazowy",
                        Type = GeocacheType.LetterboxHybrid,
                        Status = GeocacheStatus.Published,
                        DetailsUrl = "seek/cache_details.aspx?guid=540a9a3d-738b-4613-b14e-2e9818729443" }
                },
                new GCLog {
                    Date = new DateTime(2014, 8, 28),
                    LogType = GeocacheLogType.Found,
                    Thing = new GCGeocache {
                        Name = "Diabelski Kamien",
                        Type = GeocacheType.Earthcache,
                        Status = GeocacheStatus.Published,
                        DetailsUrl = "seek/cache_details.aspx?guid=ec298ce5-22d8-47c9-aa47-b4cc4aa375fd" }
                },
                new GCLog {
                    Date = new DateTime(2014, 9, 13),
                    LogType = GeocacheLogType.Found,
                    Thing = new GCGeocache {
                        Name = "Komisariat Policji VII w Gdansku",
                        Type = GeocacheType.Traditional,
                        Status = GeocacheStatus.Archived,
                        DetailsUrl = "seek/cache_details.aspx?guid=0c3a2546-369f-4195-9dbf-aeb3bf1f4926" }
                },
                new GCLog {
                    Date = new DateTime(2014, 9, 12),
                    LogType = GeocacheLogType.Found,
                    Thing = new GCGeocache {
                        Name = "Green Memorial / Zielony Pomnik",
                        Type = GeocacheType.Traditional,
                        Status = GeocacheStatus.Disabled,
                        DetailsUrl = "seek/cache_details.aspx?guid=d537f6d8-6a96-42a6-b216-69c69e733604" }
                }
            };
        };

        Because of = () =>
            _result = _gcClient.GetFoundGeocaches<GCLog>();

        It should_return_a_list_of_8_logs_of_caches_of_various_type_and_status = () =>
            _result.ShouldEqualRecursively(_expectedResult);
        It should_call_connectors_GetPage_method_with_proper_address = () =>
            _stubConnector.VerifyAllExpectations();
    }
}
