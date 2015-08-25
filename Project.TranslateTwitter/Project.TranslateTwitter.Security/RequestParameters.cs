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

		/// <summary>
		/// Web API Resource URL
		/// </summary>
		public abstract string BaseUrl { get; set; }
		public abstract string HttpMethod { get; set; }

		public abstract Dictionary<string, string> QueryProperties { get; set; }
		public abstract Dictionary<string, string> BodyProperties { get; set; }
		public RequestHeaders Headers { get; set; }

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
				  new RequestHeaders(authenticationContext).Values)
		{
		}

		protected RequestParameters(
			IAuthenticationContext authenticationContext, Dictionary<string, string> commonParameters)
			: this(authenticationContext, commonParameters,
				  new RequestHeaders(authenticationContext))
		{
		}

		protected RequestParameters(
			IAuthenticationContext authenticationContext,
			Dictionary<string, string> commonParameters,
			RequestHeaders headers)
		{
			AuthenticationContext = authenticationContext;
			CommonParameters = commonParameters;
			Headers = headers;
		}

		public Dictionary<string, string> GetParameters()
		{
			var parameters = Headers.Values.Union(QueryProperties).ToDictionary(pair => pair.Key, pair => pair.Value);
			parameters = parameters.Union(BodyProperties).ToDictionary(pair => pair.Key, pair => pair.Value);

			return parameters;
		}

		//protected string GetParameterString()
		//{
		//	return GetQueryString(GetParameters());
		//}

		private string GetQueryString(IDictionary<string, string> query)
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
			var queryString = GetQueryString(QueryProperties);
			if (string.IsNullOrWhiteSpace(queryString))
				return BaseUrl;
			return $"{BaseUrl}?{HttpUtility.UrlDecode(queryString)}";
		}
	}
}