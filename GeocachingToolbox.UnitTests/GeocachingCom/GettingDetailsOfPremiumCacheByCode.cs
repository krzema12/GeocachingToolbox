using GeocachingToolbox.GeocachingCom;
using Machine.Specifications;
using Rhino.Mocks;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GeocachingToolbox.UnitTests.GeocachingCom
{
    [Subject("Getting details of a premium geocache by its code")]
    public class GettingDetailsOfPremiumCacheByCode
    {
        protected static GCClient _gcClient;
        protected static IGCConnector _stubConnector;
        protected static GCGeocache _subject;
        protected static GCGeocache _expectedResult;

        Establish context = () =>
        {
            _stubConnector = MockRepository.GenerateMock<IGCConnector>();
            _gcClient = new GCClient(_stubConnector);
            _stubConnector.Expect(x => x.GetPage("geocache/GC41HTN"))
                .ReturnContentOf(@"GeocachingCom\WebpageContents\PremiumGeocacheDetails.html").Repeat.Once();

            _subject = new GCGeocache
            {
                Code = "GC41HTN"
            };

            _expectedResult = new GCGeocache
            {
                Code = "GC41HTN",
                Name = "Światowy Dzień Toalet 2012 / World Toilet Day 2012",
                Type = GeocacheType.Traditional,
                Size = GeocacheSize.Micro,
                Difficulty = 2.5f,
                Terrain = 1.5f,
                IsPremium = true,
                Owner = new GCUser("abdul65", -1)
            };
        };

        Because of = () =>
            _gcClient.GetGeocacheDetails(_subject);

        It should_return_an_object_with_geocache_details = () =>
            _subject.ShouldEqualRecursively(_expectedResult);
        It should_call_connectors_GetPage_method_with_proper_address = () =>
            _stubConnector.VerifyAllExpectations();
    }
}
