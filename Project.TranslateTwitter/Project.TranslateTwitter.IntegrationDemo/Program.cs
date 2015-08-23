using System;
using System.Collections;
using System.Collections.Generic;
using System.Dynamic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using Project.TranslateTwitter.Security;

namespace Project.TranslateTwitter.IntegrationDemo
{
	public class Program
	{
		public static void Main(string[] args)
		{
			// Retrieve Tweet timeline.
			dynamic timeline = GetTweetTimeline();

			// Pick a non-English tweet and translate it
			dynamic nonEnglishTweet = GetNonEnglishTweet(timeline);
			//string translatedTweet = TranslateTweet(nonEnglishTweet);

			Console.Write("Press ENTER to continue...");
			Console.ReadLine();
		}

		private static dynamic GetTweetTimeline()
		{
			string screenName = "dance2die";
			var request = GetTimelineRequest(new AuthenticationContext(), screenName);

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

		//private static string TranslateTweet(dynamic nonEnglishTweet)
		//{

		//}
	}
}
