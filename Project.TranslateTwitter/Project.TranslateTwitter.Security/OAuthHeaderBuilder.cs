using System;
using System.Collections.Generic;
using System.Linq;

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
			var query = (from requestParam in requestParameters
						 orderby requestParam.Key
						 select new { requestParam.Key, requestParam.Value }).ToList();

			List<string> paramList = new List<string>(query.Count);
			foreach (var requestParam in query)
			{
				paramList.Add($"{requestParam.Key}=\"{Uri.EscapeDataString(requestParam.Value)}\"");
			}

			return string.Join(separator, paramList.ToArray());
		}

	}
}