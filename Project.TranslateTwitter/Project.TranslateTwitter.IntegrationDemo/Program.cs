using System;
using System.Collections;
using System.Collections.Generic;
using System.Dynamic;
using System.IO;
using System.Linq;
using System.Net;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using Project.TranslateTwitter.Security;
using Project.TranslateTwitter.Translator.Microsoft.Commands;

namespace Project.TranslateTwitter.IntegrationDemo
{
	public class Program
	{
		public static void Main(string[] args)
		{
			IAuthenticationContext authenticationContext = new AuthenticationContext();
			//authenticationContext = new TestAuthenticationContext();

			//TestTimeline(authenticationContext);
			//TestSignInWithTwitter(authenticationContext);
			TestStatusUpdate(authenticationContext);

			Console.Write("Press ENTER to continue...");
			Console.ReadLine();
		}

		private static void TestStatusUpdate(IAuthenticationContext authenticationContext)
		{
			HttpWebRequest request = GetUpdateStatusRequest(authenticationContext);
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

		private static HttpWebRequest GetUpdateStatusRequest(IAuthenticationContext authenticationContext)
		{
			var requestBuilder = new RequestBuilder(authenticationContext);
			var requestParameters = new UpdateStatusRequestParameters(authenticationContext, $"Testing Twitter API - {DateTime.Now.ToShortDateString()}");

			return requestBuilder.GetRequest(requestParameters);
		}

		private static void TestSignInWithTwitter(IAuthenticationContext authenticationContext)
		{
			HttpWebRequest request = GetRequestTokenRequest(authenticationContext);
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

		private static HttpWebRequest GetRequestTokenRequest(IAuthenticationContext authenticationContext)
		{
			var requestBuilder = new RequestBuilder(authenticationContext);
			var requestParameters = new RequestTokenRequestParameters(authenticationContext, "http://localhost/sign-in-with-twitter/");
			//RequestParameters requestParameters = new TimelineRequestParameters(authenticationContext);
			//requestParameters = new TestRequestParameters(authenticationContext);
			//requestParameters.CommonParameters = GetTestRequestParams();

			return requestBuilder.GetRequest(requestParameters);
		}

		private static Dictionary<string, string> GetTestRequestParams()
		{
			Dictionary<string, string> parameters = new Dictionary<string, string>
			{
				{"include_entities", "true" },
				{"status", "Hello Ladies + Gentlemen, a signed OAuth request!" },
				{"oauth_consumer_key", "xvz1evFS4wEEPTGEFPHBog" },
				{"oauth_nonce", "kYjzVBB8Y0ZFabxSWbWovY3uYSQ2pTgmZeNu2VS4cg" },
				{"oauth_signature_method", "HMAC-SHA1" },
				{"oauth_timestamp", "1318622958" },
				{"oauth_token", "370773112-GmHxMAgYyLbNEtIKZeRNFsMKPR9EyMZeS9weJAEb" },
				{"oauth_version", "1.0" },
			};

			return parameters;
		}


		private static void TestTimeline(IAuthenticationContext authenticationContext)
		{
			// Retrieve Tweet timeline.
			dynamic timeline = GetTweetTimeline(authenticationContext);

			// Pick a non-English tweet and translate it
			dynamic nonEnglishTweet = GetNonEnglishTweet(timeline);
			string originalText = GetTweetText(nonEnglishTweet);
			string translatedTweet = TranslateTweet(nonEnglishTweet);

			Console.WriteLine(originalText);
			Console.WriteLine("--- to ---");
			Console.WriteLine(translatedTweet);
		}

		private static dynamic GetTweetTimeline(IAuthenticationContext authenticationContext)
		{
			string screenName = "dance2die";
			var request = GetTimelineRequest(authenticationContext, screenName);

			using (HttpWebResponse response = request.GetResponse() as HttpWebResponse)
			using (Stream dataStream = response.GetResponseStream())
			{
				//Open the stream using a StreamReader for easy access.
				StreamReader reader = new StreamReader(dataStream);
				//Read the content.
				string responseFromServer = reader.ReadToEnd();
				dynamic dynamicObject = JsonConvert.DeserializeObject<List<ExpandoObject>>(
					responseFromServer, new ExpandoObjectConverter());
				return dynamicObject;
			}
		}

		private static HttpWebRequest GetTimelineRequest(IAuthenticationContext authenticationContext, string screenName)
		{
			var requestBuilder = new RequestBuilder(authenticationContext);
			var requestParameters = new TimelineRequestParameters(authenticationContext)
			{
				ScreenName = screenName,
				Count = "10",
			};

			return requestBuilder.GetRequest(requestParameters);
		}

		private static dynamic GetNonEnglishTweet(dynamic timeline)
		{
			const string englishLanguageCode = "en";
			IEnumerable enumerable = timeline as IEnumerable;
			return enumerable?.Cast<dynamic>().FirstOrDefault(obj => obj.lang != englishLanguageCode);
		}

		private static string GetTweetText(dynamic tweet)
		{
			if (!ExistsProperty(tweet, "text"))
				throw new Exception("\"text\" property doesn't exist!");
			return tweet.text;
		}

		private static string TranslateTweet(dynamic nonEnglishTweet)
		{
			if (!ExistsProperty(nonEnglishTweet, "lang")) throw new Exception("\"lang\" property doesn't exist!");

			string textToTranslate = GetTweetText(nonEnglishTweet);
			string detectedLanguage = nonEnglishTweet.lang;

			string clientId = "Project_TranslateTwitter";
			string clientSecret = Environment.GetEnvironmentVariable(
				"Project_TranslateTwitter.ClientSecret", EnvironmentVariableTarget.User);
			var authenticationContext = new Translator.Microsoft.Auth.AuthenticationContext(clientId, clientSecret);

			var translator = new TranslatorCommand(authenticationContext,
				new LanguageTranslatorArg(textToTranslate, detectedLanguage));
			translator.Execute();
			var translatedText = translator.Result;
			return translatedText;
		}

		/// <remarks>
		/// http://stackoverflow.com/a/2839629/4035
		/// </remarks>
		public static bool ExistsProperty(dynamic settings, string propertyName)
		{
			return ((IDictionary<string, object>)settings).ContainsKey(propertyName);
		}
	}

	/// <summary>
	/// https://dev.twitter.com/oauth/overview/creating-signatures
	/// </summary>
	internal class TestRequestParameters : RequestParameters
	{
		public override string BaseUrl { get; set; } = "https://api.twitter.com/1/statuses/update.json";
		public override string HttpMethod { get; set; } = "POST";

		public override Dictionary<string, string> QueryProperties { get; set; } = new Dictionary<string, string>{{"include_entities", "true"}};
		public override Dictionary<string, string> BodyProperties { get; set; } = new Dictionary<string, string>(0);

		public TestRequestParameters(IAuthenticationContext authenticationContext)
			: base(authenticationContext)
		{
		}
	}
}
