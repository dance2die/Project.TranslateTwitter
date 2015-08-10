namespace Project.TranslateTwitter.Translator.Microsoft
{
	public interface IAuthenticationContext
	{
		string ClientId { get; set; }
		string ClientSecret { get; set; }
	}
}