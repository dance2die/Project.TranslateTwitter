using System;
using System.IO;
using System.Net;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Json;
using System.Text;
using System.Threading;
using System.Web;
using System.Xml;

namespace Project.TranslateTwitter.Translator.Mstf.Demo
{
	public class Program
	{
		public static void Main(string[] args)
		{
			string clientId = "Project_TranslateTwitter";
			string clientSecret = Environment.GetEnvironmentVariable(
				"Project_TranslateTwitter.ClientSecret", EnvironmentVariableTarget.User);

			//TestTranslating(clientId, clientSecret);
			TestLanguageDetection(clientId, clientSecret);
		}

		/// <remarks>
		/// https://msdn.microsoft.com/en-us/library/Ff512411.aspx
		/// </remarks>
		private static void TestLanguageDetection(string clientId, string clientSecret)
		{
			//Get Client Id and Client Secret from https://datamarket.azure.com/developer/applications/
			//Refer obtaining AccessToken (http://msdn.microsoft.com/en-us/library/hh454950.aspx) 
			AdmAuthentication admAuth = new AdmAuthentication(clientId, clientSecret);

			try
			{
				var admToken = admAuth.GetAccessToken();
				// Create a header with the access_token property of the returned token
				var headerValue = "Bearer " + admToken.access_token;
				DetectMethod(headerValue);
			}
			catch (WebException e)
			{
				ProcessWebException(e);
				Console.WriteLine("Press any key to continue...");
				Console.ReadKey(true);
			}
			catch (Exception ex)
			{
				Console.WriteLine(ex.Message);
				Console.WriteLine("Press any key to continue...");
				Console.ReadKey(true);
			}
		}

		private static void DetectMethod(string authToken)
		{
			Console.WriteLine("Enter Text to detect language:");
			string textToDetect = Console.ReadLine();
			//textToDetect = "会場限定";
			//Keep appId parameter blank as we are sending access token in authorization header.
			string uri = "http://api.microsofttranslator.com/v2/Http.svc/Detect?text=" + textToDetect;
			HttpWebRequest httpWebRequest = (HttpWebRequest)WebRequest.Create(uri);
			httpWebRequest.Headers.Add("Authorization", authToken);
			WebResponse response = null;
			try
			{
				response = httpWebRequest.GetResponse();
				using (Stream stream = response.GetResponseStream())
				{
					DataContractSerializer dcs = new DataContractSerializer(Type.GetType("System.String"));
					string languageDetected = (string)dcs.ReadObject(stream);
					Console.WriteLine("Language detected:{0}", languageDetected);
					Console.WriteLine("Press any key to continue...");
					Console.ReadKey(true);
				}
			}
			finally
			{
				if (response != null)
				{
					response.Close();
					response = null;
				}
			}
		}
		private static void ProcessWebException(WebException e)
		{
			Console.WriteLine("{0}", e.ToString());
			// Obtain detailed error information
			string strResponse = string.Empty;
			using (HttpWebResponse response = (HttpWebResponse)e.Response)
			{
				using (Stream responseStream = response.GetResponseStream())
				{
					using (StreamReader sr = new StreamReader(responseStream, Encoding.ASCII))
					{
						strResponse = sr.ReadToEnd();
					}
				}
			}
			Console.WriteLine("Http status code={0}, error message={1}", e.Status, strResponse);
		}

		/// <remarks>
		/// http://blogs.msdn.com/b/translation/p/gettingstarted2.aspx
		/// </remarks>
		private static void TestTranslating(string clientId, string clientSecret)
		{
			string translatorAccessUri = "https://datamarket.accesscontrol.windows.net/v2/OAuth2-13";
			string requestDetails = string.Format(
				"grant_type=client_credentials&client_id={0}&client_secret={1}&scope=http://api.microsofttranslator.com",
				HttpUtility.UrlEncode(clientId), HttpUtility.UrlEncode(clientSecret));

			WebRequest webRequest = WebRequest.Create(translatorAccessUri);
			webRequest.ContentType = "application/x-www-form-urlencoded";
			webRequest.Method = "POST";

			byte[] bytes = Encoding.ASCII.GetBytes(requestDetails);
			webRequest.ContentLength = bytes.Length;
			using (Stream outputStream = webRequest.GetRequestStream())
			{
				outputStream.Write(bytes, 0, bytes.Length);
			}

			AdmAccessToken token;
			using (var webResponse = webRequest.GetResponse())
			{
				DataContractJsonSerializer serializer = new DataContractJsonSerializer(typeof(AdmAccessToken));
				//Get deserialized object from JSON stream 
				token = (AdmAccessToken)serializer.ReadObject(webResponse.GetResponseStream());
			}

			string headerValue = string.Format("Bearer {0}", token.access_token);


			string txtToTranslate = "안녕 세상아";
			string uri = string.Format(
				"http://api.microsofttranslator.com/v2/Http.svc/Translate?text={0}&from=ko&to=en", 
				HttpUtility.UrlEncode(txtToTranslate));
			var translationWebRequest = WebRequest.Create(uri);
			translationWebRequest.Headers.Add("Authorization", headerValue);
			var response = translationWebRequest.GetResponse();
			var stream = response.GetResponseStream();
			var encode = Encoding.GetEncoding("utf-8");
			var translatedStream = new StreamReader(stream, encode);
			XmlDocument xTranslation = new XmlDocument();
			xTranslation.LoadXml(translatedStream.ReadToEnd());

			Console.WriteLine("Your Translation is: " + xTranslation.InnerText);
		}
	}

