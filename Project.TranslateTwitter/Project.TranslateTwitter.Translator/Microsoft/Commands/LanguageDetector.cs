using System.IO;
using System.Net;
using System.Runtime.Serialization;
using Project.TranslateTwitter.Translator.Microsoft.Auth;

namespace Project.TranslateTwitter.Translator.Microsoft.Commands
{
	public class LanguageDetector : LanguageParent<string>
	{
		private readonly string _textToDetect;

		protected override string CommandName => "Detect";
		public override string Result { get; set; }

		public LanguageDetector(IAuthenticationContext authenticationContext, string textToDetect) 
			: base(authenticationContext)
		{
			_textToDetect = textToDetect;
		}

		protected override string GetQueryString()
		{
			return $"?text={_textToDetect}";
		}

		public override void Execute()
		{
			using (WebResponse response = CreateRequest().GetResponse())
			using (Stream responseStream = response.GetResponseStream())
			{
				DataContractSerializer serializer = new DataContractSerializer(typeof(string));
				Result= (string)serializer.ReadObject(responseStream);
			}
		}
	}
}