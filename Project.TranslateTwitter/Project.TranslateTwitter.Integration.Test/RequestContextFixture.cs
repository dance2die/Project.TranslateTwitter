using Project.TranslateTwitter.Security;

namespace Project.TranslateTwitter.Integration.Test
{
	public class RequestContextFixture
	{
		public IAuthenticationContext AuthenticationContext { get; set; }
		public RequestParameters RequestParameters { get; set; }

		public RequestContextFixture()
		{
			AuthenticationContext = new TestAuthenticationContext();
			RequestParameters = new TestRequestParameters(AuthenticationContext);
		}
	}
}