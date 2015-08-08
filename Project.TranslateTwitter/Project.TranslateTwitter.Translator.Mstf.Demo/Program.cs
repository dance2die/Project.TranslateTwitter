using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Project.TranslateTwitter.Translator.Mstf.Demo
{
	public class Program
	{
		public static void Main(string[] args)
		{
			Test1();
		}

		/// <remarks>
		/// http://blogs.msdn.com/b/translation/p/gettingstarted2.aspx
		/// </remarks>
		private static void Test1()
		{

		}
	}

	public class AdmAccessToken
	{
		public string AccessToken { get; set; }
		public string TokenType { get; set; }
		public string ExpiresIn { get; set; }
		public string Scope { get; set; }
	}
}
