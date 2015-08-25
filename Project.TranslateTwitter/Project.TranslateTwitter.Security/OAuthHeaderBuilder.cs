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
			OAuthSignatureBuilder signatureBuilder = new OAuthSignatureBuilder(AuthenticationContext);
			string oauthSignature = signatureBuilder.CreateSignature(requestParameters);
			requestParameters.Headers.AddOAuthSignature(oauthSignature);

			return $"OAuth {GetParameterString(requestParameters.Headers.Values, ", ")}";
			//var headerFormat =
			//	"OAuth oauth_consumer_key=\"{0}\", oauth_nonce=\"{1}\", " +
			//	"oauth_signature=\"{2}\", oauth_signature_method=\"{3}\", " +
			//	"oauth_timestamp=\"{4}\", " +
			//	"oauth_token=\"{5}\", " +
			//	"oauth_version=\"{6}\"";

			//var result = string.Format(headerFormat,
			//	Uri.EscapeDataString(AuthenticationContext.ConsumerKey),
			//	Uri.EscapeDataString(requestParameters.OAuthNonce),
			//	Uri.EscapeDataString(oauthSignature),
			//	Uri.EscapeDataString(OAuthDefaults.SignatureMethod),
			//	Uri.EscapeDataString(requestParameters.OAuthTimestamp),
			//	Uri.EscapeDataString(AuthenticationContext.AccessToken),
			//	Uri.EscapeDataString(OAuthDefaults.Version)
			//	);
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