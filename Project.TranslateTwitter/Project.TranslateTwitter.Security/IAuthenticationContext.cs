namespace Project.TranslateTwitter.Security
{
	public interface IAuthenticationContext
	{
		string ConsumerKey { get; set; }
		string ConsumerKeySecret { get; set; }
		string AccessToken { get; set; }
		string AccessTokenSecret { get; set; }

		/// <summary>
		/// Override the current object's properties with the given authentication context.
		/// </summary>
		/// <param name="authenticationContext">Contains properties to override this object's properties with</param>
		/// <remarks>
		/// Credit goes to SimpleOAuth.Net project
		/// <see cref="https://github.com/djmc/SimpleOAuth.Net/blob/master/SimpleOAuth/Tokens.cs"/>
		/// </remarks>
		void MergeWith(IAuthenticationContext authenticationContext);
	}
}