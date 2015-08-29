using System.Collections.Generic;

namespace Project.TranslateTwitter.Security
{
	/// <summary>
	/// Request Parameter for "GET oauth/authenticate"
	/// </summary>
	/// <remarks>
	/// https://dev.twitter.com/oauth/reference/get/oauth/authenticate
	/// </remarks>
	public class AuthenticateRequestParameters : RequestParameters
	{
		private const string OAUTH_TOKEN_QUERYNAME = "oauth_token";

		public override string BaseUrl { get; set; } = "https://api.twitter.com/oauth/authenticate";
		public override string HttpMethod { get; set; } = "GET";
		public override Dictionary<string, string> BodyProperties { get; set; } = new Dictionary<string, string>(0);
		public override Dictionary<string, string> QueryProperties { get; set; } = new Dictionary<string, string>();

		public string OAuthToken
		{
			get { return QueryProperties[OAUTH_TOKEN_QUERYNAME]; }
			set { QueryProperties[OAUTH_TOKEN_QUERYNAME] = value; }
		}

		public AuthenticateRequestParameters(IAuthenticationContext authenticationContext, string oauthToken)
			: base(authenticationContext)
		{
			OAuthToken = oauthToken;
		}
	}
}
