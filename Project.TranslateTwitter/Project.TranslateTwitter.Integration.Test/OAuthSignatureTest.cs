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
		public void ValidateDictionaryJoiner()
		{
			var sut = new DictionaryToStringJoiner();
			var joinedString = sut.Join(_testRequestParameters.GetParameters());

			// From "https://dev.twitter.com/oauth/overview/creating-signatures"
			const string paramterString =
				"include_entities=true&oauth_consumer_key=xvz1evFS4wEEPTGEFPHBog&oauth_nonce=kYjzVBB8Y0ZFabxSWbWovY3uYSQ2pTgmZeNu2VS4cg&oauth_signature_method=HMAC-SHA1&oauth_timestamp=1318622958&oauth_token=370773112-GmHxMAgYyLbNEtIKZeRNFsMKPR9EyMZeS9weJAEb&oauth_version=1.0&status=Hello%20Ladies%20%2B%20Gentlemen%2C%20a%20signed%20OAuth%20request%21";

			Assert.Equal(joinedString, paramterString);
		}

		[Fact]
		public void ValidateSignatureBaseString()
		{
			var sut = new OAuthSignatureBuilder(_authenticationContext);

			string signatureString = sut.GetSignatureBaseString(_testRequestParameters);

			// From "https://dev.twitter.com/oauth/overview/creating-signatures"
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

		/// <summary>
		/// Compare OAuth Signature to be generated for Request Token
		/// </summary>
		/// <remarks>https://dev.twitter.com/oauth/reference/post/oauth/request_token</remarks>
		[Fact]
		public void ValidateRequestTokenOAuthSignature()
		{
			var authenticationContext = new AuthenticationContext();
			//authenticationContext.AccessToken = null;
			//authenticationContext.AccessTokenSecret = null;

			// SUT = System Under Test
			var sut = new OAuthSignatureBuilder(authenticationContext);

			RequestParameters requestParameters = new RequestTokenRequestParameters(authenticationContext, "oob");
			requestParameters.OAuthNonce = "e0d92fb7d5264baf85321de413176e9d";
			requestParameters.OAuthTimestamp = "1440899892";

			string baseString = sut.GetSignatureBaseString(requestParameters);
			const string expectedBaseString = "POST&https%3A%2F%2Fapi.twitter.com%2Foauth%2Frequest_token&oauth_callback%3Doob%26oauth_consumer_key%3DG572rfcAFGznOnMZ2DDgCaBJV%26oauth_nonce%3De0d92fb7d5264baf85321de413176e9d%26oauth_signature_method%3DHMAC-SHA1%26oauth_timestamp%3D1440899892%26oauth_version%3D1.0";
			Assert.Equal(expectedBaseString, baseString);

			string oauthSignature = sut.CreateSignature(requestParameters);

			// Second paramter from Twitter documentation: https://dev.twitter.com/oauth/overview/creating-signatures
			Assert.Equal(oauthSignature, "NIkCr9R68INkZ87vt9D3N6Gb8BY=");
		}
	}
}
