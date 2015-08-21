using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Compat.Web;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http.Headers;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace Project.TranslateTwitter.Security.Demo
{
	public class Program3
	{
		public static void Main(string[] args)
		{
			var screenName = "Pleasure54";	// This account uses korena & japanese

			TestUserTimeLine(screenName);
		}

		private static void TestUserTimeLine(string screenName)
		{
			var request = GetUserTimelineRequest(screenName);

            using (HttpWebResponse response = request.GetResponse() as HttpWebResponse)
			using (Stream dataStream = response.GetResponseStream())
			{
				//Open the stream using a StreamReader for easy access.
				StreamReader reader = new StreamReader(dataStream);
				//Read the content.
				string responseFromServer = reader.ReadToEnd();
			}
		}

		private static HttpWebRequest GetUserTimelineRequest(string screenName)
		{
			var oauth_token = OAuthProperties.AccessToken;
			var oauth_token_secret = OAuthProperties.AccessTokenSecret;
			var oauth_consumer_key = OAuthProperties.ConsumerKey;
			var oauth_consumer_secret = OAuthProperties.ConsumerKeySecret;

			var oauth_version = "1.0";
			var oauth_signature_method = "HMAC-SHA1";
			var oauth_nonce = Convert.ToBase64String(new ASCIIEncoding().GetBytes(DateTime.Now.Ticks.ToString()));
			var timeSpan = DateTime.UtcNow- new DateTime(1970, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc);
			var oauth_timestamp = Convert.ToInt64(timeSpan.TotalSeconds).ToString();
			var resource_url = "https://api.twitter.com/1.1/statuses/user_timeline.json";

			var count = 5;
			var httpMethod = "GET";

			//var baseFormat = "count={0}&oauth_consumer_key={1}&oauth_nonce={2}&oauth_signature_method={3}" +
			//				 "&oauth_timestamp={4}&oauth_token={5}&oauth_version={6}&screen_name={7}";

			//var baseString = string.Format(baseFormat,
			//	count,
			//	oauth_consumer_key,
			//	oauth_nonce,
			//	oauth_signature_method,
			//	oauth_timestamp,
			//	oauth_token,
			//	oauth_version,
			//	screenName
			//);


			//oauth_nonce = "ed2e216240c5da8e6e6e9bfb21260323";
			//oauth_timestamp = "1440180541";



			var authenticationContext = new AuthenticationContext();
			TimelineRequestParameters requestParameters = new TimelineRequestParameters(authenticationContext)
			{
				ScreenName = screenName,
				Count = count.ToString(),
				OAuthNonce = oauth_nonce,
				OAuthTimestamp = oauth_timestamp
			};
			
			
			//baseString = string.Concat(httpMethod + "&", Uri.EscapeDataString(queryUrl), "&", Uri.EscapeDataString(baseString));

			OAuthSignatureBuilder signatureBuilder = new OAuthSignatureBuilder(authenticationContext);
			//var bstring = signatureBuilder.GetSignatureBaseString(new SignatureInput(httpMethod, resource_url, requestParameters.Parameters));
			//var signatureInput = new SignatureInput(httpMethod, queryUrl, requestParameters.Parameters);
			var signatureInput = new SignatureInput(httpMethod, resource_url, requestParameters.Parameters);
			//var bstring = signatureBuilder.GetSignatureBaseString(signatureInput);
			//baseString = bstring;


			//string oauth_signature = signatureBuilder.CreateSignature(baseString);
			string oauth_signature = signatureBuilder.CreateSignature(signatureInput);


			var headerFormat =
				"OAuth oauth_consumer_key=\"{0}\", oauth_nonce=\"{1}\", " + 
				"oauth_signature=\"{2}\", oauth_signature_method=\"{3}\", " +
				"oauth_timestamp=\"{4}\", " +
				"oauth_token=\"{5}\", " +
				"oauth_version=\"{6}\"";


			var authHeader = string.Format(headerFormat,
				Uri.EscapeDataString(oauth_consumer_key),
				Uri.EscapeDataString(oauth_nonce),
				Uri.EscapeDataString(oauth_signature),
				Uri.EscapeDataString(oauth_signature_method),
				Uri.EscapeDataString(oauth_timestamp),
				Uri.EscapeDataString(oauth_token),
				Uri.EscapeDataString(oauth_version)
				);

			ServicePointManager.Expect100Continue = false;

			var queryUrl = requestParameters.BuildRequestUrl(resource_url);
			var request = (HttpWebRequest)WebRequest.Create(queryUrl);
			request.Headers.Add("Authorization", authHeader);
			request.Method = httpMethod;
			request.ContentType = "application/x-www-form-urlencoded";

			return request;
		}

		private static Dictionary<string, string> GetRequestParams()
		{
			var oauth_consumer_key = OAuthProperties.ConsumerKey;
			var oauth_signature_method = "HMAC-SHA1";
			var oauth_nonce = Convert.ToBase64String(new ASCIIEncoding().GetBytes(DateTime.Now.Ticks.ToString()));
			var oauth_timestamp = GetTimeStamp();
			var oauth_token = OAuthProperties.AccessToken;
			var oauth_version = "1.0";

			Dictionary<string, string> parameters = new Dictionary<string, string>
			{
				{"oauth_consumer_key", oauth_consumer_key },
				{"oauth_nonce", oauth_nonce },
				{"oauth_signature_method", oauth_signature_method },
				{"oauth_timestamp", oauth_timestamp },
				{"oauth_token", oauth_token },
				{"oauth_version", oauth_version },
			};

			// According to Twitter spec,
			// Sort the list of parameters alphabetically[1] by encoded key[2].
			//return parameters.OrderBy(pair => pair.Key).ToDictionary(pair => pair.Key, pair => pair.Value);

			return parameters;
		}

		private static string GetTimeStamp()
		{
			var timeSpan = DateTime.UtcNow - new DateTime(1970, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc);
			return Convert.ToInt64(timeSpan.TotalSeconds).ToString();
		}
	}

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
				//{"oauth_nonce", "084ac3305ffe7e8022744f7a7a07db17" },
				{"oauth_nonce", oauth_nonce },
				{"oauth_signature_method", oauth_signature_method },
				{"oauth_timestamp", oauth_timestamp },
				//{"oauth_timestamp", "1440010872" },
				{"oauth_token", oauth_token },
				{"oauth_version", oauth_version },
			};
		}

		private string GetTimeStamp()
		{
			var timeSpan = DateTime.UtcNow - new DateTime(1970, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc);
			return Convert.ToInt64(timeSpan.TotalSeconds).ToString();
		}

		/// <summary>
		/// 
		/// </summary>
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
