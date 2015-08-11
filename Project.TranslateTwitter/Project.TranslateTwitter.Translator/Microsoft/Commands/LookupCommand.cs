using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Runtime.Serialization;
using Project.TranslateTwitter.Translator.Microsoft.Auth;

namespace Project.TranslateTwitter.Translator.Microsoft.Commands
{
	public class LookupCommand : ParentCommand<List<string>>
	{
		/// <remarks>https://msdn.microsoft.com/en-us/library/ff512421.aspx</remarks>
		protected override string CommandName => "GetLanguagesForTranslate";
		public override List<string> Result { get; set; }

		public LookupCommand(IAuthenticationContext authenticationContext) 
			: base(authenticationContext)
		{
		}

		protected override string GetQueryString()
		{
			return string.Empty;
		}

		public override void Execute()
		{
			using (WebResponse response = CreateRequest().GetResponse())
			using (Stream responseStream = response.GetResponseStream())
			{
				DataContractSerializer serializer = new DataContractSerializer(typeof(List<string>));
				Result = (List<string>)serializer.ReadObject(responseStream);
			}
		}
	}
}
