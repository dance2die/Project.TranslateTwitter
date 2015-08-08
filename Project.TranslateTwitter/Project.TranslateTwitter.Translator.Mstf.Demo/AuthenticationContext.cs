namespace Project.TranslateTwitter.Translator.Mstf.Demo
{
	public class AuthenticationContext : IAuthenticationContext
	{
		public string ClientId { get; set; }
		public string ClientSecret { get; set; }

		public AuthenticationContext(string clientId, string clientSecret)
		{
			ClientId = clientId;
			ClientSecret = clientSecret;
		}
	}
}