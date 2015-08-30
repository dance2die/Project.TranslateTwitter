using System.Collections.Generic;

namespace Project.TranslateTwitter.Security
{
	/// <summary>
	/// Request parameters for accessing "POST oauth/access_token"
	/// </summary>
	/// <see cref="https://dev.twitter.com/oauth/reference/post/oauth/access_token"/>
	/// <remarks>https://dev.twitter.com/oauth/reference/post/oauth/access_token</remarks>
	public class AccessTokenRequestParameters : RequestParameters
	{
		private const string OAUTH_VERIFIER_BODYNAME = "oauth_verifier";

		public override string BaseUrl { get; set; } = "https://api.twitter.com/oauth/access_token";
		public override string HttpMethod { get; set; } = "POST";
		public override Dictionary<string, string> QueryProperties { get; set; } = new Dictionary<string, string>(0);
		public override Dictionary<string, string> BodyProperties { get; set; } = new Dictionary<string, string>();

		public string OAuthVerifier
		{
			get { return BodyProperties[OAUTH_VERIFIER_BODYNAME]; }
			set
			{
				BodyProperties[OAUTH_VERIFIER_BODYNAME] = value;
				//Headers.Values[OAUTH_VERIFIER_BODYNAME] = value;
			}
		}

		public AccessTokenRequestParameters(IAuthenticationContext authenticationContext, string oauthVerifier) 
			: base(authenticationContext)
		{
			OAuthVerifier = oauthVerifier;
		}
	}
}
