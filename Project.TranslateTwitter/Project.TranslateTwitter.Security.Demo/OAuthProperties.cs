namespace Project.TranslateTwitter.Security.Demo
{
	public static class OAuthProperties
	{
		public static string AccessToken;
		public static string AccessTokenSecret;
		public static string ConsumerKey;
		public static string ConsumerKeySecret;

		static OAuthProperties()
		{
			var context = new AuthenticationContext();

			AccessToken = context.AccessToken;
			AccessTokenSecret = context.AccessTokenSecret;
			ConsumerKey = context.ConsumerKey;
			ConsumerKeySecret = context.ConsumerKeySecret;
		}
	}
}