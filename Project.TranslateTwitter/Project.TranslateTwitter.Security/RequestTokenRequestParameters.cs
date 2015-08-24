using System.Collections.Specialized;

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
		private const string OAUTH_CALLBACK_PARAMETERNAME = "oauth_callback";

		public override string ResourceUrl { get; set; } = "https://api.twitter.com/oauth/request_token";
		public override string HttpMethod { get; set; } = "POST";

		protected override NameValueCollection GetNonCommonParameters()
		{
			return new NameValueCollection
			{
				[OAUTH_CALLBACK_PARAMETERNAME] = OAuthCallback
			};
		}

		public string OAuthCallback
		{
			get { return CommonParameters[OAUTH_CALLBACK_PARAMETERNAME]; }
			set { CommonParameters[OAUTH_CALLBACK_PARAMETERNAME] = value; }
		}


		public RequestTokenRequestParameters(IAuthenticationContext authenticationContext)
			: base(authenticationContext)
		{
		}
	}
}