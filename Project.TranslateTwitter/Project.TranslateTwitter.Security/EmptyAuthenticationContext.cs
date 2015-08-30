using System.Collections.Generic;

namespace Project.TranslateTwitter.Security
{
	/// <summary>
	/// Authentication Context with no data
	/// </summary>
	public class EmptyAuthenticationContext : IAuthenticationContext
	{
		private const string ACCESS_TOKEN_NAME = "oauth_token";
		private const string ACCESS_TOKEN_SECRET_NAME = "oauth_token_secret";

		public virtual string ConsumerKey { get; set; }
		public virtual string ConsumerKeySecret { get; set; }
		public virtual string AccessToken { get; set; }
		public virtual string AccessTokenSecret { get; set; }

		/// <summary>
		/// Override the current object's properties with the given authentication context.
		/// </summary>
		/// <param name="authenticationContext">Contains properties to override this object's properties with</param>
		/// <remarks>
		/// Credit goes to SimpleOAuth.Net project
		/// <see cref="https://github.com/djmc/SimpleOAuth.Net/blob/master/SimpleOAuth/Tokens.cs"/>
		/// </remarks>
		public virtual void MergeWith(IAuthenticationContext authenticationContext)
		{
			if (authenticationContext.ConsumerKey != null)
				ConsumerKey = authenticationContext.ConsumerKey;

			if (authenticationContext.ConsumerKeySecret != null)
				ConsumerKeySecret = authenticationContext.ConsumerKeySecret;

			if (authenticationContext.AccessToken != null)
				AccessToken = authenticationContext.AccessToken;

			if (authenticationContext.AccessTokenSecret != null)
				AccessTokenSecret = authenticationContext.AccessTokenSecret;
		}

		public virtual void MergeWith(IDictionary<string, string> dictionary)
		{
			var authenticationContext = BuildAuthenticationContext(dictionary);
			MergeWith(authenticationContext);
		}

		private IAuthenticationContext BuildAuthenticationContext(IDictionary<string, string> dictionary)
		{
			IAuthenticationContext result = new EmptyAuthenticationContext();
			string accessToken = dictionary[ACCESS_TOKEN_NAME];
			result.AccessToken = accessToken;

			string accessTokenSecret = dictionary[ACCESS_TOKEN_SECRET_NAME];
			result.AccessTokenSecret = accessTokenSecret;

			return result;
		}
	}
}
