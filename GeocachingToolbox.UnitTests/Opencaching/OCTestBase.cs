using GeocachingToolbox.Opencaching;
using Machine.Specifications;
using Rhino.Mocks;
using System.Collections.Generic;

namespace GeocachingToolbox.UnitTests.Opencaching
{
	public abstract class OCTestBase
	{
		protected static OCClient _ocClient;
		protected static IOCConnector _stubConnector;
		protected static IAccessTokenStore _stubTokenStore;

		Establish context = () =>
		{
			_stubConnector = MockRepository.GenerateMock<IOCConnector>();
			_stubConnector.Expect(x => x.GetResponse(Arg<string>.Is.Anything))
				.ReturnContentOf(@"Opencaching\JsonResponses\UserInfo.json").Repeat.Once();

			_stubTokenStore = MockRepository.GenerateMock<IAccessTokenStore>();
			_stubTokenStore.Stub(x => x.Populated).Return(true);
			_stubTokenStore.Stub(x => x.Token).Return("abc");
			_stubTokenStore.Stub(x => x.TokenSecret).Return("def");

			_ocClient = new OCClient("someInstallationUrl", _stubConnector, _stubTokenStore);
			_ocClient.Connect();
		};
	}
}
