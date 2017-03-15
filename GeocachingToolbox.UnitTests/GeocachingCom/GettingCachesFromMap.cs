using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using System.Web;
using GeocachingToolbox.GeocachingCom;
using Machine.Specifications;
using Rhino.Mocks;


namespace GeocachingToolbox.UnitTests.GeocachingCom
{
    [Subject("Getting caches from Map (caches found + owned) lploumen user")]
    public class GettingCachesFromMap1
    {
        private static GCClient _gcClient;
        private static IGCConnector _stubConnector;
        private static IEnumerable<GCGeocache> _result;
        private static IEnumerable<GCGeocache> _expectedResult;
        Establish context = () =>
        {
            _stubConnector = MockRepository.GenerateMock<IGCConnector>();
            _gcClient = new GCClient(_stubConnector);

            _stubConnector.Expect(x => x.Login(Arg<string>.Is.Anything, Arg<string>.Is.Anything))
               .ReturnContentOf(@"GeocachingCom\WebpageContents\LoginSuccessfulAndUserProfile.html").Repeat.Once();

            _stubConnector.Expect(x => x.GetPage(Arg<string>.Is.Anything))
               .Do((Func<string, Task<string>>)(url =>
                {
                    if (url == GCConstants.URL_LIVE_MAP)
                    {
                        var page = MspecExtensionMethods.ReadContent(@"GeocachingCom\WebpageContents\LiveMap.html");
                        return Task.FromResult(page);
                    }
                    var x = HttpUtility.ParseQueryString(url).Get(0);
                    var y = HttpUtility.ParseQueryString(url).Get("y");
                    var z = HttpUtility.ParseQueryString(url).Get("z");
                    var data = MspecExtensionMethods.ReadContent($@"GeocachingCom\WebpageContents\Tiles\Set1\map_{x}_{y}_{z}.json");
                    return Task.FromResult(data);
                }));

            _stubConnector.Expect(x => x.GetContent(Arg<string>.Is.Anything, Arg<IDictionary<string, string>>.Is.Null))
                .Do((Func<string, IDictionary<string, string>, Task<HttpContent>>)((url, data) =>
               {
                   var x = HttpUtility.ParseQueryString(url).Get(0);
                   var y = HttpUtility.ParseQueryString(url).Get("y");
                   var z = HttpUtility.ParseQueryString(url).Get("z");
                   var bytes = MspecExtensionMethods.ReadContentasByteArray($@"GeocachingCom\WebpageContents\Tiles\Set1\map_{x}_{y}_{z}.png");
                   return Task.FromResult<HttpContent>(new ByteArrayContent(bytes));
               }));
            GCUser expectedOwner = new GCUser("Krzema",123);
            _expectedResult = new List<GCGeocache>
            {

                 new GCGeocache {
                        Code="GC2H81Y",
                        Name = "Respect 2",
                        Found = true,
                        Waypoint = new Location(50,39.588M,5,54.751M),
                        Type = GeocacheType.Unknown
                   },
                 new GCGeocache {
                        Code = "GC62EY0",
                        Name = "Hoof",
                        Found = false,
                        Waypoint = new Location(50,39.287M,5,55.293M),
                        Type = GeocacheType.Unknown,
                   },
                 new GCGeocache {
                        Code="GC62EX6",
                        Name = "Grünhaut 3",
                        Waypoint = new Location(50,39.032M,5,55.579M),
                        Found = false,
                        Type = GeocacheType.Unknown,
                   },
                 new GCGeocache {
                        Code="GC62EWZ",
                        Name = "Grünhaut 2",
                        Waypoint = new Location(50,38.941M,5,55.132M),
                        Found = false,
                        Type = GeocacheType.Unknown,
                   },
                 new GCGeocache {
                        Code="GCK6J8",
                        Name = "La forêt de Grünhaut",
                        Waypoint = new Location(50,38.850M,5,55.710M),
                        Found = true,
                        Type = GeocacheType.Unknown
                   },
                 new GCGeocache {
                        Code="GC62EPM",
                        Name = "Grünhaut 1",
                        Waypoint = new Location(50,38.833M,5,54.960M),
                        Found = false,
                        Type = GeocacheType.Unknown,
                   },
                 new GCGeocache {
                         Code="GC3RMAT",
                         Name = "Ecoduc Grünhaut",
                         Waypoint = new Location(50,39.006M,5,55.953M),
                         Found = true,
                         Type = GeocacheType.Unknown
                   },
                  new GCGeocache {
                         Code="GC5JHRV",
                         Name = "Saint Georges",
                         Waypoint = new Location(50,39.130M,5,56.730M),
                         Found = false,
                         Type = GeocacheType.Mystery
                   },
            };
        };

