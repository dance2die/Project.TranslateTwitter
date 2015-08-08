using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Runtime.Serialization.Json;
using System.Xml.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;

namespace Project.TranslateTwitter.Translator.Mstf.Demo
{
	public class Program
	{
		public static void Main(string[] args)
		{
			Test1();
		}

		/// <remarks>
		/// http://blogs.msdn.com/b/translation/p/gettingstarted2.aspx
		/// </remarks>
		private static void Test1()
		{
			string clientID = "Project_TranslateTwitter";
			string clientSecret = Environment.GetEnvironmentVariable(
				"Project_TranslateTwitter.ClientSecret", EnvironmentVariableTarget.User);

			string translatorAccessUri = "https://datamarket.accesscontrol.windows.net/v2/OAuth2-13";
			string requestDetails = string.Format(
				"grant_type=client_credentials&client_id={0}&client_secret={1}&scope=http://api.microsofttranslator.com",
				HttpUtility.UrlEncode(clientID), HttpUtility.UrlEncode(clientSecret));

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
			System.Xml.XmlDocument xTranslation = new System.Xml.XmlDocument();
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
}
