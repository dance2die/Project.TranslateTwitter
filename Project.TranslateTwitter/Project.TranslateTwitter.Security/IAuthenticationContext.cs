namespace Project.TranslateTwitter.Security
{
	public interface IAuthenticationContext
	{
		string ConsumerKey { get; }
		string ConsumerKeySecret { get; }
		string AccessToken { get; set; }
		string AccessTokenSecret { get; set; }
	}
}