        Because of = () =>
        {
            CancellationTokenSource cts = new CancellationTokenSource();
           _result = _gcClient.GetGeocachesFromMap<GCGeocache>(new Location(50.6416774807, 5.9097003937),
                       new Location(50.6637713105, 5.9530448914),cts.Token).Await().AsTask.Result;
       };
        It should_return_a_list_of_8_caches_found_and_owned_lploumen = () =>
        {
            _result.Count().ShouldEqual(8);
            foreach (var expectedResult in _expectedResult)
            {
                var cache = _result.FirstOrDefault(x => x.Code == expectedResult.Code);
                cache.ShouldNotBeNull();
                Debug.WriteLine($"Cache {cache.Name}");
                cache.Found.ShouldEqual(expectedResult.Found);

                cache.IsDetailed.ShouldBeFalse();
                Math.Abs(cache.Waypoint.Latitude - expectedResult.Waypoint.Latitude).ShouldBeLessThan(0.001);
                Math.Abs(cache.Waypoint.Longitude - expectedResult.Waypoint.Longitude).ShouldBeLessThan(0.001);
                cache.Type.ShouldEqual(expectedResult.Type);
                expectedResult.Owner?.Name?.ShouldEqual(cache.Owner?.Name);
            }
        };
    }


    [Subject("Getting caches from Map (no caches found) cyborg3 user")]
    public class GettingCachesFromMap2
    {
        private static GCClient _gcClient;
        private static IGCConnector _stubConnector;
        private static IEnumerable<GCGeocache> _result;
        private static IEnumerable<GCGeocache> _expectedResult;
        Establish context = () =>
        {
            _stubConnector = MockRepository.GenerateMock<IGCConnector>();
            _gcClient = new GCClient(_stubConnector);

            _stubConnector.Expect(x => x.Login(Arg<string>.Is.Anything, Arg<string>.Is.Anything))
               .ReturnContentOf(@"GeocachingCom\WebpageContents\LoginSuccessfulAndUserProfile.html").Repeat.Once();

            _stubConnector.Expect(x => x.GetPage(Arg<string>.Is.Anything))
               .Do((Func<string, Task<string>>)(url =>
               {
                   if (url == GCConstants.URL_LIVE_MAP)
                   {
                       var page = MspecExtensionMethods.ReadContent(@"GeocachingCom\WebpageContents\LiveMap.html");
                       return Task.FromResult(page);
                   }
                   var x = HttpUtility.ParseQueryString(url).Get(0);
                   var y = HttpUtility.ParseQueryString(url).Get("y");
                   var z = HttpUtility.ParseQueryString(url).Get("z");
                   var data = MspecExtensionMethods.ReadContent($@"GeocachingCom\WebpageContents\Tiles\Set2\map_{x}_{y}_{z}.json");
                   return Task.FromResult(data);
               }));

            _stubConnector.Expect(x => x.GetContent(Arg<string>.Is.Anything, Arg<IDictionary<string, string>>.Is.Null))
                .Do((Func<string, IDictionary<string, string>, Task<HttpContent>>)((url, data) =>
                {
                    var x = HttpUtility.ParseQueryString(url).Get(0);
                    var y = HttpUtility.ParseQueryString(url).Get("y");
                    var z = HttpUtility.ParseQueryString(url).Get("z");
                    var bytes = MspecExtensionMethods.ReadContentasByteArray($@"GeocachingCom\WebpageContents\Tiles\Set2\map_{x}_{y}_{z}.png");
                    return Task.FromResult<HttpContent>(new ByteArrayContent(bytes));
                }));
            GCUser expectedOwner = new GCUser("Krzema", 123);
            _expectedResult = new List<GCGeocache>
            {

                 new GCGeocache {
                        Code="GC2H81Y",
                        Name = "Respect 2",
                        Found = false,
                        Waypoint = new Location(50,39.588M,5,54.751M),
                        Type = GeocacheType.Traditional
                   },
                 new GCGeocache {
                        Code = "GC62EY0",
                        Name = "Hoof",
                        Found = false,
                        Waypoint = new Location(50,39.287M,5,55.293M),
                        Type = GeocacheType.Traditional,
                   },
                 new GCGeocache {
                        Code="GC62EX6",
                        Name = "Grünhaut 3",
                        Waypoint = new Location(50,39.032M,5,55.579M),
                        Found = false,
                        Type = GeocacheType.Traditional,
                   },
                 new GCGeocache {
                        Code="GC62EWZ",
                        Name = "Grünhaut 2",
                        Waypoint = new Location(50,38.941M,5,55.132M),
                        Found = false,
                        Type = GeocacheType.Traditional ,
                   },
                 new GCGeocache {
                        Code="GCK6J8",
                        Name = "La forêt de Grünhaut",
                        Waypoint = new Location(50,38.850M,5,55.710M),
                        Found = false,
                        Type = GeocacheType.Traditional
                   },
                 new GCGeocache {
                        Code="GC62EPM",
                        Name = "Grünhaut 1",
                        Waypoint = new Location(50,38.833M,5,54.960M),
                        Found = false,
                        Type = GeocacheType.Traditional,
                   },
                 new GCGeocache {
                         Code="GC3RMAT",
                         Name = "Ecoduc Grünhaut",
                         Waypoint = new Location(50,39.006M,5,55.953M),
                         Found = false,
                         Type = GeocacheType.Traditional
                   },
                  new GCGeocache {
                         Code="GC5JHRV",
                         Name = "Saint Georges",
                         Waypoint = new Location(50,39.130M,5,56.730M),
                         Found = false,
                         Type = GeocacheType.Mystery
                   },
            };
        };

        Because of = () =>
        {
            CancellationTokenSource cts = new CancellationTokenSource();
            _result = _gcClient.GetGeocachesFromMap<GCGeocache>(new Location(50.6416774807, 5.9097003937),
                        new Location(50.6637713105, 5.9530448914), cts.Token).Await().AsTask.Result;
        };
        It should_return_a_list_of_8_caches_none_found_cyborg93 = () =>
        {
            _result.Count().ShouldEqual(8);
            foreach (var expectedResult in _expectedResult)
            {
                var cache = _result.FirstOrDefault(x => x.Code == expectedResult.Code);
                cache.ShouldNotBeNull();
                Debug.WriteLine($"Cache {cache.Name}");
                cache.Found.ShouldEqual(expectedResult.Found);

                cache.IsDetailed.ShouldBeFalse();
                Math.Abs(cache.Waypoint.Latitude - expectedResult.Waypoint.Latitude).ShouldBeLessThan(0.001);
                Math.Abs(cache.Waypoint.Longitude - expectedResult.Waypoint.Longitude).ShouldBeLessThan(0.001);
                cache.Type.ShouldEqual(expectedResult.Type);
                expectedResult.Owner?.Name?.ShouldEqual(cache.Owner?.Name);
            }
        };
    }
}