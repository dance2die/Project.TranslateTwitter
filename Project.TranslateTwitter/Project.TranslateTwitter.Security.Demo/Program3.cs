using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
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

			var count = 10;
			var httpMethod = "GET";

			var baseFormat = "count={0}&oauth_consumer_key={1}&oauth_nonce={2}&oauth_signature_method={3}" +
							 "&oauth_timestamp={4}&oauth_token={5}&oauth_version={6}&screen_name={7}";

			var baseString = string.Format(baseFormat,
				count,
				oauth_consumer_key,
				oauth_nonce,
				oauth_signature_method,
				oauth_timestamp,
				oauth_token,
				oauth_version,
				screenName
			);

            baseString = string.Concat(httpMethod + "&", Uri.EscapeDataString(resource_url),
				"&", Uri.EscapeDataString(baseString));

			OAuthSignatureBuilder signatureBuilder = new OAuthSignatureBuilder(new AuthenticationContext());
			string oauth_signature = signatureBuilder.CreateSignature(baseString);

			var headerFormat =
				"OAuth oauth_nonce=\"{0}\", oauth_signature_method=\"{1}\", " +
				"oauth_timestamp=\"{2}\", oauth_consumer_key=\"{3}\", " +
				"oauth_token=\"{4}\", oauth_signature=\"{5}\", " +
				"oauth_version=\"{6}\"";

			var authHeader = string.Format(headerFormat,
				Uri.EscapeDataString(oauth_nonce),
				Uri.EscapeDataString(oauth_signature_method),
				Uri.EscapeDataString(oauth_timestamp),
				Uri.EscapeDataString(oauth_consumer_key),
				Uri.EscapeDataString(oauth_token),
				Uri.EscapeDataString(oauth_signature),
				Uri.EscapeDataString(oauth_version)
				);

			ServicePointManager.Expect100Continue = false;

			var request = (HttpWebRequest)WebRequest.Create(resource_url);
			request.Headers.Add("Authorization", authHeader);
			request.Method = httpMethod;
			request.ContentType = "application/x-www-form-urlencoded";

			return request;
		}
	}
}
