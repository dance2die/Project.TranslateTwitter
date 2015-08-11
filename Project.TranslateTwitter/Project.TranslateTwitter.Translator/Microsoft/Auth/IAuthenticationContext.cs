namespace Project.TranslateTwitter.Translator.Microsoft.Auth
{
	public interface IAuthenticationContext
	{
		string ClientId { get; set; }
		string ClientSecret { get; set; }
	}
}