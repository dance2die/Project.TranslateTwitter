using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;

namespace Project.TranslateTwitter.Security
{
	/// <summary>
	/// Build OAuth Signature
	/// </summary>
	/// <remarks>
	/// Twitter Signature Generation Specification: https://dev.twitter.com/oauth/overview/creating-signatures
	/// </remarks>
	public class OAuthSignatureBuilder
	{
		private IAuthenticationContext AuthenticationContext { get; }

		public OAuthSignatureBuilder(IAuthenticationContext authenticationContext)
		{
			AuthenticationContext = authenticationContext;
		}

		public string CreateSignature(RequestParameters parameters)
		{
			// 3.) Creating the signature base string
			string signatureBaseString = GetSignatureBaseString(parameters);

			// 4.) Getting a signing key
			string signingKey = GetSigningKey();

			// 5.) Calculating the signature
			var result = CalculateSignature(signingKey, signatureBaseString);

			return result;
		}

		public string GetSignatureBaseString(RequestParameters parameters)
		{
			const string separator = "&";

			var result = new StringBuilder();

			// 1.) Convert the HTTP Method to uppercase and set the output string equal to this value.
			result.Append(parameters.HttpMethod.ToUpperInvariant());

			// 2.) Append the ‘&’ character to the output string.
			result.Append(separator);

			// 3.) Percent encode the URL and append it to the output string.
			result.Append(Uri.EscapeDataString(parameters.BaseUrl));

			// 4.) Append the ‘&’ character to the output string.
			result.Append(separator);

			// 5.) Percent encode the parameter string and append it to the output string.
			var baseString = GetParameterString(parameters.GetParameters(), separator);
			result.Append(Uri.EscapeDataString(baseString));

			return result.ToString();
		}

		private string GetParameterString(IDictionary<string, string> requestParameters, string separator)
		{
			return new DictionaryToStringJoiner().Join(requestParameters, separator);
        }

		private string GetSigningKey()
		{
			return $"{AuthenticationContext.ConsumerKeySecret}&{AuthenticationContext.AccessTokenSecret}";
		}

		private string CalculateSignature(string signingKey, string signatureBaseString)
		{
			using (HMACSHA1 hasher = new HMACSHA1(Encoding.ASCII.GetBytes(signingKey)))
			{
				return Convert.ToBase64String(hasher.ComputeHash(Encoding.ASCII.GetBytes(signatureBaseString)));
			}
		}
	}
}
