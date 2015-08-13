﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using LinqToTwitter;
using Nito.AsyncEx;
using TweetSharp;

namespace Project.TranslateTwitter.Security.Demo
{
	public class Program
	{
		public static void Main(string[] args)
		{
			//AsyncContext.Run(() => TestSignInWithXAuth());

			//TestWithTweetSharpXAuth();

			// copied from Project.EncryptTwitter
			// Need to roll out my own auth library.
			Test1();
		}

		private static void Test1()
		{
			// Authenticate to Twitter using OAuth
			/// http://www.codeproject.com/Articles/247336/Twitter-OAuth-authentication-using-Net

			var status = "Tired from testing many Twitter libraries. #tired";
			var postBody = "status=" + Uri.EscapeDataString(status);
			var request = GetTwitterWebRequest(status);
			using (Stream stream = request.GetRequestStream())
			{
				byte[] content = Encoding.ASCII.GetBytes(postBody);
				stream.Write(content, 0, content.Length);
			}

			WebResponse response = request.GetResponse();
			using (var reader = new StreamReader(response.GetResponseStream()))
			{
				string objText = reader.ReadToEnd();
				//JavaScriptSerializer js = new JavaScriptSerializer();
				//MyObject myojb = (MyObject)js.Deserialize(objText, typeof(MyObject));
			}
		}


		private static HttpWebRequest GetTwitterWebRequest(string status)
		{
			var oauth_token = OAuthProperties.Token;
			var oauth_token_secret = OAuthProperties.TokenSecret;
			var oauth_consumer_key = OAuthProperties.ConsumerKey;
			var oauth_consumer_secret = OAuthProperties.ConsumerKeySecret;

			var oauth_version = "1.0";
			var oauth_signature_method = "HMAC-SHA1";
			var oauth_nonce = Convert.ToBase64String(
				new ASCIIEncoding().GetBytes(
					DateTime.Now.Ticks.ToString()));
			var timeSpan = DateTime.UtcNow
						   - new DateTime(1970, 1, 1, 0, 0, 0, 0,
							   DateTimeKind.Utc);
			var oauth_timestamp = Convert.ToInt64(timeSpan.TotalSeconds).ToString();
			var resource_url = "https://api.twitter.com/1.1/statuses/update.json";

			var baseFormat = "oauth_consumer_key={0}&oauth_nonce={1}&oauth_signature_method={2}" +
							 "&oauth_timestamp={3}&oauth_token={4}&oauth_version={5}&status={6}";

			var baseString = string.Format(baseFormat,
				oauth_consumer_key,
				oauth_nonce,
				oauth_signature_method,
				oauth_timestamp,
				oauth_token,
				oauth_version,
				Uri.EscapeDataString(status)
			);

			baseString = string.Concat("POST&", Uri.EscapeDataString(resource_url),
				"&", Uri.EscapeDataString(baseString));

			var compositeKey = string.Concat(Uri.EscapeDataString(oauth_consumer_secret),
				"&", Uri.EscapeDataString(oauth_token_secret));

			string oauth_signature;
			using (HMACSHA1 hasher = new HMACSHA1(Encoding.ASCII.GetBytes(compositeKey)))
			{
				oauth_signature = Convert.ToBase64String(
					hasher.ComputeHash(Encoding.ASCII.GetBytes(baseString)));
			}

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
			request.Method = "POST";
			request.ContentType = "application/x-www-form-urlencoded";

			return request;
		}


		private static void TestWithTweetSharpXAuth()
		{
			// OAuth Access Token Exchange
			TwitterService service = new TwitterService(OAuthProperties.ConsumerKey, OAuthProperties.ConsumerKeySecret);

			Console.WriteLine("Enter Username...");
			string username = Console.ReadLine();
			Console.WriteLine("Enter Password...");
			string password = Console.ReadLine();
			OAuthAccessToken accessToken = service.GetAccessTokenWithXAuth(username, password);

			service.AuthenticateWith(accessToken.Token, accessToken.TokenSecret);
			var verifyCredentialsOptions = new VerifyCredentialsOptions { IncludeEntities = true };
			TwitterUser user = service.VerifyCredentials(verifyCredentialsOptions);
		}

		private async static void TestSignInWithXAuth()
		{
			Console.WriteLine("Enter User Name...");
			string userName = Console.ReadLine();
			Console.WriteLine("Enter Password...");
			string password = Console.ReadLine();

			var auth = new XAuthAuthorizer
			{
				CredentialStore = new XAuthCredentials
				{
					ConsumerKey = OAuthProperties.ConsumerKey,
					ConsumerSecret = OAuthProperties.ConsumerKeySecret,
					UserName = userName,
					Password = password
				}
			};

			try
			{
				await auth.AuthorizeAsync();
			}
			catch (Exception ex)
			{
				Console.WriteLine(ex);
			}

			using (var twitterCtx = new TwitterContext(auth))
			{
				//Log
				twitterCtx.Log = Console.Out;

				List<LinqToTwitter.User> users =
					(from tweet in twitterCtx.User
					 where tweet.Type == UserType.Show
							&& tweet.ScreenName == "JoeMayo"
					 select tweet)
					.ToList();

				users.ForEach(user =>
				{
					var status =
						user.Protected || user.Status == null ?
							"Status Unavailable" :
							user.Status.Text;

					Console.WriteLine(
						"ID: {0}, Name: {1}\nLast Tweet: {2}\n",
						user.UserID, user.ScreenName, status);
				});
			}
		}
	}
}
