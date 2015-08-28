using System.Collections.Generic;
using Project.TranslateTwitter.Security;

namespace Project.TranslateTwitter.IntegrationDemo
{
	/// <summary>
	/// https://dev.twitter.com/oauth/overview/creating-signatures
	/// </summary>
	internal class TestRequestParameters : RequestParameters
	{
		public override string BaseUrl { get; set; } = "https://api.twitter.com/1/statuses/update.json";
		public override string HttpMethod { get; set; } = "POST";

		public override Dictionary<string, string> QueryProperties { get; set; } 
			= new Dictionary<string, string> { { "include_entities", "true" } };
		public override Dictionary<string, string> BodyProperties { get; set; } 
			= new Dictionary<string, string>(0);

		public TestRequestParameters(IAuthenticationContext authenticationContext)
			: base(authenticationContext)
		{
		}
	}
}