	public class AdmAccessToken
	{
		public string access_token { get; set; }
		public string token_type { get; set; }
		public string expires_in { get; set; }
		public string scope { get; set; }
	}

	public class AdmAuthentication
	{
		public static readonly string DatamarketAccessUri = "https://datamarket.accesscontrol.windows.net/v2/OAuth2-13";
		private readonly string _clientId;
		private string _clientSecret;
		private readonly string _request;
		private AdmAccessToken _token;
		private readonly Timer _accessTokenRenewer;
		//Access token expires every 10 minutes. Renew it every 9 minutes only.
		private const int RefreshTokenDuration = 9;

		public AdmAuthentication(string clientId, string clientSecret)
		{
			_clientId = clientId;
			_clientSecret = clientSecret;
			//If clientid or client secret has special characters, encode before sending request
			_request = string.Format(
				"grant_type=client_credentials&client_id={0}&client_secret={1}&scope=http://api.microsofttranslator.com", 
				HttpUtility.UrlEncode(clientId), HttpUtility.UrlEncode(clientSecret));
			_token = HttpPost(DatamarketAccessUri, this._request);
			//renew the token every specified minutes
			_accessTokenRenewer = new Timer(
				OnTokenExpiredCallback, this, TimeSpan.FromMinutes(RefreshTokenDuration), TimeSpan.FromMilliseconds(-1));
		}

		public AdmAccessToken GetAccessToken()
		{
			return _token;
		}

		private void RenewAccessToken()
		{
			AdmAccessToken newAccessToken = HttpPost(DatamarketAccessUri, this._request);
			//swap the new token with old one
			//Note: the swap is thread unsafe
			this._token = newAccessToken;
			Console.WriteLine(string.Format("Renewed token for user: {0} is: {1}", this._clientId, this._token.access_token));
		}

		private void OnTokenExpiredCallback(object stateInfo)
		{
			try
			{
				RenewAccessToken();
			}
			catch (Exception ex)
			{
				Console.WriteLine(string.Format("Failed renewing access token. Details: {0}", ex.Message));
			}
			finally
			{
				try
				{
					_accessTokenRenewer.Change(TimeSpan.FromMinutes(RefreshTokenDuration), TimeSpan.FromMilliseconds(-1));
				}
				catch (Exception ex)
				{
					Console.WriteLine(string.Format("Failed to reschedule the timer to renew access token. Details: {0}", ex.Message));
				}
			}
		}

		private AdmAccessToken HttpPost(string DatamarketAccessUri, string requestDetails)
		{
			//Prepare OAuth request 
			WebRequest webRequest = WebRequest.Create(DatamarketAccessUri);
			webRequest.ContentType = "application/x-www-form-urlencoded";
			webRequest.Method = "POST";
			byte[] bytes = Encoding.ASCII.GetBytes(requestDetails);
			webRequest.ContentLength = bytes.Length;

			using (Stream outputStream = webRequest.GetRequestStream())
			{
				outputStream.Write(bytes, 0, bytes.Length);
			}

			using (WebResponse webResponse = webRequest.GetResponse())
			{
				DataContractJsonSerializer serializer = new DataContractJsonSerializer(typeof(AdmAccessToken));
				//Get deserialized object from JSON stream
				AdmAccessToken token = (AdmAccessToken)serializer.ReadObject(webResponse.GetResponseStream());
				return token;
			}
		}
	}
}
