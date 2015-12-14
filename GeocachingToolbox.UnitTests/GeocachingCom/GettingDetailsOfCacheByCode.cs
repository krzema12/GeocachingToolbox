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
	[Subject("Getting details of a geocache by its code")]
	public class GettingDetailsOfCacheByCode
	{
		protected static GCClient _gcClient;
		protected static IGCConnector _stubConnector;
		protected static GCGeocache _subject;
		protected static GCGeocache _expectedResult;

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

			_expectedResult = new GCGeocache
			{
				Code = "GC5V392",
				Name = "Biało-czerwoni",
				Waypoint = new Location(54, 23.565M, 18, 35.752M),
				Type = GeocacheType.Traditional,
				Status = GeocacheStatus.Published,
				Size = GeocacheSize.Micro,
				Difficulty = 2.0f,
				Terrain = 1.5f,
				IsPremium = false,
				DateHidden = new DateTime(2015, 05, 17),
				Description = "<p>Tradycyjna skrytka mikro. Weź ze sobą coś do pisania."
					+ "</p><br /><p>Znajdujący się w tym miejscu ogródek skalny z fontanną to"
					+ " instalacja \"Polska\" autorstwa Piotra Szwabe. Zasadniczą częścią"
					+ " instalacji jest sześć biało-czerwonych manekinów. Każdy odpowiada"
					+ " jednej literze z nazwy naszego kraju.</p>",
				Hint = "magnetyk",
				Owner = new GCUser("ratunku&koralik", -1)
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
