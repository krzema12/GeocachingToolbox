using GeocachingToolbox.GeocachingCom;
using Machine.Specifications;
using Rhino.Mocks;

namespace GeocachingToolbox.UnitTests.GeocachingCom
{
    [Subject("Encode image in base 64")]
    public class EncodeImageInBase64
    {
        protected static GCClient _gcClient;
        protected static IGCConnector _stubConnector;
        protected static GCGeocache _subject;
        protected static GCGeocache _expectedResult;
        Establish context = () =>
        {
            _stubConnector = MockRepository.GenerateMock<IGCConnector>();
            _gcClient = new GCClient(_stubConnector);
            _stubConnector.Expect(x => x.GetPage("geocache/GC3RMAT"))
                .ReturnContentOf(@"GeocachingCom\WebpageContents\GC3RMAT\GC3RMAT.html").Repeat.Once();
            _stubConnector.Expect(
                x =>
                    x.GetContent("http://img.geocaching.com/cache/large/624e3701-540b-43b1-b676-296ddab86de3.jpg", null))
                .ReturnContentOf(@"GeocachingCom\WebpageContents\GC3RMAT\624e3701-540b-43b1-b676-296ddab86de3.jpg");
            _stubConnector.Expect(
                x =>
                    x.GetContent("http://img.geocaching.com/cache/large/d08ba397-06dc-48e9-bf1a-47893ace6241.jpg", null))
                .ReturnContentOf(@"GeocachingCom\WebpageContents\GC3RMAT\d08ba397-06dc-48e9-bf1a-47893ace6241.jpg");

            _subject = new GCGeocache
            {
                Code = "GC3RMAT"
            };
        };
        Because of = () =>
        {
            _gcClient._dateFormat = "dd/MM/yyyy";
            _gcClient.GetGeocacheDetailsAsync(_subject).Await();
        };

        private It should_return_an_object_with_geocache_details = () =>
        {
            _subject.Description.ShouldNotContain("src=\"http");
        };
    }
}