using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Dynamic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Mime;
using System.Threading;
using System.Windows.Forms;
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

			//TestTimeline(authenticationContext);
			//TestStatusUpdate(authenticationContext);
			TestSignInWithTwitter(authenticationContext);

			Console.Write("Press ENTER to continue...");
			Console.ReadLine();
		}

		private static void TestSignInWithTwitter(IAuthenticationContext authenticationContext)
		{
			Dictionary<string, string> requestTokens = GetRequestTokens(authenticationContext);
			string oauthToken = requestTokens["oauth_token"];

			HttpWebRequest request = GetAuthenticationRequest(authenticationContext, oauthToken);

			//RunBrowserThread(request.RequestUri);
			var process = Process.Start(request.RequestUri.ToString());
			//process.OutputDataReceived += Process_OutputDataReceived;

			//using (HttpWebResponse response = request.GetResponse() as HttpWebResponse)
			//using (Stream responseStream = response.GetResponseStream())
			//using (StreamReader streamReader = new StreamReader(responseStream))
			//{
			//	string responseFromServer = streamReader.ReadToEnd();
			//	Console.WriteLine(responseFromServer);
			//}
		}

		private static void Process_OutputDataReceived(object sender, DataReceivedEventArgs e)
		{
			Console.WriteLine(e.Data);
		}

		/// <remarks>http://stackoverflow.com/a/4271581/4035</remarks>
		private static void RunBrowserThread(Uri url)
		{
			var th = new Thread(() => {
				var br = new WebBrowser();
				br.DocumentCompleted += browser_DocumentCompleted;
				br.Navigate(url);
				Application.Run();
			});
			th.SetApartmentState(ApartmentState.STA);
			th.Start();
		}

		/// <remarks>http://stackoverflow.com/a/4271581/4035</remarks>
		private static void browser_DocumentCompleted(object sender, WebBrowserDocumentCompletedEventArgs e)
		{
			var br = sender as WebBrowser;
			if (br.Url == e.Url)
			{
				Console.WriteLine("Natigated to {0}", e.Url);
				Application.ExitThread();   // Stops the thread
			}
		}

		private static HttpWebRequest GetAuthenticationRequest(
			IAuthenticationContext authenticationContext, string oauthToken)
		{
			RequestParameters authenticateRequestParameters = 
				new AuthenticateRequestParameter(authenticationContext, oauthToken);
			return GetWebRequest(authenticationContext, authenticateRequestParameters);
		}

		private static Dictionary<string, string> GetRequestTokens(IAuthenticationContext authenticationContext)
		{
			HttpWebRequest request = GetRequestTokenRequest(authenticationContext);
			using (HttpWebResponse response = request.GetResponse() as HttpWebResponse)
			using (Stream dataStream = response.GetResponseStream())
			{
				//Open the stream using a StreamReader for easy access.
				StreamReader reader = new StreamReader(dataStream);
				//Read the content.
				string responseFromServer = reader.ReadToEnd();

				var tokens = responseFromServer.Split(new[] {"&"}, StringSplitOptions.RemoveEmptyEntries);
				return tokens.ToDictionary(s => s.Split('=')[0], s => s.Split('=')[1]);
			}
		}

		private static HttpWebRequest GetRequestTokenRequest(IAuthenticationContext authenticationContext)
		{
			var requestParameters = new RequestTokenRequestParameters(
				authenticationContext, "http://localhost/sign-in-with-twitter/");
			//RequestParameters requestParameters = new TimelineRequestParameters(authenticationContext);
			//requestParameters = new TestRequestParameters(authenticationContext);
			//requestParameters.CommonParameters = GetTestRequestParams();

			return GetWebRequest(authenticationContext, requestParameters);
		}

		private static HttpWebRequest GetWebRequest(
			IAuthenticationContext authenticationContext, RequestParameters requestParameters)
		{
			var requestBuilder = new RequestBuilder(authenticationContext);
			return requestBuilder.GetRequest(requestParameters);
		}

		private static void TestStatusUpdate(IAuthenticationContext authenticationContext)
		{
			HttpWebRequest request = GetUpdateStatusRequest(authenticationContext);
			using (HttpWebResponse response = request.GetResponse() as HttpWebResponse)
			using (Stream dataStream = response.GetResponseStream())
			using (StreamReader reader = new StreamReader(dataStream))
			{
				var responseFromServer = reader.ReadToEnd();
				//dynamic dynamicObject = JsonConvert.DeserializeObject<List<ExpandoObject>>(
				//	responseFromServer, new ExpandoObjectConverter());

				Console.WriteLine(responseFromServer);
			}
		}

		private static HttpWebRequest GetUpdateStatusRequest(IAuthenticationContext authenticationContext)
		{
			var requestBuilder = new RequestBuilder(authenticationContext);
			RequestParameters requestParameters = new UpdateStatusRequestParameters(authenticationContext,
				$"Testing Twitter API - {DateTime.Now.ToString("MM/dd/yyyy hh:mm:ss tt")}");

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
			if (!HasDynamicProperty(tweet, "text"))
				throw new Exception("\"text\" property doesn't exist!");
			return tweet.text;
		}

		private static string TranslateTweet(dynamic nonEnglishTweet)
		{
			if (!HasDynamicProperty(nonEnglishTweet, "lang")) throw new Exception("\"lang\" property doesn't exist!");

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
		public static bool HasDynamicProperty(dynamic settings, string propertyName)
		{
			return ((IDictionary<string, object>)settings).ContainsKey(propertyName);
		}
	}
}
