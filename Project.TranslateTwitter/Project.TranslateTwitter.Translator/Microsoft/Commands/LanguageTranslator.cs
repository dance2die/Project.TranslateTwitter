using System.IO;
using System.Net;
using System.Runtime.Serialization;
using Project.TranslateTwitter.Translator.Microsoft.Auth;

namespace Project.TranslateTwitter.Translator.Microsoft.Commands
{
	public class LanguageTranslator : LanguageParent<string>
	{
		private LanguageTranslatorArg Arg { get; set; }

		protected override string CommandName => "Translate";
		public override string Result { get; set;  }

		public LanguageTranslator(IAuthenticationContext authenticationContext,
			LanguageTranslatorArg arg) 
			: base(authenticationContext)
		{
			Arg = arg;
		}

		protected override string GetQueryString()
		{
			return $"?text={Arg.TextToTranslate}&from={Arg.FromLanguage}&to={Arg.ToLanguage}";
		}

		public override void Execute()
		{
			using (WebResponse response = CreateRequest().GetResponse())
			using (Stream responseStream = response.GetResponseStream())
			{
				//var encode = Encoding.GetEncoding("utf-8");
				//var translatedStream = new StreamReader(responseStream, encode);

				//XmlDocument xTranslation = new XmlDocument();
				//xTranslation.LoadXml(translatedStream.ReadToEnd());

				//return xTranslation.InnerText;

				DataContractSerializer serializer = new DataContractSerializer(typeof(string));
				string languageTranslated = (string)serializer.ReadObject(responseStream);
				Result = languageTranslated;
			}
		}
	}
}
