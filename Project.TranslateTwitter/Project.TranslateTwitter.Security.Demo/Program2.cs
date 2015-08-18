using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Project.TranslateTwitter.Security.Demo
{
	public class Program2
	{
		public static void Main(string[] args)
		{
			// Need to learn how to create a signature first.
			// https://dev.twitter.com/oauth/overview/creating-signatures
			TestCreateSignature();
		}

		private static void TestCreateSignature()
		{
			// 1.) Collecting the request method and URL
			string httpMethod = "POST";
			string baseUrl = "https://api.twitter.com/1.1/statuses/user_timeline.json";
			httpMethod = "POST";
			baseUrl = "https://api.twitter.com/1/statuses/update.json";

			// 2.) Collecting parameters
			Dictionary<string, string> requestParams = GetRequestParams();
			requestParams = GetTestRequestParams();

			var signature = CreateSignature(httpMethod, baseUrl, requestParams);
			Console.WriteLine("Signature: {0}", signature);
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

		private static string CreateSignature(
			string httpMethod, string baseUrl, IDictionary<string, string> requestParams)
		{
			var result = string.Empty;

			// 3.) Creating the signature base string
			string signatureBaseString = CreateSignatureBaseString(httpMethod, baseUrl, requestParams);

			return result;
		}

		private static string CreateSignatureBaseString(
			string httpMethod, string baseUrl, IDictionary<string, string> requestParams)
		{
			const string separator = "&";

			var result = new StringBuilder();

			// 1.) Convert the HTTP Method to uppercase and set the output string equal to this value.
			result.Append(httpMethod.ToUpperInvariant());

			// 2.) Append the ‘&’ character to the output string.
			result.Append(separator);

			// 3.) Percent encode the URL and append it to the output string.
			result.Append(Uri.EscapeDataString(baseUrl));

			// 4.) Append the ‘&’ character to the output string.
			result.Append(separator);

			// 5.) Percent encode the parameter string and append it to the output string.
			var baseString = GetParameterString(requestParams, separator);
			result.Append(Uri.EscapeDataString(baseString));

			return result.ToString();
		}

		private static string GetParameterString(IDictionary<string, string> requestParams, string separator)
		{
			var query = (from requestParam in requestParams
						// According to Twitter spec,
						// Sort the list of parameters alphabetically[1] by encoded key[2].
						 orderby requestParam.Key
						select new {requestParam.Key, requestParam.Value}).ToList();

			List<string> paramList = new List<string>(query.Count);
			foreach (var requestParam in query)
			{
				paramList.Add($"{requestParam.Key}={Uri.EscapeDataString(requestParam.Value)}");
			}

			return string.Join(separator, paramList.ToArray());
		}
	}
}
