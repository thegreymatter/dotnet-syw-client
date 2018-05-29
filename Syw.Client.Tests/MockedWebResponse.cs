using System.IO;
using System.Net;
using System.Text;

namespace Syw.Client.Tests
{
	public class MockedWebResponse : WebResponse
	{
		public string Response { get; set; }


		public override Stream GetResponseStream()
		{
			var byteArray = Encoding.ASCII.GetBytes(Response);
			return new MemoryStream(byteArray);

		}
	}
}