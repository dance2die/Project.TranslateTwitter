namespace Project.TranslateTwitter.Translator.Microsoft.Commands
{
	public class LanguageTranslatorArg
	{
		public string TextToTranslate { get; set; }
		public string FromLanguage { get; set; }
		public string ToLanguage { get; set; }

		public LanguageTranslatorArg(string textToTranslate, string fromLanguage, string toLanguage = "en")
		{
			TextToTranslate = textToTranslate;
			FromLanguage = fromLanguage;
			ToLanguage = toLanguage;
		}
	}
}