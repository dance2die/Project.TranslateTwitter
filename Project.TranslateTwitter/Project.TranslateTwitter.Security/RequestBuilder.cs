using System.IO;
using System.Net;
using System.Text;

namespace Project.TranslateTwitter.Security
{
	public class RequestBuilder
	{
		public IAuthenticationContext AuthenticationContext { get; set; }

		public RequestBuilder(IAuthenticationContext authenticationContext)
		{
			AuthenticationContext = authenticationContext;
		}

		public HttpWebRequest GetRequest(RequestParameters requestParameters)
		{
			OAuthHeaderBuilder oAuthHeaderBuilder = new OAuthHeaderBuilder(AuthenticationContext);
			var authHeader = oAuthHeaderBuilder.BuildAuthHeader(requestParameters);

			ServicePointManager.Expect100Continue = false;

			var queryUrl = requestParameters.GetRequestUrl();
			var request = (HttpWebRequest)WebRequest.Create(queryUrl);
			request.Headers.Add("Authorization", authHeader);
			request.Method = requestParameters.HttpMethod;
			request.ContentType = "application/x-www-form-urlencoded";

			string postBody = requestParameters.GetPostBody();

			if (!string.IsNullOrWhiteSpace(postBody))
			{
				using (Stream stream = request.GetRequestStream())
				{
					byte[] content = Encoding.ASCII.GetBytes(postBody);
					stream.Write(content, 0, content.Length);
				}
			}

			return request;
		}
	}
}