using System.Collections.Generic;
using Project.TranslateTwitter.Security;
using Xunit;

namespace Project.TranslateTwitter.Integration.Test
{
	public class AuthenticationContextTest
	{
		private readonly IAuthenticationContext _authenticationContext;

		public AuthenticationContextTest()
		{
			_authenticationContext = new EmptyAuthenticationContext
			{
				ConsumerKey = "consumer_key",
				ConsumerKeySecret = "consumer_key_secret",
				AccessToken = "access_token",
				AccessTokenSecret = "access_token_secret"
			};
		}

		[Fact]
		public void TestMergeWithAuthenticationContext()
		{
			var sut = new EmptyAuthenticationContext();
			sut.MergeWith(_authenticationContext);

			Assert.Equal(_authenticationContext.ConsumerKey, sut.ConsumerKey);
			Assert.Equal(_authenticationContext.ConsumerKeySecret, sut.ConsumerKeySecret);
			Assert.Equal(_authenticationContext.AccessToken, sut.AccessToken);
			Assert.Equal(_authenticationContext.AccessTokenSecret, sut.AccessTokenSecret);
		}

		[Fact]
		public void TestMergeWithDictionary()
		{
			var sut = new EmptyAuthenticationContext();
			IDictionary<string, string> dictoinary = new Dictionary<string, string>
			{
				{ "oauth_token", "oauth_token_value" },
				{ "oauth_token_secret", "oauth_token_secret_value" }
			};
			sut.MergeWith(dictoinary);

			Assert.Equal("oauth_token_value", sut.AccessToken);
			Assert.Equal("oauth_token_secret_value", sut.AccessTokenSecret);
		}
	}
}
