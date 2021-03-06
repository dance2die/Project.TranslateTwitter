﻿using System;
using System.Collections.Generic;
using System.Compat.Web;
using System.IO;
using System.Linq;
using System.Net;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using LinqToTwitter;
using Nito.AsyncEx;
using TweetSharp;
using Hammock;
using Hammock.Authentication.OAuth;

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
			//Test1();

			// http://www.codeproject.com/Articles/676313/Twitter-API-v-with-OAuth
			//Verify_Credentials();

			Console.Write("Enter UserName:");
			string userName = Console.ReadLine();
			Console.Write("Enter Password:");
			string password = Console.ReadLine();
			TestXAuth(userName, password);
			//var sig = GetXAuthSignature(userName, password);
			//Console.WriteLine("xauth_signature = {0}", sig);

			//TestXAuthWithHammock(userName, password);
		}

		private static void TestXAuth(string userName, string password)
		{
			var request = GetXAuthWebRequest(userName, password);
			//using (Stream stream = request.GetRequestStream())
			//{
			//}
			WebResponse response = request.GetResponse();
			using (var reader = new StreamReader(response.GetResponseStream()))
			{
				string objText = reader.ReadToEnd();
				//JavaScriptSerializer js = new JavaScriptSerializer();
				//MyObject myojb = (MyObject)js.Deserialize(objText, typeof(MyObject));
			}
		}

		private static HttpWebRequest GetXAuthWebRequest(string userName, string password)
		{
			var oauth_token = "";
			var oauth_token_secret = "";
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
			const string resource_url = "https://api.twitter.com/oauth/access_token";

			var x_auth_mode = "client_auth";
			var x_auth_username = userName;
			var x_auth_password = password;



			//oauth_consumer_key = "JvyS7DO2qd6NNTsXJ4E7zA";
			//oauth_consumer_secret = "9z6157pUbOBqtbm0A0q4r29Y2EYzIHlUwbF4Cl9c";
			//oauth_nonce = "6AN2dKRzxyGhmIXUKSmp1JcB4pckM8rD3frKMTmVAo";
			//oauth_signature_method = "HMAC-SHA1";
			//oauth_timestamp = "1284565601";
			//oauth_version = "1.0";
			//x_auth_mode = "client_auth";
			//x_auth_password = "twitter-xauth";
			//x_auth_username = "oauth_test_exec";


			string postBody = string.Format(
				"x_auth_mode={0}&x_auth_password={1}&x_auth_username={2}",
				x_auth_mode, x_auth_password, x_auth_username);
			//postBody = Uri.EscapeDataString(postBody);

			var baseFormat = "oauth_consumer_key={0}&oauth_nonce={1}&oauth_signature_method={2}" +
							 "&oauth_timestamp={3}&oauth_version={4}" +
							 "&" + postBody;


			var baseString = string.Format(baseFormat,
				oauth_consumer_key,
				oauth_nonce,
				oauth_signature_method,
				oauth_timestamp,
				oauth_version
				);

			baseString = string.Concat("POST&", Uri.EscapeDataString(resource_url),
				"&", Uri.EscapeDataString(baseString));

			oauth_token_secret = "";
			var compositeKey = string.Concat(Uri.EscapeDataString(oauth_consumer_secret),
				"&", Uri.EscapeDataString(oauth_token_secret));

			string oauth_signature;
			using (HMACSHA1 hasher = new HMACSHA1(Encoding.ASCII.GetBytes(compositeKey)))
			{
				oauth_signature = Convert.ToBase64String(hasher.ComputeHash(Encoding.ASCII.GetBytes(baseString)));
			}

			var headerFormat =
				"OAuth oauth_nonce=\"{0}\", oauth_signature_method=\"{1}\", " +
				"oauth_timestamp=\"{2}\", oauth_consumer_key=\"{3}\", " +
				"oauth_signature=\"{4}\", " +
				"oauth_version=\"{5}\"";

			var authHeader = string.Format(headerFormat,
				Uri.EscapeDataString(oauth_nonce),
				Uri.EscapeDataString(oauth_signature_method),
				Uri.EscapeDataString(oauth_timestamp),
				Uri.EscapeDataString(oauth_consumer_key),
				Uri.EscapeDataString(oauth_signature),
				Uri.EscapeDataString(oauth_version)
				);

			ServicePointManager.Expect100Continue = false;

			//postBody = Uri.EscapeDataString(postBody);
			//baseString = Uri.EscapeDataString(baseString);
            //byte[] encodedBody = Encoding.ASCII.GetBytes(postBody);
			//byte[] encodedBody = Encoding.ASCII.GetBytes(baseString);
			byte[] encodedBody = Encoding.UTF8.GetBytes(postBody);
			//byte[] encodedBody = Encoding.UTF8.GetBytes(baseString);
			var request = (HttpWebRequest)WebRequest.Create(resource_url);
			request.Headers.Add("Authorization", authHeader);
			request.Method = "POST";
			request.ContentType = "application/x-www-form-urlencoded";
			request.ContentLength = encodedBody.Length;
			using (Stream requeStream = request.GetRequestStream())
			{
				requeStream.Write(encodedBody, 0, encodedBody.Length);
			}

			return request;
		}

		private static void TestXAuthWithHammock(string userName, string password)
		{
			RestClient client = new RestClient
			{
				Authority = "https://api.twitter.com",

				//HasElevatedPermissions = true,
				Credentials = new OAuthCredentials()
				{
					ConsumerKey = OAuthProperties.ConsumerKey,
					ConsumerSecret = OAuthProperties.ConsumerKeySecret,
					SignatureMethod = OAuthSignatureMethod.HmacSha1,
					ParameterHandling = OAuthParameterHandling.HttpAuthorizationHeader,
					Version = "1.0"
				}
			};

			RestRequest request = new RestRequest()
			{
				Path = "oauth/access_token",
			};

			request.AddParameter("x_auth_mode", "client_auth");
			request.AddParameter("x_auth_username", userName);
			request.AddParameter("x_auth_password", password);

			//client.BeginRequest(request, Callback);

			var restResponse = client.Request(request);
			Console.WriteLine("Content = {0}", restResponse.Content);
		}

		private static void Callback(RestRequest request, RestResponse response, object userState)
		{
			// the response from the server
			Console.WriteLine(response.Content);
		}

		/// <summary>
		/// http://www.codeproject.com/Articles/676313/Twitter-API-v-with-OAuth
		/// </summary>
		public static void Verify_Credentials()
		{
			string oauthconsumerkey = OAuthProperties.ConsumerKey;
			string oauthconsumersecret = OAuthProperties.ConsumerKeySecret;
			string oauthsignaturemethod = "HMAC-SHA1";
			string oauthversion = "1.0";
			string oauthtoken = OAuthProperties.AccessToken;
			string oauthtokensecret = OAuthProperties.AccessTokenSecret;
			string oauthnonce = Convert.ToBase64String(new ASCIIEncoding().GetBytes(DateTime.Now.Ticks.ToString()));
			TimeSpan ts = DateTime.UtcNow - new DateTime(1970, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc);
			string oauthtimestamp = Convert.ToInt64(ts.TotalSeconds).ToString();
			SortedDictionary<string, string> basestringParameters = new SortedDictionary<string, string>();
			basestringParameters.Add("oauth_version", "1.0");
			basestringParameters.Add("oauth_consumer_key", oauthconsumerkey);
			basestringParameters.Add("oauth_nonce", oauthnonce);
			basestringParameters.Add("oauth_signature_method", "HMAC-SHA1");
			basestringParameters.Add("oauth_timestamp", oauthtimestamp);
			basestringParameters.Add("oauth_token", oauthtoken);
			//GS - Build the signature string
			StringBuilder baseString = new StringBuilder();
			baseString.Append("GET" + "&");
			baseString.Append(EncodeCharacters(Uri.EscapeDataString("https://api.twitter.com/1.1/account/verify_credentials.json") + "&"));
			foreach (KeyValuePair<string, string> entry in basestringParameters)
			{
				baseString.Append(EncodeCharacters(Uri.EscapeDataString(entry.Key + "=" + entry.Value + "&")));
			}

			//Since the baseString is urlEncoded we have to remove the last 3 chars - %26
			string finalBaseString = baseString.ToString().Substring(0, baseString.Length - 3);

			//Build the signing key
			string signingKey = EncodeCharacters(Uri.EscapeDataString(oauthconsumersecret)) + "&" +
			EncodeCharacters(Uri.EscapeDataString(oauthtokensecret));

			//Sign the request
			HMACSHA1 hasher = new HMACSHA1(new ASCIIEncoding().GetBytes(signingKey));
			string oauthsignature = Convert.ToBase64String(hasher.ComputeHash(new ASCIIEncoding().GetBytes(finalBaseString)));

			//Tell Twitter we don't do the 100 continue thing
			ServicePointManager.Expect100Continue = false;

			//authorization header
			HttpWebRequest hwr = (HttpWebRequest)WebRequest.Create(
			  @"https://api.twitter.com/1.1/account/verify_credentials.json");
			StringBuilder authorizationHeaderParams = new StringBuilder();
			authorizationHeaderParams.Append("OAuth ");
			authorizationHeaderParams.Append("oauth_nonce=" + "\"" + Uri.EscapeDataString(oauthnonce) + "\",");
			authorizationHeaderParams.Append("oauth_signature_method=" + "\"" + Uri.EscapeDataString(oauthsignaturemethod) + "\",");
			authorizationHeaderParams.Append("oauth_timestamp=" + "\"" + Uri.EscapeDataString(oauthtimestamp) + "\",");
			authorizationHeaderParams.Append("oauth_consumer_key=" + "\"" + Uri.EscapeDataString(oauthconsumerkey) + "\",");
			if (!string.IsNullOrEmpty(oauthtoken))
				authorizationHeaderParams.Append("oauth_token=" + "\"" + Uri.EscapeDataString(oauthtoken) + "\",");
			authorizationHeaderParams.Append("oauth_signature=" + "\"" + Uri.EscapeDataString(oauthsignature) + "\",");
			authorizationHeaderParams.Append("oauth_version=" + "\"" + Uri.EscapeDataString(oauthversion) + "\"");
			hwr.Headers.Add("Authorization", authorizationHeaderParams.ToString());
			hwr.Method = "GET";
			hwr.ContentType = "application/x-www-form-urlencoded";

			//Allow us a reasonable timeout in case Twitter's busy
			hwr.Timeout = 3 * 60 * 1000;
			try
			{
				//hwr.Proxy = new WebProxy("enter proxy details/address");
				HttpWebResponse rsp = hwr.GetResponse() as HttpWebResponse;
				Stream dataStream = rsp.GetResponseStream();
				//Open the stream using a StreamReader for easy access.
				StreamReader reader = new StreamReader(dataStream);
				//Read the content.
				string responseFromServer = reader.ReadToEnd();
			}
			catch (Exception ex)
			{

			}
		}

		private static string EncodeCharacters(string data)
		{
			//as per OAuth Core 1.0 Characters in the unreserved character set MUST NOT be encoded
			//unreserved = ALPHA, DIGIT, '-', '.', '_', '~'
			if (data.Contains("!"))
				data = data.Replace("!", "%21");
			if (data.Contains("'"))
				data = data.Replace("'", "%27");
			if (data.Contains("("))
				data = data.Replace("(", "%28");
			if (data.Contains(")"))
				data = data.Replace(")", "%29");
			if (data.Contains("*"))
				data = data.Replace("*", "%2A");
			if (data.Contains(","))
				data = data.Replace(",", "%2C");

			return data;
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

		private static string GetXAuthSignature(string userName, string password)
		{
			var oauth_token = OAuthProperties.AccessToken;
			var oauth_token_secret = OAuthProperties.AccessTokenSecret;
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
			var resource_url = "https://api.twitter.com/oauth/access_token";


			var baseFormat = "oauth_consumer_key={0}&oauth_nonce={1}&oauth_signature_method={2}" +
							 "&oauth_timestamp={3}&oauth_token={4}&oauth_version={5}" +
							 "&x_auth_mode=client_auth&x_auth_username={6}&x_auth_password={7}";

			string x_auth_username = userName;
			string x_auth_password = password;

			var baseString = string.Format(baseFormat,
				oauth_consumer_key,
				oauth_nonce,
				oauth_signature_method,
				oauth_timestamp,
				oauth_token,
				oauth_version,
				x_auth_username,
				x_auth_password
				);

			baseString = string.Concat("POST&", Uri.EscapeDataString(resource_url),
				"&", Uri.EscapeDataString(baseString));

			oauth_token_secret = "";
			var compositeKey = string.Concat(Uri.EscapeDataString(oauth_consumer_secret),
				"&", Uri.EscapeDataString(oauth_token_secret));
			//var compositeKey = string.Concat(Uri.EscapeDataString(oauth_consumer_secret));
			//compositeKey = "9z6157pUbOBqtbm0A0q4r29Y2EYzIHlUwbF4Cl9c&";

			string oauth_signature;
			using (HMACSHA1 hasher = new HMACSHA1(Encoding.ASCII.GetBytes(compositeKey)))
			//using (HMACSHA1 hasher = new HMACSHA1())
			{
				// https://dev.twitter.com/oauth/xauth
				var test =
					Encoding.ASCII.GetBytes(
						"POST&https%3A%2F%2Fapi.twitter.com%2Foauth%2Faccess_token&oauth_consumer_key%3DJvyS7DO2qd6NNTsXJ4E7zA%26oauth_nonce%3D6AN2dKRzxyGhmIXUKSmp1JcB4pckM8rD3frKMTmVAo%26oauth_signature_method%3DHMAC-SHA1%26oauth_timestamp%3D1284565601%26oauth_version%3D1.0%26x_auth_mode%3Dclient_auth%26x_auth_password%3Dtwitter-xauth%26x_auth_username%3Doauth_test_exec");
				oauth_signature = Convert.ToBase64String(
					hasher.ComputeHash(test));
				//hasher.ComputeHash(Encoding.ASCII.GetBytes(baseString)));
			}

			return oauth_signature;
		}


		private static HttpWebRequest GetTwitterWebRequest(string status)
		{
			var oauth_token = OAuthProperties.AccessToken;
			var oauth_token_secret = OAuthProperties.AccessTokenSecret;
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
			TwitterService twitterService = new TwitterService(OAuthProperties.ConsumerKey, OAuthProperties.ConsumerKeySecret);
			twitterService.AuthenticateWith(OAuthProperties.AccessToken, OAuthProperties.AccessTokenSecret);

			Console.WriteLine("Enter Username...");
			string username = Console.ReadLine();
			Console.WriteLine("Enter Password...");
			string password = Console.ReadLine();
			OAuthAccessToken accessToken = twitterService.GetAccessTokenWithXAuth(username, password);

			twitterService.AuthenticateWith(accessToken.Token, accessToken.TokenSecret);
			var verifyCredentialsOptions = new VerifyCredentialsOptions { IncludeEntities = true };
			TwitterUser user = twitterService.VerifyCredentials(verifyCredentialsOptions);
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
