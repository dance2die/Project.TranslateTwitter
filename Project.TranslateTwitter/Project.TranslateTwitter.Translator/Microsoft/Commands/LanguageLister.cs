using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;
using Project.TranslateTwitter.Translator.Microsoft.Auth;

namespace Project.TranslateTwitter.Translator.Microsoft.Commands
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

			using (WebResponse response = CreateRequest().GetResponse())
			using (Stream responseStream = response.GetResponseStream())
			{
				DataContractSerializer serializer = new DataContractSerializer(typeof(List<string>));
				result = (List<string>)serializer.ReadObject(responseStream);
				return result;
			}
		}

		protected override string GetQueryString()
		{
			return string.Empty;
		}
	}
}
