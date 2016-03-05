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
    [Subject("Posting a log to a geocache")]
    public class PostingLogToGeocache
    {
        protected static GCClient _gcClient;
        protected static IGCConnector _stubConnector;
        protected static GCGeocache _cacheToBeLogged;
        protected static GeocacheLogType _logType;
        protected static DateTime _date;
        protected static string _description;

        Establish context = () =>
        {
            _stubConnector = MockRepository.GenerateMock<IGCConnector>();
            _gcClient = new GCClient(_stubConnector);
            _stubConnector.Expect(x => x.GetPage("seek/log.aspx?wp=GC51VY1"))
                .ReturnContentOf(@"GeocachingCom\WebpageContents\PostingGeocacheLog.html").Repeat.Once();
            /*Dictionary<string, string> parameters = new Dictionary<string, string>
            {
                    { "__EVENTTARGET", ""  },
                    { "__EVENTARGUMENT", "" },
                    { "ctl00$ContentBody$24LogBookPanel1$24uxLogCreationSource", "Old" },
                    { "ctl00$ContentBody$24LogBookPanel1$24ddLogType", "4" },
                    { "ctl00$ContentBody$24LogBookPanel1$24uxDateVisited", "14/08/2015" },
                    { "ctl00$ContentBody$24LogBookPanel1$24uxLogInfo", "Testowa treść." },
                    { "ctl00$ContentBody$24LogBookPanel1$24btnSubmitLog", "Submit Log Entry" },
            };*/
            _stubConnector.Expect(x => x.PostToPage(Arg<string>.Is.Anything, Arg<IDictionary<string, string>>.Is.Anything))
                .ReturnContentOf(@"GeocachingCom\WebpageContents\PostingGeocacheLogSuccessful.html").Repeat.Once();

            _cacheToBeLogged = new GCGeocache
            {
                Code = "GC51VY1"
            };

            _logType = GeocacheLogType.WriteNote;
            _date = new DateTime(2015, 8, 14);
            _description = "Testowa treść";
        };

        Because of = () =>
            _gcClient.PostGeocacheLog(_cacheToBeLogged, _logType, _date, _description);

        It should_call_connectors_GetPage_and_PostToPage_methods = () =>
            _stubConnector.VerifyAllExpectations();
    }
}
