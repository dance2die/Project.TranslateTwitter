using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Web;

namespace Project.TranslateTwitter.Core
{
	public class TimelineRequestParameters
	{
		private const string SCREEN_NAME = "screen_name";
		private const string COUNT = "count";
		private const string OAUTH_NONCE = "oauth_nonce";
		private const string OAUTH_TIMESTAMP = "oauth_timestamp";

		public Dictionary<string, string> Parameters { get; }
		public IAuthenticationContext AuthenticationContext { get; }

		public string ScreenName
		{
			get { return Parameters[SCREEN_NAME]; }
			set { Parameters[SCREEN_NAME] = value; }
		}

		public string Count
		{
			get { return Parameters[COUNT]; }
			set { Parameters[COUNT] = value; }
		}

		public string OAuthNonce
		{
			get { return Parameters[OAUTH_NONCE]; }
			set { Parameters[OAUTH_NONCE] = value; }
		}

		public string OAuthTimestamp
		{
			get { return Parameters[OAUTH_TIMESTAMP]; }
			set { Parameters[OAUTH_TIMESTAMP] = value; }
		}

		public string ResourceUrl { get; set; } = "https://api.twitter.com/1.1/statuses/user_timeline.json";
		public string HttpMethod { get; set; } = "GET";


		public TimelineRequestParameters(IAuthenticationContext authenticationContext)
		{
			AuthenticationContext = authenticationContext;
			Parameters = GetCommonParameters();

			Count = "5";
		}

		private Dictionary<string, string> GetCommonParameters()
		{
			var oauth_consumer_key = AuthenticationContext.ConsumerKey;
			var oauth_signature_method = "HMAC-SHA1";
			var oauth_nonce = Convert.ToBase64String(new ASCIIEncoding().GetBytes(DateTime.Now.Ticks.ToString()));
			var oauth_timestamp = GetTimeStamp();
			var oauth_token = AuthenticationContext.AccessToken;
			var oauth_version = "1.0";

			//oauth_nonce = "4339355b1dd5c6fcb025ccebe6d50f67";
			//oauth_timestamp = "1439914884";

			return new Dictionary<string, string>
			{
				{"oauth_consumer_key", oauth_consumer_key },
				{"oauth_nonce", oauth_nonce },
				{"oauth_signature_method", oauth_signature_method },
				{"oauth_timestamp", oauth_timestamp },
				{"oauth_token", oauth_token },
				{"oauth_version", oauth_version },
			};
		}

		private string GetTimeStamp()
		{
			var timeSpan = DateTime.UtcNow - new DateTime(1970, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc);
			return Convert.ToInt64(timeSpan.TotalSeconds).ToString();
		}

		/// <param name="apiUrl">Twitter API URL</param>
		/// <returns></returns>
		public string BuildRequestUrl(string apiUrl)
		{
			//var uriBuilder = new UriBuilder(apiUrl);

			NameValueCollection query = new NameValueCollection
			{
				[SCREEN_NAME] = ScreenName,
				[COUNT] = Count.ToString(CultureInfo.InvariantCulture)
			};

			var queryString = GetQueryString(query);
			var result = $"{apiUrl}?{HttpUtility.UrlDecode(queryString)}";

			return result;
		}

		private string GetQueryString(NameValueCollection query)
		{
			var array = (from key in query.AllKeys
				from value in query.GetValues(key)
				orderby key
				where !string.IsNullOrWhiteSpace(value)
				select $"{HttpUtility.UrlEncode(key)}={HttpUtility.UrlEncode(value)}").ToArray();
			return string.Join("&", array);
		}
	}
}