using System;
using System.IO;
using System.Net;
using System.Runtime.Serialization.Json;
using System.Text;
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

			TestTranslating(clientId, clientSecret);
			//TestLanguageDetection(clientId, clientSecret);
		}

		/// <remarks>
		/// https://msdn.microsoft.com/en-us/library/Ff512411.aspx
		/// </remarks>
		private static void TestLanguageDetection(string clientId, string clientSecret)
		{
			try
			{
				Console.WriteLine("Enter Text to detect language:");
				string textToDetect = Console.ReadLine();
				//textToDetect = "会場限定";

				var detector = new LanguageDetector(new AuthenticationContext(clientId, clientSecret));
				var detectedLanguage = detector.DetectMethod(textToDetect);

				Console.WriteLine("Language Detected: {0}", detectedLanguage);
				Console.WriteLine("Press any key to continue...");
				Console.ReadKey(true);
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

		private static void ProcessWebException(WebException e)
		{
			Console.WriteLine("{0}", e.ToString());
			// Obtain detailed error information
			string strResponse = String.Empty;
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

			var authenticationContext = new AuthenticationContext(clientId, clientSecret);

			string txtToTranslate = "안녕 세상아";
			string uri = string.Format(
				"http://api.microsofttranslator.com/v2/Http.svc/Translate?text={0}&from=ko&to=en", 
				HttpUtility.UrlEncode(txtToTranslate));
			var translationWebRequest = WebRequest.Create(uri);
			translationWebRequest.Headers.Add("Authorization", GetAuthorizationToken(authenticationContext));

			Stream stream;
			using (var response = translationWebRequest.GetResponse())
			{
				stream = response.GetResponseStream();
			}

			var encode = Encoding.GetEncoding("utf-8");
			var translatedStream = new StreamReader(stream, encode);
			XmlDocument xTranslation = new XmlDocument();
			xTranslation.LoadXml(translatedStream.ReadToEnd());

			Console.WriteLine("Your Translation is: " + xTranslation.InnerText);
		}

		private static string GetAuthorizationToken(AuthenticationContext authenticationContext)
		{
			return $"Bearer {GetAccessToken(authenticationContext).access_token}";
		}

		private static MstfAzureMarketplaceAccessToken GetAccessToken(AuthenticationContext authenticationContext)
		{
			//Get Client Id and Client Secret from https://datamarket.azure.com/developer/applications/
			//Refer obtaining AccessToken (http://msdn.microsoft.com/en-us/library/hh454950.aspx) 
			MstfAzureMarketplaceAuthentication mstfAzureMarketplaceAuth = new MstfAzureMarketplaceAuthentication(
				authenticationContext.ClientId, authenticationContext.ClientSecret);
			return mstfAzureMarketplaceAuth.GetAccessToken();
		}

		private static HttpWebRequest GetWebRequest(AuthenticationContext authenticationContext)
		{
			string translatorAccessUri = "https://datamarket.accesscontrol.windows.net/v2/OAuth2-13";
			string requestDetails = string.Format(
				"grant_type=client_credentials&client_id={0}&client_secret={1}&scope=http://api.microsofttranslator.com",
				HttpUtility.UrlEncode(authenticationContext.ClientId), 
				HttpUtility.UrlEncode(authenticationContext.ClientSecret));

			HttpWebRequest result = (HttpWebRequest)WebRequest.Create(translatorAccessUri);
			result.ContentType = "application/x-www-form-urlencoded";
			result.Method = "POST";

			byte[] bytes = Encoding.ASCII.GetBytes(requestDetails);
			result.ContentLength = bytes.Length;
			using (Stream outputStream = result.GetRequestStream())
			{
				outputStream.Write(bytes, 0, bytes.Length);
			}

			return result;
		}
	}
}
