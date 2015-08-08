using System;
using System.IO;
using System.Net;
using System.Runtime.Serialization;

namespace Project.TranslateTwitter.Translator.Mstf.Demo
{
	public class LanguageDetector
	{
		public IAuthenticationContext AuthenticationContext { get; set; }

		public LanguageDetector(IAuthenticationContext authenticationContext)
		{
			AuthenticationContext = authenticationContext;
		}

		public string DetectMethod(string textToDetect)
		{
			using (WebResponse response = CreateRequest(textToDetect).GetResponse())
			using (Stream responseStream = response.GetResponseStream())
			{
				DataContractSerializer serializer = new DataContractSerializer(Type.GetType("System.String"));
				string languageDetected = (string) serializer.ReadObject(responseStream);
				return languageDetected;
			}
		}

		private HttpWebRequest CreateRequest(string textToDetect)
		{
			string uri = $"http://api.microsofttranslator.com/v2/Http.svc/Detect?text={textToDetect}";
			var result = (HttpWebRequest)WebRequest.Create(uri);
			result.Headers.Add("Authorization", GetAuthorizationToken());

			return result;
		}

		private string GetAuthorizationToken()
		{
			return $"Bearer {GetAccessToken().access_token}";
		}

		private AdmAccessToken GetAccessToken()
		{
			//Get Client Id and Client Secret from https://datamarket.azure.com/developer/applications/
			//Refer obtaining AccessToken (http://msdn.microsoft.com/en-us/library/hh454950.aspx) 
			AdmAuthentication admAuth = new AdmAuthentication(
				AuthenticationContext.ClientId, AuthenticationContext.ClientSecret);
			return admAuth.GetAccessToken();
		}
	}
}