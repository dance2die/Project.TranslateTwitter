using System;
using System.Collections.Generic;
using System.Dynamic;
using System.IO;
using System.Net;
using System.Text;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using Project.TranslateTwitter.Core;

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
				//dynamic dynamicObject = JsonConvert.DeserializeObject<List<ExpandoObject>>(responseFromServer);
				dynamic dynamicObject = JsonConvert.DeserializeObject<List<ExpandoObject>>(responseFromServer, new ExpandoObjectConverter());
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

			var count = 5;

			var authenticationContext = new AuthenticationContext();
			TimelineRequestParameters requestParameters = new TimelineRequestParameters(authenticationContext)
			{
				ScreenName = screenName,
				Count = count.ToString(),
				//OAuthNonce = oauth_nonce,
				//OAuthTimestamp = oauth_timestamp
			};
			

			OAuthSignatureBuilder signatureBuilder = new OAuthSignatureBuilder(authenticationContext);
			string oauth_signature = signatureBuilder.CreateSignature(requestParameters);


			var headerFormat =
				"OAuth oauth_consumer_key=\"{0}\", oauth_nonce=\"{1}\", " + 
				"oauth_signature=\"{2}\", oauth_signature_method=\"{3}\", " +
				"oauth_timestamp=\"{4}\", " +
				"oauth_token=\"{5}\", " +
				"oauth_version=\"{6}\"";


			var authHeader = string.Format(headerFormat,
				Uri.EscapeDataString(oauth_consumer_key),
				Uri.EscapeDataString(requestParameters.OAuthNonce),
				Uri.EscapeDataString(oauth_signature),
				Uri.EscapeDataString(oauth_signature_method),
				Uri.EscapeDataString(requestParameters.OAuthTimestamp),
				Uri.EscapeDataString(oauth_token),
				Uri.EscapeDataString(oauth_version)
				);

			ServicePointManager.Expect100Continue = false;

			var queryUrl = requestParameters.BuildRequestUrl(requestParameters.ResourceUrl);
			var request = (HttpWebRequest)WebRequest.Create(queryUrl);
			request.Headers.Add("Authorization", authHeader);
			request.Method = requestParameters.HttpMethod;
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
}
