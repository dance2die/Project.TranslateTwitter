using System.Collections.Generic;

namespace Project.TranslateTwitter.Security
{
	public class RequestTokenRequestParameters : RequestParameters
	{
		/// <summary>
		/// Holds OAuth callback URL
		/// </summary>
		/// <remarks>
		/// Constant unique to this request parameter
		/// </remarks>
		private const string OAUTH_CALLBACK_HEADERNAME = "oauth_callback";
		private const string OAUTH_TOKEN_HEADERNAME = "oauth_token";
		private const string OAUTH_TOKEN_SECRET_HEADERNAME = "oauth_token_secret";

		public override string BaseUrl { get; set; } = "https://api.twitter.com/oauth/request_token";
		public override string HttpMethod { get; set; } = "POST";
		public override Dictionary<string, string> QueryProperties { get; set; } = new Dictionary<string, string>();
		public override Dictionary<string, string> BodyProperties { get; set; } = new Dictionary<string, string>();

		public string OAuthCallbackHeader
		{
			get { return Headers.Values[OAUTH_CALLBACK_HEADERNAME]; }
			set { Headers.Values[OAUTH_CALLBACK_HEADERNAME] = value; }
		}

		public RequestTokenRequestParameters(IAuthenticationContext authenticationContext, string oauthCallbackHeader)
			: base(authenticationContext)
		{
			OAuthCallbackHeader = oauthCallbackHeader;

			// Since this request is for retrieving Access Token and Access Token Secret, we need to clear it.
			AuthenticationContext.AccessToken = null;
			AuthenticationContext.AccessTokenSecret = null;

			// We are requesting to get OAuth Token so clear the value for this request.
			ClearOAuthToken(OAUTH_TOKEN_HEADERNAME);
			ClearOAuthToken(OAUTH_TOKEN_SECRET_HEADERNAME);
		}

		private void ClearOAuthToken(string tokenName)
		{
			Headers.Values.Remove(tokenName);
			QueryProperties.Remove(tokenName);
			BodyProperties.Remove(tokenName);
		}
	}
}