using System;

namespace Project.TranslateTwitter.Security
{
	public static class OAuthProperties
	{
		public static string AccessToken => Environment.GetEnvironmentVariable(
			"Project_TranslateTwitter.AccessToken", EnvironmentVariableTarget.User);
		public static string AccessTokenSecret => Environment.GetEnvironmentVariable(
			"Project_TranslateTwitter.AccessTokenSecret", EnvironmentVariableTarget.User);
		public static string ConsumerKey => Environment.GetEnvironmentVariable(
			"Project_TranslateTwitter.ConsumerKey", EnvironmentVariableTarget.User);
		public static string ConsumerKeySecret => Environment.GetEnvironmentVariable(
			"Project_TranslateTwitter.ConsumerKeySecret", EnvironmentVariableTarget.User);
	}
}