using System;
using System.IO;
using System.Net;
using System.Runtime.Serialization;

namespace Project.TranslateTwitter.Translator.Microsoft
{
	public class LanguageDetector : LanguageParent
	{
		private string _textToDetect;

		protected override string MethodName => "Detect";

		public LanguageDetector(IAuthenticationContext authenticationContext) 
			: base(authenticationContext)
		{
		}

		protected override string GetQueryString()
		{
			return $"?text={_textToDetect}";
		}

		public string Detect(string textToDetect)
		{
			_textToDetect = textToDetect;

			using (WebResponse response = CreateRequest().GetResponse())
			using (Stream responseStream = response.GetResponseStream())
			{
				DataContractSerializer serializer = new DataContractSerializer(Type.GetType("System.String"));
				string languageDetected = (string) serializer.ReadObject(responseStream);
				return languageDetected;
			}
		}
	}
}