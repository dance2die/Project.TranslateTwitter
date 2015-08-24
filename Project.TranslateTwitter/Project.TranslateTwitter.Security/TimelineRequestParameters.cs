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

		public string ScreenName
		{
			get { return CommonParameters[SCREENNAME_PARAMETERNAME]; }
			set { CommonParameters[SCREENNAME_PARAMETERNAME] = value; }
		}

		public string Count
		{
			get { return CommonParameters[COUNT_PARAMETERNAME]; }
			set { CommonParameters[COUNT_PARAMETERNAME] = value; }
		}

		public TimelineRequestParameters(IAuthenticationContext authenticationContext)
			: base(authenticationContext)
		{
			Count = DEFAULT_COUNT;
		}

		public override Dictionary<string, string> GetQueryProperties()
		{
			return new Dictionary<string, string>
			{
				[SCREENNAME_PARAMETERNAME] = ScreenName,
				[COUNT_PARAMETERNAME] = Count.ToString(CultureInfo.InvariantCulture)
			};
		}
	}
}