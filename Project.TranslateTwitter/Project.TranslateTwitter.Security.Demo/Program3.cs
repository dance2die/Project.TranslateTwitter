using System;
using System.Collections.Generic;
using System.Dynamic;
using System.IO;
using System.Net;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using Project.TranslateTwitter.Core;

namespace Project.TranslateTwitter.Security.Demo
{
	public class Program3
	{
		public static void Main(string[] args)
		{
			var screenName = "Pleasure54";  // This account uses korena & japanese

			TestUserTimeLine(screenName);
		}

		private static void TestUserTimeLine(string screenName)
		{
			var count = 5;
			var authenticationContext = new AuthenticationContext();
			TimelineRequestParameters requestParameters = new TimelineRequestParameters(authenticationContext)
			{
				ScreenName = screenName,
				Count = count.ToString(),
			};
			var request = GetUserTimelineRequest(authenticationContext, requestParameters);

			using (HttpWebResponse response = request.GetResponse() as HttpWebResponse)
			using (Stream dataStream = response.GetResponseStream())
			{
				//Open the stream using a StreamReader for easy access.
				StreamReader reader = new StreamReader(dataStream);
				//Read the content.
				string responseFromServer = reader.ReadToEnd();
				dynamic dynamicObject = JsonConvert.DeserializeObject<List<ExpandoObject>>(
					responseFromServer, new ExpandoObjectConverter());
			}
		}

		private static HttpWebRequest GetUserTimelineRequest(
			AuthenticationContext authenticationContext, TimelineRequestParameters requestParameters)
		{
			var oauthVersion = "1.0";
			var oauthSignatureMethod = "HMAC-SHA1";

			OAuthSignatureBuilder signatureBuilder = new OAuthSignatureBuilder(authenticationContext);
			string oauthSignature = signatureBuilder.CreateSignature(requestParameters);

			var headerFormat =
				"OAuth oauth_consumer_key=\"{0}\", oauth_nonce=\"{1}\", " +
				"oauth_signature=\"{2}\", oauth_signature_method=\"{3}\", " +
				"oauth_timestamp=\"{4}\", " +
				"oauth_token=\"{5}\", " +
				"oauth_version=\"{6}\"";


			var authHeader = string.Format(headerFormat,
				Uri.EscapeDataString(authenticationContext.ConsumerKey),
				Uri.EscapeDataString(requestParameters.OAuthNonce),
				Uri.EscapeDataString(oauthSignature),
				Uri.EscapeDataString(oauthSignatureMethod),
				Uri.EscapeDataString(requestParameters.OAuthTimestamp),
				Uri.EscapeDataString(authenticationContext.AccessToken),
				Uri.EscapeDataString(oauthVersion)
				);

			ServicePointManager.Expect100Continue = false;

			var queryUrl = requestParameters.BuildRequestUrl(requestParameters.ResourceUrl);
			var request = (HttpWebRequest)WebRequest.Create(queryUrl);
			request.Headers.Add("Authorization", authHeader);
			request.Method = requestParameters.HttpMethod;
			request.ContentType = "application/x-www-form-urlencoded";

			return request;
		}
	}
}
