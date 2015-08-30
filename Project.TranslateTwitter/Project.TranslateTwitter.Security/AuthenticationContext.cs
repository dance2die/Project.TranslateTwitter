using System;

namespace Project.TranslateTwitter.Security
{
	public class AuthenticationContext : IAuthenticationContext
	{
		public string ConsumerKey => Environment.GetEnvironmentVariable(
			"Project_TranslateTwitter.ConsumerKey", EnvironmentVariableTarget.User);
		public string ConsumerKeySecret => Environment.GetEnvironmentVariable(
			"Project_TranslateTwitter.ConsumerKeySecret", EnvironmentVariableTarget.User);
		public string AccessToken { get; set; } = Environment.GetEnvironmentVariable(
			"Project_TranslateTwitter.AccessToken", EnvironmentVariableTarget.User);
		public string AccessTokenSecret { get; set; } = Environment.GetEnvironmentVariable(
			"Project_TranslateTwitter.AccessTokenSecret", EnvironmentVariableTarget.User);
	}
}