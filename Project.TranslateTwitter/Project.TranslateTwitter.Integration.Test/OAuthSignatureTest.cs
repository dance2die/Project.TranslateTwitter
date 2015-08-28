using Project.TranslateTwitter.Security;
using Xunit;

namespace Project.TranslateTwitter.Integration.Test
{
	public class OAuthSignatureTest
	{
		/// <summary>
		/// Test whether OAuth Sinature is generated correctly according to Twitter's developer documentation
		/// </summary>
		/// <remarks>
		/// https://dev.twitter.com/oauth/overview/creating-signatures
		/// </remarks>
		[Fact]
		public void ValidateOAuthSignature()
		{
			// I need to figure out how to create and use mocking object 
			// instead of creating a concrete class to pass to c'tor
			IAuthenticationContext authenticationContext = new TestAuthenticationContext();
			// SUT = System Under Test
			var sut = new OAuthSignatureBuilder(authenticationContext);

			RequestParameters parameters = new TestRequestParameters(authenticationContext);
			string oauthSignature = sut.CreateSignature(parameters);

			// Second paramter from Twitter documentation: https://dev.twitter.com/oauth/overview/creating-signatures
			Assert.Equal(oauthSignature, "tnnArxj06cWHq44gCs1OSKk/jLY=");
		}
	}
}
