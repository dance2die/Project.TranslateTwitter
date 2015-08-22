using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Text;
using System.Web;

namespace Project.TranslateTwitter.Security
{
	public abstract class RequestParameters
	{
		protected const string SCREEN_NAME = "screen_name";
		protected const string COUNT = "count";
		protected const string OAUTH_NONCE = "oauth_nonce";
		protected const string OAUTH_TIMESTAMP = "oauth_timestamp";

		public Dictionary<string, string> CommonParameters { get; set; }
		public IAuthenticationContext AuthenticationContext { get; set; }

		public string OAuthNonce
		{
			get { return CommonParameters[OAUTH_NONCE]; }
			set { CommonParameters[OAUTH_NONCE] = value; }
		}

		public string OAuthTimestamp
		{
			get { return CommonParameters[OAUTH_TIMESTAMP]; }
			set { CommonParameters[OAUTH_TIMESTAMP] = value; }
		}

		protected RequestParameters(IAuthenticationContext authenticationContext)
		{
			AuthenticationContext = authenticationContext;
			CommonParameters = GetCommonParameters();
		}

		public abstract string ResourceUrl { get; set; }
		public abstract string HttpMethod { get; set; }
		public abstract string BuildRequestUrl(string apiUrl);

		protected Dictionary<string, string> GetCommonParameters()
		{
			var oauthConsumerKey = AuthenticationContext.ConsumerKey;
			var oauthNonce = Convert.ToBase64String(Encoding.ASCII.GetBytes(DateTime.Now.Ticks.ToString()));
			var oauthTimestamp = GetTimeStamp();
			var oauthToken = AuthenticationContext.AccessToken;

			return new Dictionary<string, string>
			{
				{"oauth_consumer_key", oauthConsumerKey },
				{"oauth_nonce", oauthNonce },
				{"oauth_signature_method", OAuthDefaults.SignatureMethod },
				{"oauth_timestamp", oauthTimestamp },
				{"oauth_token", oauthToken },
				{"oauth_version", OAuthDefaults.Version }
			};
		}

		protected string GetQueryString(NameValueCollection query)
		{
			var array = (from key in query.AllKeys
				from value in query.GetValues(key)
				orderby key
				where !string.IsNullOrWhiteSpace(value)
				select $"{HttpUtility.UrlEncode(key)}={HttpUtility.UrlEncode(value)}").ToArray();
			return string.Join("&", array);
		}

		private string GetTimeStamp()
		{
			var timeSpan = DateTime.UtcNow - new DateTime(1970, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc);
			return Convert.ToInt64(timeSpan.TotalSeconds).ToString();
		}
	}
}