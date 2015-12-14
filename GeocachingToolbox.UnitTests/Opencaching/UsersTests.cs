using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Machine.Specifications;
using Rhino.Mocks;
using GeocachingToolbox.Opencaching;

namespace GeocachingToolbox.UnitTests.Opencaching
{
	//[Subject("Getting a nonempty list of logs of found geocaches")]
	//public class GetUserInformation
	//{
	//	protected static OCClient _ocClient;
	//	protected static IOCConnector _stubConnector;
	//	protected static OCUser _expectedUser;
	//	protected static OCUser _user;

	//	Establish context = () =>
	//	{
	//		_stubConnector = MockRepository.GenerateMock<IOCConnector>();
	//		_ocClient = new OCClient("someInstallationUrl", _stubConnector);
	//		_stubConnector.Expect(x => x.GetURL(Arg<string>.Is.Equal("services/users/user"),
	//			Arg<Dictionary<string, string>>.Is.Anything,
	//			Arg<string>.Is.Anything, Arg<string>.Is.Anything,
	//			Arg<string>.Is.Anything, Arg<string>.Is.Anything, Arg<bool>.Is.Anything))
	//			.Return("SomeUrl").Repeat.Once();
	//		_stubConnector.Expect(x => x.GetResponse(Arg<string>.Is.Anything))
	//			.ReturnContentOf(@"Opencaching\JsonResponses\NonemptyFoundGeocaches.json").Repeat.Once();

	//		_expectedUser = new OCUser("Krzema", 14, "A2FEE2F0-9F5C-E1E9-1945-11B16D2B7B4F");
	//	};

	//	Because of = () =>
	//	{
	//		_user = _ocClient.User as OCUser;
	//	};

	//	It should_return_user_with_correct_data = () =>
	//		_user.ShouldEqual<OCUser>(_expectedUser);
	//}
}
