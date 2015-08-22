using System.Collections.Specialized;
using System.Globalization;
using System.Web;

namespace Project.TranslateTwitter.Security
{
	public class TimelineRequestParameters : RequestParameters
	{
		private const string DEFAULT_COUNT = "5";

		public override string ResourceUrl { get; set; } = "https://api.twitter.com/1.1/statuses/user_timeline.json";
		public override string HttpMethod { get; set; } = "GET";

		public string ScreenName
		{
			get { return CommonParameters[SCREEN_NAME]; }
			set { CommonParameters[SCREEN_NAME] = value; }
		}

		public string Count
		{
			get { return CommonParameters[COUNT]; }
			set { CommonParameters[COUNT] = value; }
		}

		public TimelineRequestParameters(IAuthenticationContext authenticationContext)
			: base(authenticationContext)
		{
			Count = DEFAULT_COUNT;
		}

		/// <param name="apiUrl">Twitter API URL</param>
		/// <returns></returns>
		public override string BuildRequestUrl(string apiUrl)
		{
			NameValueCollection query = new NameValueCollection
			{
				[SCREEN_NAME] = ScreenName,
				[COUNT] = Count.ToString(CultureInfo.InvariantCulture)
			};

			var queryString = GetQueryString(query);
			var result = $"{apiUrl}?{HttpUtility.UrlDecode(queryString)}";

			return result;
		}
	}
}