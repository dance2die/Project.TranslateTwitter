using System;

namespace Project.TranslateTwitter.Security.Demo
{
	public static class OAuthProperties
	{
		public static string Token => Environment.GetEnvironmentVariable("Project_TranslateTwitter.AccessToken", EnvironmentVariableTarget.User);
		public static string TokenSecret => Environment.GetEnvironmentVariable("Project_TranslateTwitter.AccessTokenSecret", EnvironmentVariableTarget.User);
		public static string ConsumerKey => Environment.GetEnvironmentVariable("Project_TranslateTwitter.ConsumerKey", EnvironmentVariableTarget.User);
		public static string ConsumerKeySecret => Environment.GetEnvironmentVariable("Project_TranslateTwitter.ConsumerKeySecret", EnvironmentVariableTarget.User);
	}
}