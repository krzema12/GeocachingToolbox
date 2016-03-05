using GeocachingToolbox.Opencaching;
using Machine.Specifications;
using Rhino.Mocks;
using System;
using System.Collections.Generic;

namespace GeocachingToolbox.UnitTests.Opencaching
{
    [Subject("Getting details of a geocache")]
    public class GettingDetailsOfCache : OCTestBase
    {
        protected static OCGeocache _subject;
        protected static OCGeocache _expectedResult;

        Establish context = () =>
        {
            _stubConnector.Expect(x => x.GetResponse(Arg<string>.Is.Anything))
                .ReturnContentOf(@"Opencaching\JsonResponses\GeocacheDetails.json").Repeat.Once();

            _stubConnector.Expect(x => x.GetURL(Arg<string>.Is.Equal("services/caches/geocache"),
                Arg<Dictionary<string, string>>.Is.Anything)).Return("SomeUrl").Repeat.Once();

            _subject = new OCGeocache
            {
                Code = "OP886F"
            };

            _expectedResult = new OCGeocache()
            {
                Code = "OP886F",
                Name = "PARK MILLENIUM GDAŃSKA",
                Waypoint = new Location(54.39635M, 18.6134M),
                Type = GeocacheType.Traditional,
                Status = GeocacheStatus.Published,
                Size = GeocacheSize.Micro,
                Difficulty = 2.0f,
                Terrain = 1.0f,
                DateHidden = new DateTime(2015, 04, 08),
                Description = "Park wybudowany zosta\u0142 w latach 1997\u20131998,"
                    + " ze \u015brodk\u00f3w Gminnego Funduszu Ochrony \u015arodowiska"
                    + " i Gospodarki Wodnej, jako park osiedlowy dla mieszka\u0144c"
                    + "\u00f3w dzielnic Zaspa i Dolnego Wrzeszcza.",
                Hint = "Poniżej gruntu",
                Owner = new OCUser("KTA AZYMUT", -1, "07474FE9-277F-A6F3-2696-B01B9B87862B")
            };
        };

        Because of = () =>
        {
            _ocClient.GetGeocacheDetails(_subject);
        };

        It should_return_an_object_with_geocache_details = () =>
            _subject.ShouldEqualRecursively(_expectedResult);

        It should_call_connectors_methods = () =>
            _stubConnector.VerifyAllExpectations();
    }
}
