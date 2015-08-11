using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Project.TranslateTwitter.Translator.Microsoft
{
	public class LanguageLister : LanguageParent
	{
		protected override string MethodName => "GetLanguagesForTranslate";

		public LanguageLister(IAuthenticationContext authenticationContext) 
			: base(authenticationContext)
		{
		}

		public List<string> GetSupportedLanguages()
		{
			List<string> result = new List<string>();
			return result;
		}

		protected override string GetQueryString()
		{
			return string.Empty;
		}
	}
}
