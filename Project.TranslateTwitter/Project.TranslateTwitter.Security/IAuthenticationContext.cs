namespace Project.TranslateTwitter.Security
{
	public interface IAuthenticationContext
	{
		string ConsumerKey { get; }
		string ConsumerKeySecret { get; }
		string AccessToken { get; }
		string AccessTokenSecret { get; }
	}
}