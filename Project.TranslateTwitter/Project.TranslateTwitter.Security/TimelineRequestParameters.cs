using System.Collections.Generic;
using System.Globalization;

namespace Project.TranslateTwitter.Security
{
	public class TimelineRequestParameters : RequestParameters
	{
		private const string SCREENNAME_PARAMETERNAME = "screen_name";
		private const string COUNT_PARAMETERNAME = "count";
		private const string DEFAULT_COUNT = "10";

		public override string BaseUrl { get; set; } = "https://api.twitter.com/1.1/statuses/user_timeline.json";
		public override string HttpMethod { get; set; } = "GET";

		public override Dictionary<string, string> QueryProperties { get; set; } = new Dictionary<string, string>();
		public override Dictionary<string, string> BodyProperties { get; set; } = new Dictionary<string, string>();

		public string ScreenName
		{
			get { return QueryProperties[SCREENNAME_PARAMETERNAME]; }
			set { QueryProperties[SCREENNAME_PARAMETERNAME] = value; }
		}

		public string Count
		{
			get { return QueryProperties[COUNT_PARAMETERNAME]; }
			set { QueryProperties[COUNT_PARAMETERNAME] = value; }
		}

		public TimelineRequestParameters(IAuthenticationContext authenticationContext)
			: base(authenticationContext)
		{
			Count = DEFAULT_COUNT;

			ScreenName = string.Empty;
			ScreenName = DEFAULT_COUNT;
		}
	}
}