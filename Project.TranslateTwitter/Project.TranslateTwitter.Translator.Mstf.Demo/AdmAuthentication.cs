using System;
using System.IO;
using System.Net;
using System.Runtime.Serialization.Json;
using System.Text;
using System.Threading;
using System.Web;

namespace Project.TranslateTwitter.Translator.Mstf.Demo
{
	/// <remarks>
	/// https://msdn.microsoft.com/en-us/library/Ff512411.aspx
	/// </remarks>
	public class AdmAuthentication
	{
		private const string DATAMARKET_ACCESS_URI = "https://datamarket.accesscontrol.windows.net/v2/OAuth2-13";
		//Access token expires every 10 minutes. Renew it every 9 minutes only.
		private const int REFRESH_TOKEN_DURATION = 9;
		private const string CONTENT_TYPE = "application/x-www-form-urlencoded";

		private readonly string _request;
		private AdmAccessToken _token;
		private readonly Timer _accessTokenRenewer;

		public string ClientId { get; }
		public string ClientSecret { get; }

		public AdmAuthentication(string clientId, string clientSecret)
		{
			ClientId = clientId;
			ClientSecret = clientSecret;
			
			//If clientid or client secret has special characters, encode before sending request
			_request = string.Format(
				"grant_type=client_credentials&client_id={0}&client_secret={1}&scope=http://api.microsofttranslator.com", 
				HttpUtility.UrlEncode(clientId), HttpUtility.UrlEncode(ClientSecret));
			_token = HttpPost(DATAMARKET_ACCESS_URI, _request);
			//renew the token every specified minutes

			_accessTokenRenewer = new Timer(OnTokenExpiredCallback, this, 
				TimeSpan.FromMinutes(REFRESH_TOKEN_DURATION), TimeSpan.FromMilliseconds(-1));
		}

		public AdmAccessToken GetAccessToken()
		{
			return _token;
		}

		private void OnTokenExpiredCallback(object stateInfo)
		{
			try
			{
				RenewAccessToken();
			}
			catch (Exception ex)
			{
				Console.WriteLine($"Failed renewing access token. Details: {ex.Message}");
			}
			finally
			{
				try
				{
					_accessTokenRenewer.Change(TimeSpan.FromMinutes(REFRESH_TOKEN_DURATION), TimeSpan.FromMilliseconds(-1));
				}
				catch (Exception ex)
				{
					Console.WriteLine($"Failed to reschedule the timer to renew access token. Details: {ex.Message}");
				}
			}
		}

		private void RenewAccessToken()
		{
			AdmAccessToken newAccessToken = HttpPost(DATAMARKET_ACCESS_URI, _request);
			//swap the new token with old one
			//Note: the swap is thread unsafe
			_token = newAccessToken;
		}

		private AdmAccessToken HttpPost(string datamarketAccessUri, string requestDetails)
		{
			var webRequest = Request(datamarketAccessUri, requestDetails);
			using (WebResponse webResponse = webRequest.GetResponse())
			{
				DataContractJsonSerializer serializer = new DataContractJsonSerializer(typeof(AdmAccessToken));
				//Get deserialized object from JSON stream
				AdmAccessToken token = (AdmAccessToken)serializer.ReadObject(webResponse.GetResponseStream());
				return token;
			}
		}

		private WebRequest Request(string datamarketAccessUri, string requestDetails)
		{
			var webRequest = GetRequest(datamarketAccessUri, requestDetails);
			using (Stream outputStream = webRequest.GetRequestStream())
			{
				byte[] bytes = Encoding.ASCII.GetBytes(requestDetails);
				outputStream.Write(bytes, 0, bytes.Length);
			}
			return webRequest;
		}

		private WebRequest GetRequest(string uri, string requestDetails)
		{
			//Prepare OAuth request 
			var result = WebRequest.Create(uri);
			result.ContentType = CONTENT_TYPE;
			result.Method = "POST";

			byte[] bytes = Encoding.ASCII.GetBytes(requestDetails);
			result.ContentLength = bytes.Length;

			return result;
		}
	}
}