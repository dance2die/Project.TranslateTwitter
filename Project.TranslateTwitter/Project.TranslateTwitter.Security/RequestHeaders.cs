﻿using System;
using System.Collections.Generic;
using System.Text;

namespace Project.TranslateTwitter.Security
{
	public class RequestHeaders
	{
		public IAuthenticationContext AuthenticationContext { get; set; }
		public Dictionary<string, string> Values { get; }

		public RequestHeaders(IAuthenticationContext authenticationContext)
		{
			AuthenticationContext = authenticationContext;
			Values = GetCommonHeaders();
		}

		public void Add(string key, string value)
		{
			Values[key] = value;
		}

		public void AddOAuthSignature(string signature)
		{
			Values.Add("oauth_signature", signature);
		}

		protected Dictionary<string, string> GetCommonHeaders()
		{
			var oauthConsumerKey = AuthenticationContext.ConsumerKey;
			var oauthNonce = Convert.ToBase64String(Encoding.ASCII.GetBytes(DateTime.Now.Ticks.ToString()));
			var oauthTimestamp = GetTimeStamp();
			var oauthToken = AuthenticationContext.AccessToken;

			return new Dictionary<string, string>
			{
				{"oauth_consumer_key", oauthConsumerKey },
				{"oauth_nonce", oauthNonce },
				{"oauth_signature_method", OAuthDefaults.SignatureMethod },
				{"oauth_timestamp", oauthTimestamp },
				{"oauth_token", oauthToken },
				{"oauth_version", OAuthDefaults.Version }
			};
		}

		private string GetTimeStamp()
		{
			var timeSpan = DateTime.UtcNow - new DateTime(1970, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc);
			return Convert.ToInt64(timeSpan.TotalSeconds).ToString();
		}
	}
}
