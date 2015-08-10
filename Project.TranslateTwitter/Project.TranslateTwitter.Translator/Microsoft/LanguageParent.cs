using System.Net;

namespace Project.TranslateTwitter.Translator.Microsoft
{
	public abstract class LanguageParent
	{
		private const string REQUEST_URI = "http://api.microsofttranslator.com/v2/Http.svc";

		public IAuthenticationContext AuthenticationContext { get; set; }

		public LanguageParent(IAuthenticationContext authenticationContext)
		{
			AuthenticationContext = authenticationContext;
		}

		protected HttpWebRequest CreateRequest()
		{
			string uri = $"{REQUEST_URI}/{MethodName}{GetQueryString()}";
			var result = (HttpWebRequest)WebRequest.Create(uri);
			result.Headers.Add("Authorization", GetAuthorizationToken());

			return result;
		}

		protected abstract string MethodName { get; }
        protected abstract string GetQueryString();

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
