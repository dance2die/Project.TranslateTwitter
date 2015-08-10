using System.IO;
using System.Net;
using System.Text;
using System.Web;
using System.Xml;

namespace Project.TranslateTwitter.Translator.Microsoft
{
	public class LanguageTranslator
	{
		public IAuthenticationContext AuthenticationContext { get; set; }

		public LanguageTranslator(IAuthenticationContext authenticationContext)
		{
			AuthenticationContext = authenticationContext;
		}

		public string Translate(TranslationArg arg)
		{
			using (WebResponse response = CreateRequest(arg).GetResponse())
			using (Stream stream = response.GetResponseStream())
			{
				var encode = Encoding.GetEncoding("utf-8");
				var translatedStream = new StreamReader(stream, encode);

				XmlDocument xTranslation = new XmlDocument();
				xTranslation.LoadXml(translatedStream.ReadToEnd());

				return xTranslation.InnerText;
			}
		}

		private HttpWebRequest CreateRequest(TranslationArg arg)
		{
			string uri = string.Format(
				"http://api.microsofttranslator.com/v2/Http.svc/Translate?text={0}&from={1}&to={2}",
				HttpUtility.UrlEncode(arg.TextToTranslate),
				arg.FromLanguage, arg.ToLanguage);
			var result = (HttpWebRequest)WebRequest.Create(uri);
			result.Headers.Add("Authorization", GetAuthorizationToken());
			return result;
		}

		private string GetAuthorizationToken()
		{
			return $"Bearer {GetAccessToken().access_token}";
		}

		private MstfAzureMarketplaceAccessToken GetAccessToken()
		{
			//Get Client Id and Client Secret from https://datamarket.azure.com/developer/applications/
			//Refer obtaining AccessToken (http://msdn.microsoft.com/en-us/library/hh454950.aspx) 
			MstfAzureMarketplaceAuthentication mstfAzureMarketplaceAuth = new MstfAzureMarketplaceAuthentication(
				AuthenticationContext.ClientId, AuthenticationContext.ClientSecret);
			return mstfAzureMarketplaceAuth.GetAccessToken();
		}
	}

	public class TranslationArg
	{
		public string TextToTranslate { get; set; }
		public string FromLanguage { get; set; }
		public string ToLanguage { get; set; }

		public TranslationArg(string textToTranslate, string fromLanguage, string toLanguage = "en")
		{
			TextToTranslate = textToTranslate;
			FromLanguage = fromLanguage;
			ToLanguage = toLanguage;
		}
	}
}
