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
    [Subject("Posting a log to a trackable")]
    public class PostingLogToTrackable
    {
        protected static GCClient _gcClient;
        protected static IGCConnector _stubConnector;
        protected static GCTrackable _trackableToBeLogged;
        protected static TrackableLogType _logType;
        protected static DateTime _date;
        protected static string _description;

        Establish context = () =>
        {
            _stubConnector = MockRepository.GenerateMock<IGCConnector>();
            _gcClient = new GCClient(_stubConnector);
            _stubConnector.Expect(x => x.GetPage("track/details.aspx?tracker=F5K12Z"))
                .ReturnContentOf(@"GeocachingCom\WebpageContents\TrackableDetailsPage.html").Repeat.Once();
            _stubConnector.Expect(x => x.GetPage("track/log.aspx?wid=c09c3066-97d1-4935-8c67-2cfe0d4bcdf1&c=F5K12Z"))
                .ReturnContentOf(@"GeocachingCom\WebpageContents\PostingTrackableLogForm.html").Repeat.Once();
            // TODO: specify what should be taken by PostToPage as arguments. I don't know how to test it with MSpec.
            // See some commented code in "PostingLogToGeocache.cs".
            _stubConnector.Expect(x => x.PostToPage(Arg<string>.Is.Anything, Arg<IDictionary<string, string>>.Is.Anything))
                .ReturnContentOf(@"GeocachingCom\WebpageContents\PostingTrackableLogSuccessful.html").Repeat.Once();

            _trackableToBeLogged = new GCTrackable
            {
                TrackingCode = "F5K12Z"
            };

            _logType = TrackableLogType.WriteNote;
            _date = new DateTime(2015, 10, 5);
            _description = "Testowa treść";
        };

        Because of = () =>
            _gcClient.PostTrackableLogAsync(_trackableToBeLogged, _logType, _date, _description);

        It should_call_connectors_GetPage_and_PostToPage_methods = () =>
            _stubConnector.VerifyAllExpectations();
    }

    [Subject("Trying to posting a log to a trackable which does not exist")]
    public class TrackableNotFound
    {
        protected static GCClient _gcClient;
        protected static IGCConnector _stubConnector;
        protected static GCTrackable _trackableToBeLogged;
        protected static TrackableLogType _logType;
        protected static DateTime _date;
        protected static string _description;
        protected static Exception _exception;

        Establish context = () =>
        {
            _stubConnector = MockRepository.GenerateMock<IGCConnector>();
            _gcClient = new GCClient(_stubConnector);
            _stubConnector.Expect(x => x.GetPage("track/details.aspx?tracker=ABCDEFG"))
                .ReturnContentOf(@"GeocachingCom\WebpageContents\TrackableDoesNotExist.html").Repeat.Once();

            _trackableToBeLogged = new GCTrackable
            {
                TrackingCode = "ABCDEFG"
            };

            _logType = TrackableLogType.WriteNote;
            _date = new DateTime(2015, 10, 5);
            _description = "Testowa treść";
        };

        Because of = () =>
            _exception = Catch.Exception(() => _gcClient.PostTrackableLogAsync(_trackableToBeLogged, _logType, _date, _description).Await());

        It should_call_connectors_GetPage_method = () =>
            _stubConnector.VerifyAllExpectations();

        It should_throw_proper_exception = () =>
            _exception.ShouldBeOfExactType(typeof(TrackableNotFoundException));
    }
}
