using GeocachingToolbox.GeocachingCom;
using Machine.Specifications;
using Rhino.Mocks;
using System;
using System.Collections.Generic;

namespace GeocachingToolbox.UnitTests.GeocachingCom
{
    [Subject("Getting nearest geocaches from given place")]
    public class GettingNearestGeocachesFromGivenPlace
    {
        protected static GCClient _gcClient;
        protected static IGCConnector _stubConnector;
        protected static IEnumerable<GCGeocache> _result;
        protected static IEnumerable<GCGeocache> _expectedResult;

        Establish context = () =>
        {
            _stubConnector = MockRepository.GenerateMock<IGCConnector>();
            _gcClient = new GCClient(_stubConnector);
            _stubConnector.Expect(x => x.GetPage("seek/nearest.aspx?lat=54.371676&lng=18.612415&ex=0"))
                .ReturnContentOf(@"GeocachingCom\WebpageContents\NonemptyNearestGeocaches.html").Repeat.Once();

            _expectedResult = new List<GCGeocache>()
            {
                new GCGeocache {
                    Name = "WETI Welcome",
                    Code = "GC41N6T",
                    Type = GeocacheType.Mystery,
                    Status = GeocacheStatus.Published,
                    Size = GeocacheSize.Micro,
                    Difficulty = 2.5f,
                    Terrain = 1.5f,
                    IsPremium = false,
                    DateHidden = new DateTime(2012, 11, 21),
                    Owner = new GCUser("Madzia_Krzys", -1)
                },
                new GCGeocache {
                    Name = "Dwa kwadraty",
                    Code = "GC4V13Q",
                    Type = GeocacheType.Mystery,
                    Status = GeocacheStatus.Published,
                    Size = GeocacheSize.Small,
                    Difficulty = 4.0f,
                    Terrain = 2.0f,
                    IsPremium = false,
                    DateHidden = new DateTime(2013, 12, 3),
                    Owner = new GCUser("grzesztof", -1)
                },
                new GCGeocache {
                    Name = "Czas na zagadkę!",
                    Code = "GC5GG2J",
                    Type = GeocacheType.Mystery,
                    Status = GeocacheStatus.Published,
                    Size = GeocacheSize.Small,
                    Difficulty = 4.5f,
                    Terrain = 2.0f,
                    IsPremium = false,
                    DateHidden = new DateTime(2014, 11, 14),
                    Owner = new GCUser("Krzema", -1)
                },
                new GCGeocache {
                    Name = "Bary mleczne / Milk bar - Akademicki",
                    Code = "GC410P0",
                    Type = GeocacheType.Traditional,
                    Status = GeocacheStatus.Published,
                    Size = GeocacheSize.Other,
                    Difficulty = 3.0f,
                    Terrain = 2.0f,
                    IsPremium = true,
                    DateHidden = new DateTime(2012, 11, 9),
                    Owner = new GCUser("abdul65", -1)
                },
                new GCGeocache {
                    Name = "Regionalne Centrum Krwiodawstwa i Krwiolecznictwa ",
                    Code = "GC4AFD5",
                    Type = GeocacheType.Mystery,
                    Status = GeocacheStatus.Published,
                    Size = GeocacheSize.Micro,
                    Difficulty = 2.0f,
                    Terrain = 1.5f,
                    IsPremium = false,
                    DateHidden = new DateTime(2013, 4, 23),
                    Owner = new GCUser("MASHTRIX & FuriousSquirrel", -1)
                }
            };
        };

        private Because of = () =>
        {
            _gcClient._dateFormat = "dd/MM/yyyy";
            _result =
                _gcClient.GetNearestGeocachesAsync<GCGeocache>(new Location(54.371676M, 18.612415M))
                    .Await()
                    .AsTask.Result;
        };

        It should_return_a_list_of_5_caches = () =>
            _result.ShouldEqualRecursively(_expectedResult);
    }
}
