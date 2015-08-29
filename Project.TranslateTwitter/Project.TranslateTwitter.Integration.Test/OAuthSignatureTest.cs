using Project.TranslateTwitter.Security;
using Xunit;

namespace Project.TranslateTwitter.Integration.Test
{
	public class OAuthSignatureTest
	{
		private readonly IAuthenticationContext _authenticationContext;
		private readonly RequestParameters _testRequestParameters;

		public OAuthSignatureTest()
		{
			// I need to figure out how to create and use mocking object 
			// instead of creating a concrete class to pass to c'tor
			_authenticationContext = new TestAuthenticationContext();

			_testRequestParameters = new TestRequestParameters(_authenticationContext);
		}

		[Fact]
		public void ValidateSignatureBaseString()
		{
			var sut = new OAuthSignatureBuilder(_authenticationContext);

			RequestParameters parameters = new TestRequestParameters(_authenticationContext);
			string signatureString = sut.GetSignatureBaseString(_testRequestParameters);

			const string twitterDocumentationSignatureBaseString =
				"POST&https%3A%2F%2Fapi.twitter.com%2F1%2Fstatuses%2Fupdate.json&include_entities%3Dtrue%26oauth_consumer_key%3Dxvz1evFS4wEEPTGEFPHBog%26oauth_nonce%3DkYjzVBB8Y0ZFabxSWbWovY3uYSQ2pTgmZeNu2VS4cg%26oauth_signature_method%3DHMAC-SHA1%26oauth_timestamp%3D1318622958%26oauth_token%3D370773112-GmHxMAgYyLbNEtIKZeRNFsMKPR9EyMZeS9weJAEb%26oauth_version%3D1.0%26status%3DHello%2520Ladies%2520%252B%2520Gentlemen%252C%2520a%2520signed%2520OAuth%2520request%2521";
			Assert.Equal(signatureString, twitterDocumentationSignatureBaseString);
		}

		/// <summary>
		/// Test whether OAuth Sinature is generated correctly according to Twitter's developer documentation
		/// </summary>
		/// <remarks>
		/// https://dev.twitter.com/oauth/overview/creating-signatures
		/// </remarks>
		[Fact]
		public void ValidateOAuthSignature()
		{
			// SUT = System Under Test
			var sut = new OAuthSignatureBuilder(_authenticationContext);

			string oauthSignature = sut.CreateSignature(_testRequestParameters);

			// Second paramter from Twitter documentation: https://dev.twitter.com/oauth/overview/creating-signatures
			Assert.Equal(oauthSignature, "tnnArxj06cWHq44gCs1OSKk/jLY=");
		}


	}
}
