using System.Collections.Generic;
using System.Linq;

namespace Project.TranslateTwitter.Security
{
	public abstract class RequestParameters
	{
		private const string OAUTH_NONCE = "oauth_nonce";
		private const string OAUTH_TIMESTAMP = "oauth_timestamp";

		public IAuthenticationContext AuthenticationContext { get; set; }

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
			get { return Headers.Values[OAUTH_NONCE]; }
			set { Headers.Values[OAUTH_NONCE] = value; }
		}

		public string OAuthTimestamp
		{
			get { return Headers.Values[OAUTH_TIMESTAMP]; }
			set { Headers.Values[OAUTH_TIMESTAMP] = value; }
		}

		protected RequestParameters(
			IAuthenticationContext authenticationContext)
			: this(authenticationContext, new RequestHeaders(authenticationContext))
		{
		}

		protected RequestParameters(
			IAuthenticationContext authenticationContext,
			RequestHeaders headers)
		{
			AuthenticationContext = authenticationContext;
			Headers = headers;
		}

		public Dictionary<string, string> GetParameters()
		{
			//var parameters = Headers.Values.Union(QueryProperties).ToDictionary(pair => pair.Key, pair => pair.Value);
			//parameters = parameters.Union(BodyProperties).ToDictionary(pair => pair.Key, pair => pair.Value);

			var parameters = CombineDictionaries(new Dictionary<int, Dictionary<string, string>>
			{
				{0, Headers.Values}, 
				{1, QueryProperties}, 
				{2, BodyProperties}, 
			});

			return parameters;
		}

		private Dictionary<string, string> CombineDictionaries(Dictionary<int, Dictionary<string, string>> dictionaries)
		{
			return new DictionaryMerger().MergeDictionaries(dictionaries);
		}

		public string GetRequestUrl()
		{
			var queryString = GetEncodedString(QueryProperties);
			if (string.IsNullOrWhiteSpace(queryString))
				return BaseUrl;
			return $"{BaseUrl}?{queryString}";
		}

		public string GetPostBody()
		{
			return GetEncodedString(BodyProperties);
		}

		private string GetEncodedString(IDictionary<string, string> dictionary)
		{
			return new DictionaryToStringJoiner().Join(dictionary);
		}
	}
}