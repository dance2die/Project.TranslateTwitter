namespace Project.TranslateTwitter.Security.Demo
{
	/// <summary>
	/// Used to test
	/// </summary>
	/// <remarks>
	/// https://dev.twitter.com/oauth/overview/creating-signatures
	/// </remarks>
	public class TestAuthenticationContext : EmptyAuthenticationContext
	{
		public string ConsumerKey { get; set; } = string.Empty;
		public string ConsumerKeySecret { get; set; } = "kAcSOqF21Fu85e7zjz7ZN2U4ZRhfV3WpwPAoE3Z7kBw";
		public string AccessToken { get; set; } = string.Empty;
		public string AccessTokenSecret { get; set; } = "LswwdoUaIvS8ltyTt5jkRh4J50vUPVVHtR2YPi5kE";
	}
}