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

		public string CreateSignature(SignatureInput input)
		{
			// 3.) Creating the signature base string
			string signatureBaseString = GetSignatureBaseString(input);

			// 4.) Getting a signing key
			string signingKey = GetSigningKey();

			// 5.) Calculating the signature
			var result = CalculateSignature(signingKey, signatureBaseString);

			return result;
		}

		private string GetSignatureBaseString(SignatureInput input)
		{
			const string separator = "&";

			var result = new StringBuilder();

			// 1.) Convert the HTTP Method to uppercase and set the output string equal to this value.
			result.Append(input.HttpMethod.ToUpperInvariant());

			// 2.) Append the ‘&’ character to the output string.
			result.Append(separator);

			// 3.) Percent encode the URL and append it to the output string.
			result.Append(Uri.EscapeDataString(input.BaseUrl));

			// 4.) Append the ‘&’ character to the output string.
			result.Append(separator);

			// 5.) Percent encode the parameter string and append it to the output string.
			var baseString = GetParameterString(input.RequestParameters, separator);
			result.Append(Uri.EscapeDataString(baseString));

			return result.ToString();
		}

		private string GetParameterString(IDictionary<string, string> requestParams, string separator)
		{
			var query = (from requestParam in requestParams
							 // According to Twitter spec,
							 // Sort the list of parameters alphabetically[1] by encoded key[2].
						 orderby requestParam.Key
						 select new { requestParam.Key, requestParam.Value }).ToList();

			List<string> paramList = new List<string>(query.Count);
			foreach (var requestParam in query)
			{
				paramList.Add($"{requestParam.Key}={Uri.EscapeDataString(requestParam.Value)}");
			}

			return string.Join(separator, paramList.ToArray());
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
