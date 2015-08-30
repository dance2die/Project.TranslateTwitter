using Project.TranslateTwitter.Security;

namespace Project.TranslateTwitter.IntegrationDemo
{
	public class TestAuthenticationContext : EmptyAuthenticationContext
	{
		public string ConsumerKey { get; set; } = "xvz1evFS4wEEPTGEFPHBog";
		public string ConsumerKeySecret { get; set; } = "kAcSOqF21Fu85e7zjz7ZN2U4ZRhfV3WpwPAoE3Z7kBw";
		public string AccessToken { get; set; } = "370773112-GmHxMAgYyLbNEtIKZeRNFsMKPR9EyMZeS9weJAEb";
		public string AccessTokenSecret { get; set; } = "LswwdoUaIvS8ltyTt5jkRh4J50vUPVVHtR2YPi5kE";
	}
}