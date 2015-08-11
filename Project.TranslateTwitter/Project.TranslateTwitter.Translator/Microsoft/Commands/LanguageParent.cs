using System.Net;
using System.Windows.Input;
using Project.TranslateTwitter.Translator.Microsoft.Auth;

namespace Project.TranslateTwitter.Translator.Microsoft.Commands
{
	public abstract class LanguageParent<T> : ILanguageCommand, ILanguageCommandWithResult<T>
	{
		private const string REQUEST_URI = "http://api.microsofttranslator.com/v2/Http.svc";

		public IAuthenticationContext AuthenticationContext { get; set; }

		public LanguageParent(IAuthenticationContext authenticationContext)
		{
			AuthenticationContext = authenticationContext;
		}

		protected HttpWebRequest CreateRequest()
		{
			string uri = $"{REQUEST_URI}/{CommandName}{GetQueryString()}";
			var result = (HttpWebRequest)WebRequest.Create(uri);
			result.Headers.Add("Authorization", GetAuthorizationToken());

			return result;
		}

		protected abstract string CommandName { get; }
		public abstract T Result { get; set; }

		protected abstract string GetQueryString();

		public abstract void Execute();

		private string GetAuthorizationToken()
		{
			return $"Bearer {GetAccessToken().access_token}";
		}

		private MstfAzureMarketplaceAccessToken GetAccessToken()
		{
			//Get Client Id and Client Secret from https://datamarket.azure.com/developer/applications/
			//Refer obtaining AccessToken (http://msdn.microsoft.com/en-us/library/hh454950.aspx) 
			MstfAzureMarketplaceAuthentication mstfAzureMarketplaceAuth = new MstfAzureMarketplaceAuthentication(
				AuthenticationContext.ClientId, AuthenticationContext.ClientSecret);
			return mstfAzureMarketplaceAuth.GetAccessToken();
		}
	}
}
