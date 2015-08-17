using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Project.TranslateTwitter.Translator.Microsoft
{
	public class LanguageLookup : LanguageParent
	{
		protected override string MethodName => "GetLanguagesForTranslate";

		public LanguageLookup(IAuthenticationContext authenticationContext) 
			: base(authenticationContext)
		{
		}

		protected override string GetRequestUriQueryString()
		{
			return string.Empty;
		}


	}
}
