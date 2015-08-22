﻿using System.Collections.Generic;
using System.Dynamic;
using System.IO;
using System.Net;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace Project.TranslateTwitter.Security.Demo
{
	public class Program3
	{
		public static void Main(string[] args)
		{
			var screenName = "Pleasure54";  // This account uses korena & japanese

			TestUserTimeLine(screenName);
		}

		private static void TestUserTimeLine(string screenName)
		{
			var count = 5;
			var authenticationContext = new AuthenticationContext();
			RequestParameters requestParameters = new TimelineRequestParameters(authenticationContext)
			{
				ScreenName = screenName,
				Count = count.ToString(),
			};

			var requestBuilder = new RequestBuilder(authenticationContext);
			var request = requestBuilder.GetRequest(requestParameters);

			using (HttpWebResponse response = request.GetResponse() as HttpWebResponse)
			using (Stream dataStream = response.GetResponseStream())
			{
				//Open the stream using a StreamReader for easy access.
				StreamReader reader = new StreamReader(dataStream);
				//Read the content.
				string responseFromServer = reader.ReadToEnd();
				dynamic dynamicObject = JsonConvert.DeserializeObject<List<ExpandoObject>>(
					responseFromServer, new ExpandoObjectConverter());
			}
		}
	}
}
