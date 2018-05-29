using System;
using System.Collections.Specialized;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Web;
using Newtonsoft.Json;

namespace Syw.Client.WebClient
{
	public interface IWebClient
	{
		Action<HttpWebRequest> CustomizeRequest { get; set; }
		string Request(Uri url, HttpMethod method, string body);

	}

	public class WebClient : IWebClient
	{
		public WebClient()
		{
			Encoding = Encoding.UTF8;
			RequestTimeout = 30000; // 30 seconds default timeout
			ReadWriteTimeout = 5000; // Allow 5 seconds between reads or writes by default.
		}

		public int RequestTimeout { get; set; }

		public int ReadWriteTimeout { get; set; }

		public Encoding Encoding { get; set; }

		public Action<HttpWebRequest> CustomizeRequest { get; set; }

		public string Request(Uri url, HttpMethod method, string body)
		{
			return UploadValues(url, method, Encoding.GetBytes(body));
		}

		private string UploadValues(Uri uri, HttpMethod method, byte[] bytes)
		{
			var request = GetWebRequest(uri);
			request.Method = method.Method;
			request.ContentLength = bytes.Length;

			using (var dataStream = request.GetRequestStream())
				dataStream.Write(bytes, 0, bytes.Length);
			return ReadResponse(request);
		}

		private string ReadResponse(WebRequest request)
		{
			using (var response = request.GetResponse())
			using (var reader = new StreamReader(response.GetResponseStream(), Encoding))
			{
				return reader.ReadToEnd();
			}
		}

		private WebRequest GetWebRequest(Uri url)
		{
			var request = WebRequest.Create(url);
			CustomizeRequestInternal(request);
			return request;
		}

		private void CustomizeRequestInternal(WebRequest wr)
		{
			wr.Timeout = RequestTimeout;
			if (!(wr is HttpWebRequest hwr)) return;
			CustomizeRequest(hwr);

			hwr.AutomaticDecompression = DecompressionMethods.GZip | DecompressionMethods.Deflate;
			hwr.ReadWriteTimeout = ReadWriteTimeout;
		}
	}
}
