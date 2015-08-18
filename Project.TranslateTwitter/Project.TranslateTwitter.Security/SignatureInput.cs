using System.Collections.Generic;

namespace Project.TranslateTwitter.Security
{
	public class SignatureInput
	{
		public string HttpMethod { get; set; }
		public string BaseUrl { get; set; }
		public IDictionary<string, string> RequestParameters { get; set; }

		public SignatureInput(string httpMethod, string baseUrl, IDictionary<string, string> requestParameters)
		{
			HttpMethod = httpMethod;
			BaseUrl = baseUrl;
			RequestParameters = requestParameters;
		}
	}
}