using System;

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

			var headerFormat =
				"OAuth oauth_consumer_key=\"{0}\", oauth_nonce=\"{1}\", " +
				"oauth_signature=\"{2}\", oauth_signature_method=\"{3}\", " +
				"oauth_timestamp=\"{4}\", " +
				"oauth_token=\"{5}\", " +
				"oauth_version=\"{6}\"";

			var result = string.Format(headerFormat,
				Uri.EscapeDataString(AuthenticationContext.ConsumerKey),
				Uri.EscapeDataString(requestParameters.OAuthNonce),
				Uri.EscapeDataString(oauthSignature),
				Uri.EscapeDataString(OAuthDefaults.SignatureMethod),
				Uri.EscapeDataString(requestParameters.OAuthTimestamp),
				Uri.EscapeDataString(AuthenticationContext.AccessToken),
				Uri.EscapeDataString(OAuthDefaults.Version)
				);

			return result;
		}
	}
}