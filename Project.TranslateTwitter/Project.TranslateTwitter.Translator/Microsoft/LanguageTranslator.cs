using System.IO;
using System.Net;
using System.Text;
using System.Xml;

namespace Project.TranslateTwitter.Translator.Microsoft
{
	public class LanguageTranslator : LanguageParent
	{
		private LanguageTranslatorArg Arg { get; set; }

		protected override string MethodName => "Translate";

		public LanguageTranslator(IAuthenticationContext authenticationContext) 
			: base(authenticationContext)
		{
		}

		public string Translate(LanguageTranslatorArg arg)
		{
			Arg = arg;

			using (WebResponse response = CreateRequest().GetResponse())
			using (Stream stream = response.GetResponseStream())
			{
				var encode = Encoding.GetEncoding("utf-8");
				var translatedStream = new StreamReader(stream, encode);

				XmlDocument xTranslation = new XmlDocument();
				xTranslation.LoadXml(translatedStream.ReadToEnd());

				return xTranslation.InnerText;
			}
		}

		protected override string GetQueryString()
		{
			return $"?text={Arg.TextToTranslate}&from={Arg.FromLanguage}&to={Arg.ToLanguage}";
		}
	}
}
