using GeocachingToolbox.Opencaching;
using Machine.Specifications;
using Rhino.Mocks;
using System;
using System.Collections.Generic;

namespace GeocachingToolbox.UnitTests.Opencaching
{
    [Subject("Getting a nonempty list of logs of found geocaches")]
    public class GetNonemptyListOfFoundGeocaches : OCTestBase
    {
        protected static IEnumerable<OCLog> _result;
        protected static IEnumerable<OCLog> _expectedResult;

        Establish context = () =>
        {
            _stubConnector.Expect(x => x.GetResponse(Arg<string>.Is.Anything))
                .ReturnContentOf(@"Opencaching\JsonResponses\NonemptyFoundGeocaches.json").Repeat.Once();

            _stubConnector.Expect(x => x.GetURL(Arg<string>.Is.Equal("services/logs/userlogs"),
                Arg<Dictionary<string, string>>.Is.Anything)).Return("SomeUrl").Repeat.Once();

            _expectedResult = new List<OCLog>()
            {
                new OCLog {
                    Date = new DateTime(2015, 1, 17, 14, 05, 00),
                    LogType = GeocacheLogType.Found,
                    Uuid = "44ead5e6-ff8d-4301-851e-21ed85840a93",
                    Comment = "Some comment in English, without any diacritics.",
                    Thing = new OCGeocache {
                        Code = "OP7EA1"
                    }
                },
                new OCLog {
                    Date = new DateTime(2014, 12, 24, 14, 20, 00),
                    LogType = GeocacheLogType.Found,
                    Uuid = "60b8071d-839c-46f0-bded-158b7b3c266e",
                    Comment = "Another comment, not too long.",
                    Thing = new OCGeocache {
                        Code = "OP51B1"
                    }
                },
                new OCLog {
                    Date = new DateTime(2014, 12, 14, 00, 54, 00),
                    LogType = GeocacheLogType.Attended,
                    Uuid = "13CBBEEE-B4D0-A9CC-65CF-206F04FA2E8B",
                    Comment = "This was a great event!",
                    Thing = new OCGeocache {
                        Code = "OP85B1"
                    }
                }
            };
        };

        Because of = () =>
            _result = _ocClient.GetFoundGeocaches<OCLog>();

        It should_return_a_list_of_3_logs_of_caches = () =>
            _result.ShouldEqualRecursively(_expectedResult);

        It should_call_connectors_methods = () =>
            _stubConnector.VerifyAllExpectations();
    }
}
