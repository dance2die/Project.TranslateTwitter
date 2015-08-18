namespace Project.TranslateTwitter.Security
{
	/// <summary>
	/// Used to test
	/// </summary>
	/// <remarks>
	/// https://dev.twitter.com/oauth/overview/creating-signatures
	/// </remarks>
	public class TestAuthenticationContext : IAuthenticationContext
	{
		public string ConsumerKey => string.Empty;
		public string ConsumerKeySecret => "kAcSOqF21Fu85e7zjz7ZN2U4ZRhfV3WpwPAoE3Z7kBw";
		public string AccessToken => string.Empty;
		public string AccessTokenSecret => "LswwdoUaIvS8ltyTt5jkRh4J50vUPVVHtR2YPi5kE";
	}
}