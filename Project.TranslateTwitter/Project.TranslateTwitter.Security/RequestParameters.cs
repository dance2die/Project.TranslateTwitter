using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Project.TranslateTwitter.Security
{
	public abstract class RequestParameters
	{
		protected const string OAUTH_NONCE = "oauth_nonce";
		protected const string OAUTH_TIMESTAMP = "oauth_timestamp";

		public IAuthenticationContext AuthenticationContext { get; set; }
		public Dictionary<string, string> CommonParameters { get; set; }
		public Dictionary<string, string> Headers { get; set; }

		public string OAuthNonce
		{
			get { return CommonParameters[OAUTH_NONCE]; }
			set { CommonParameters[OAUTH_NONCE] = value; }
		}

		public string OAuthTimestamp
		{
			get { return CommonParameters[OAUTH_TIMESTAMP]; }
			set { CommonParameters[OAUTH_TIMESTAMP] = value; }
		}

		protected RequestParameters(IAuthenticationContext authenticationContext)
			: this(authenticationContext,
				  new RequestHeaders(authenticationContext).Headers,
				  new RequestHeaders(authenticationContext).Headers)
		{
		}

		protected RequestParameters(
			IAuthenticationContext authenticationContext, Dictionary<string, string> commonParameters)
			: this(authenticationContext, commonParameters,
				  new RequestHeaders(authenticationContext).Headers)
		{
		}

		protected RequestParameters(
			IAuthenticationContext authenticationContext,
			Dictionary<string, string> commonParameters,
			Dictionary<string, string> headers)
		{
			AuthenticationContext = authenticationContext;
			CommonParameters = commonParameters;
			Headers = headers;
		}

		/// <summary>
		/// Web API Resource URL
		/// </summary>
		public abstract string BaseUrl { get; set; }
		public abstract string HttpMethod { get; set; }
		/// <summary>
		/// Query String parameters to Base URL
		/// </summary>
		public abstract Dictionary<string, string> GetQueryProperties();

		protected string GetQueryString(IDictionary<string, string> query)
		{
			var array = (from key in query.Keys
						 let value = query[key]
						 orderby key
						 where !string.IsNullOrWhiteSpace(value)
						 select $"{HttpUtility.UrlEncode(key)}={HttpUtility.UrlEncode(value)}").ToArray();
			return string.Join("&", array);
		}

		public string GetRequestUrl()
		{
			var queryString = GetQueryString(GetQueryProperties());
			var result = $"{BaseUrl}?{HttpUtility.UrlDecode(queryString)}";
			return result;
		}
	}
}