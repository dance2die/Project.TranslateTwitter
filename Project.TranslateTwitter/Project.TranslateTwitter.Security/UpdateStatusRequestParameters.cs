using System.Collections.Generic;

namespace Project.TranslateTwitter.Security
{
	public class UpdateStatusRequestParameters : RequestParameters
	{
		private const string STATUS_BODYCONTENT_NAME = "status";

		public override string BaseUrl { get; set; } = "https://api.twitter.com/1.1/statuses/update.json";
		public override string HttpMethod { get; set; } = "POST";
		public override Dictionary<string, string> QueryProperties { get; set; } = new Dictionary<string, string> { { "include_entities", "true" } };
		public override Dictionary<string, string> BodyProperties { get; set; } = new Dictionary<string, string>();

		public string Status
		{
			get { return BodyProperties[STATUS_BODYCONTENT_NAME]; }
			set { BodyProperties[STATUS_BODYCONTENT_NAME] = value; }
		}

		public UpdateStatusRequestParameters(IAuthenticationContext authenticationContext, string status)
			: base(authenticationContext)
		{
			Status = status;
		}
	}
}