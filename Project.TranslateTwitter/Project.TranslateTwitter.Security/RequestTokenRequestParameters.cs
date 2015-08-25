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

		public override string BaseUrl { get; set; } = "https://api.twitter.com/oauth/request_token";
		public override string HttpMethod { get; set; } = "POST";

		public string OAuthCallbackHeader
		{
			get { return Headers.Values[OAUTH_CALLBACK_HEADERNAME]; }
			set { Headers.Values[OAUTH_CALLBACK_HEADERNAME] = value; }
		}

		public RequestTokenRequestParameters(IAuthenticationContext authenticationContext, string oauthCallbackHeader)
			: base(authenticationContext)
		{
			OAuthCallbackHeader = oauthCallbackHeader;
		}

		public override Dictionary<string, string> GetQueryProperties()
		{
			return new Dictionary<string, string>(0);
		}
	}
}