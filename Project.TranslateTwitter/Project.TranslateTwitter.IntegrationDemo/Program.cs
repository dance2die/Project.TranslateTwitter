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
			var authenticationContext = new AuthenticationContext();

			TestTimeline(authenticationContext);
			//TestSignInWithTwitter(authenticationContext);

			Console.Write("Press ENTER to continue...");
			Console.ReadLine();
		}

		private static void TestSignInWithTwitter(AuthenticationContext authenticationContext)
		{
			HttpWebRequest request = GetRequestTokenRequest(new AuthenticationContext());
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

		private static HttpWebRequest GetRequestTokenRequest(AuthenticationContext authenticationContext)
		{
			var requestBuilder = new RequestBuilder(authenticationContext);
			var requestParameters = new RequestTokenRequestParameters(authenticationContext)
			{
				OAuthCallbackHeader = "http://localhost"
			};

			return requestBuilder.GetRequest(requestParameters);
		}

		private static void TestTimeline(AuthenticationContext authenticationContext)
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

		private static dynamic GetTweetTimeline(AuthenticationContext authenticationContext)
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

		private static HttpWebRequest GetTimelineRequest(AuthenticationContext authenticationContext, string screenName)
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
}
