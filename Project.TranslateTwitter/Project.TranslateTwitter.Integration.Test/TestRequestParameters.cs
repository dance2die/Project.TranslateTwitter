using System.Collections.Generic;
using Project.TranslateTwitter.Security;

namespace Project.TranslateTwitter.Integration.Test
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
			= new Dictionary<string, string> { { "status", "Hello Ladies + Gentlemen, a signed OAuth request!" } };

		public TestRequestParameters(IAuthenticationContext authenticationContext)
			: base(authenticationContext)
		{
			OAuthNonce = "kYjzVBB8Y0ZFabxSWbWovY3uYSQ2pTgmZeNu2VS4cg";
			OAuthTimestamp = "1318622958";
		}
	}
}