using System;
using System.IO;
using System.Net;
using System.Runtime.Serialization.Json;
using System.Text;
using System.Threading;
using System.Web;

namespace Project.TranslateTwitter.Translator.Mstf.Demo
{
	public class AdmAuthentication
	{
		public static readonly string DatamarketAccessUri = "https://datamarket.accesscontrol.windows.net/v2/OAuth2-13";
		private readonly string _clientId;
		private string _clientSecret;
		private readonly string _request;
		private AdmAccessToken _token;
		private readonly Timer _accessTokenRenewer;
		//Access token expires every 10 minutes. Renew it every 9 minutes only.
		private const int RefreshTokenDuration = 9;

		public AdmAuthentication(string clientId, string clientSecret)
		{
			_clientId = clientId;
			_clientSecret = clientSecret;
			//If clientid or client secret has special characters, encode before sending request
			_request = string.Format(
				"grant_type=client_credentials&client_id={0}&client_secret={1}&scope=http://api.microsofttranslator.com", 
				HttpUtility.UrlEncode(clientId), HttpUtility.UrlEncode(clientSecret));
			_token = HttpPost(DatamarketAccessUri, this._request);
			//renew the token every specified minutes
			_accessTokenRenewer = new Timer(
				OnTokenExpiredCallback, this, TimeSpan.FromMinutes(RefreshTokenDuration), TimeSpan.FromMilliseconds(-1));
		}

		public AdmAccessToken GetAccessToken()
		{
			return _token;
		}

		private void RenewAccessToken()
		{
			AdmAccessToken newAccessToken = HttpPost(DatamarketAccessUri, this._request);
			//swap the new token with old one
			//Note: the swap is thread unsafe
			this._token = newAccessToken;
			Console.WriteLine(string.Format("Renewed token for user: {0} is: {1}", this._clientId, this._token.access_token));
		}

		private void OnTokenExpiredCallback(object stateInfo)
		{
			try
			{
				RenewAccessToken();
			}
			catch (Exception ex)
			{
				Console.WriteLine(string.Format("Failed renewing access token. Details: {0}", ex.Message));
			}
			finally
			{
				try
				{
					_accessTokenRenewer.Change(TimeSpan.FromMinutes(RefreshTokenDuration), TimeSpan.FromMilliseconds(-1));
				}
				catch (Exception ex)
				{
					Console.WriteLine(string.Format("Failed to reschedule the timer to renew access token. Details: {0}", ex.Message));
				}
			}
		}

		private AdmAccessToken HttpPost(string DatamarketAccessUri, string requestDetails)
		{
			//Prepare OAuth request 
			WebRequest webRequest = WebRequest.Create(DatamarketAccessUri);
			webRequest.ContentType = "application/x-www-form-urlencoded";
			webRequest.Method = "POST";
			byte[] bytes = Encoding.ASCII.GetBytes(requestDetails);
			webRequest.ContentLength = bytes.Length;

			using (Stream outputStream = webRequest.GetRequestStream())
			{
				outputStream.Write(bytes, 0, bytes.Length);
			}

			using (WebResponse webResponse = webRequest.GetResponse())
			{
				DataContractJsonSerializer serializer = new DataContractJsonSerializer(typeof(AdmAccessToken));
				//Get deserialized object from JSON stream
				AdmAccessToken token = (AdmAccessToken)serializer.ReadObject(webResponse.GetResponseStream());
				return token;
			}
		}
	}
}