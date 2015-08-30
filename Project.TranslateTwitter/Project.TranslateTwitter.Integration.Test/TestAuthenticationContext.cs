using Project.TranslateTwitter.Security;

namespace Project.TranslateTwitter.Integration.Test
{
	public class TestAuthenticationContext : EmptyAuthenticationContext
	{
		public override string ConsumerKey { get; set; } = "xvz1evFS4wEEPTGEFPHBog";
		public override string ConsumerKeySecret { get; set; } = "kAcSOqF21Fu85e7zjz7ZN2U4ZRhfV3WpwPAoE3Z7kBw";
		public override string AccessToken { get; set; } = "370773112-GmHxMAgYyLbNEtIKZeRNFsMKPR9EyMZeS9weJAEb";
		public override string AccessTokenSecret { get; set; } = "LswwdoUaIvS8ltyTt5jkRh4J50vUPVVHtR2YPi5kE";
	}
}