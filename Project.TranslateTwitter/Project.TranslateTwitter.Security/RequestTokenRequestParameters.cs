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
		private const string OAUTH_CALLBACK_HEADERNAME = "oauth_callback";

		public override string BaseUrl { get; set; } = "https://api.twitter.com/oauth/request_token";
		public override string HttpMethod { get; set; } = "POST";

		protected override NameValueCollection GetNonCommonParameters()
		{
			return new NameValueCollection();
		}

		public string OAuthCallbackHeader
		{
			get { return Headers[OAUTH_CALLBACK_HEADERNAME]; }
			set { Headers[OAUTH_CALLBACK_HEADERNAME] = value; }
		}

		public RequestTokenRequestParameters(IAuthenticationContext authenticationContext)
			: base(authenticationContext)
		{
		}
	}
}