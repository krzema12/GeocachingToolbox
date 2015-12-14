using System.Net;
using GeocachingToolbox.GeocachingCom;
using Machine.Specifications;
using Rhino.Mocks;
using System;

namespace GeocachingToolbox.UnitTests.GeocachingCom
{
	public abstract class LoginTestsBase
	{
		protected static GCClient _gcClient;
		protected static IGCConnector _stubConnector;

		Establish context = () =>
		{
			_stubConnector = MockRepository.GenerateMock<IGCConnector>();
			_gcClient = new GCClient(_stubConnector);
		};
	}

	[Subject("Logging in to Geocaching.com")]
	public class CorrectCredentials : LoginTestsBase
	{
		Establish context = () =>
		{
			_stubConnector.Expect(x => x.Login(Arg<string>.Is.Anything, Arg<string>.Is.Anything))
                .ReturnContentOf(@"GeocachingCom\WebpageContents\LoginSuccessfulAndUserProfile.html").Repeat.Once();
		};

		Because of = () =>
			_gcClient.Login("Krzema", "CorrectPassword");

		It should_call_connectors_Login_method = () =>
			_stubConnector.VerifyAllExpectations();
        It should_assign_user_name_to_appropriate_field = () =>
            _gcClient.User.Name.ShouldEqual("Krzema");
        It should_assign_number_of_found_geocaches_to_appropriate_field = () =>
			_gcClient.User.FoundGeocachesCount.ShouldEqual(153);
	}

	[Subject("Logging in to Geocaching.com")]
	public class IncorrectCredentials : LoginTestsBase
	{
		private static Exception _exception;

		Establish context = () =>
		{
			_stubConnector.Expect(x => x.Login(Arg<string>.Is.Anything, Arg<string>.Is.Anything))
				.ReturnContentOf(@"GeocachingCom\WebpageContents\LoginWrongLoginPassword.html").Repeat.Once();
		};

		Because of = () =>
			_exception = Catch.Exception(() => _gcClient.Login("Krzema", "IncorrectPassword"));

		It should_throw_IncorrectCredentialsException = () =>
			_exception.ShouldBeOfExactType<IncorrectCredentialsException>();
		It should_throw_exception_with_proper_message = () =>
			_exception.Message.ShouldEqual("Incorrect credentials!");
	}

	[Subject("Logging in to Geocaching.com")]
	public class NoInternetConnection : LoginTestsBase
	{
		private static Exception _exception;

		Establish context = () =>
		{
			_stubConnector.Expect(x => x.Login(Arg<string>.Is.Anything, Arg<string>.Is.Anything))
				.Throw(new WebException()).Repeat.Once();
		};

		Because of = () =>
			_exception = Catch.Exception(() => _gcClient.Login("Krzema", "Anything"));

		It should_throw_ConnectionProblemException = () =>
			_exception.ShouldBeOfExactType<ConnectionProblemException>();
		It should_throw_exception_with_proper_message = () =>
			_exception.Message.ShouldEqual("A problem with Internet connection occured!");
	}
}
