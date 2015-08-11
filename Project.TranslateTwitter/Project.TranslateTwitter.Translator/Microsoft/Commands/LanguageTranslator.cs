using System;
using System.IO;
using System.Net;
using System.Runtime.Serialization;
using System.Text;
using System.Xml;
using Project.TranslateTwitter.Translator.Microsoft.Auth;
using Project.TranslateTwitter.Translator.Microsoft.Commands;

namespace Project.TranslateTwitter.Translator.Microsoft.Commands
{
	public class LanguageTranslator : LanguageParent
	{
		private LanguageTranslatorArg Arg { get; set; }

		protected override string MethodName => "Translate";

		public LanguageTranslator(IAuthenticationContext authenticationContext) 
			: base(authenticationContext)
		{
		}

		protected override string GetQueryString()
		{
			return $"?text={Arg.TextToTranslate}&from={Arg.FromLanguage}&to={Arg.ToLanguage}";
		}

		public string Translate(LanguageTranslatorArg arg)
		{
			Arg = arg;

			using (WebResponse response = CreateRequest().GetResponse())
			using (Stream responseStream = response.GetResponseStream())
			{
				//var encode = Encoding.GetEncoding("utf-8");
				//var translatedStream = new StreamReader(responseStream, encode);

				//XmlDocument xTranslation = new XmlDocument();
				//xTranslation.LoadXml(translatedStream.ReadToEnd());

				//return xTranslation.InnerText;

				DataContractSerializer serializer = new DataContractSerializer(Type.GetType("System.String"));
				string languageTranslated = (string)serializer.ReadObject(responseStream);
				return languageTranslated;

			}
		}
	}
}
