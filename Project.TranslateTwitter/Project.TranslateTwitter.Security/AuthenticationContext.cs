using System;

namespace Project.TranslateTwitter.Security
{
	public class AuthenticationContext : EmptyAuthenticationContext
	{
		public override string ConsumerKey => Environment.GetEnvironmentVariable(
			"Project_TranslateTwitter.ConsumerKey", EnvironmentVariableTarget.User);
		public override string ConsumerKeySecret => Environment.GetEnvironmentVariable(
			"Project_TranslateTwitter.ConsumerKeySecret", EnvironmentVariableTarget.User);
		public override string AccessToken { get; set; } = Environment.GetEnvironmentVariable(
			"Project_TranslateTwitter.AccessToken", EnvironmentVariableTarget.User);
		public override string AccessTokenSecret { get; set; } = Environment.GetEnvironmentVariable(
			"Project_TranslateTwitter.AccessTokenSecret", EnvironmentVariableTarget.User);
	}
}