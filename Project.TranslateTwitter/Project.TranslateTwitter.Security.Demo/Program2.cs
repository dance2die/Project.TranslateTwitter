using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace Project.TranslateTwitter.Security.Demo
{
	public class Program2
	{
		public static void Main(string[] args)
		{
			// Need to learn how to create a signature first.
			// https://dev.twitter.com/oauth/overview/creating-signatures
			TestCreateSignature();
		}

		private static void TestCreateSignature()
		{
			string httpMethod = "GET";
			string baseUrl = "";
			Dictionary<string, string> requestParams = GetRequestParams();
			var signature = CreateSignature(httpMethod, baseUrl, requestParams);
			Console.WriteLine("Signature: {0}", signature);
		}

		private static string CreateSignature(
			string httpMethod, string baseUrl, IDictionary<string, string> requestParams)
		{
			var result = string.Empty;

			return result;
		}
    }
}
