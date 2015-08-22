using System.Collections.Generic;

namespace Project.TranslateTwitter.Security
{
	public class SignatureInput
	{
		public string HttpMethod { get; set; }
		public IDictionary<string, string> RequestParameters { get; set; }

		public SignatureInput(string httpMethod, IDictionary<string, string> requestParameters)
		{
			HttpMethod = httpMethod;
			RequestParameters = requestParameters;
		}
	}
}