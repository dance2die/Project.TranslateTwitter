using System.Collections.Generic;

namespace Project.TranslateTwitter.Security
{
	public class OAuthHeaderBuilder
	{
		public IAuthenticationContext AuthenticationContext { get; set; }

		public OAuthHeaderBuilder(IAuthenticationContext authenticationContext)
		{
			AuthenticationContext = authenticationContext;
		}

		public string BuildAuthHeader(RequestParameters requestParameters)
		{
			return $"OAuth {GetParameterString(requestParameters.Headers.Values, ", ")}";
		}

		private string GetParameterString(IDictionary<string, string> requestParameters, string separator = "&")
		{
			return new DictionaryToStringJoiner().Join(requestParameters, separator);
		}

	}
}