using System;
using System.Collections.Specialized;
using Moq;
using NUnit.Framework;

using Syw.Client;
using Syw.Client.WebClient;

namespace Syw.Client.Tests
{
	[TestFixture]
	public class SywClientTests
	{
		[Test]
		public void Get_Search_CreateSearchRequest()
		{

			var mockedWebClientBuilder = new Mock<IWebClientBuilder>();
			var mockedWebClient = new Mock<IWebClient>();
			mockedWebClientBuilder.Setup(x => x.Create()).Returns(mockedWebClient.Object);

			var result = new User()
			{
				Name = "batman"
			};
			var parameters = new NameValueCollection();
			parameters.Add("q","batman");
			parameters.Add("token","");
			parameters.Add("hash", "e3b0c44298fc1c149afbf4c8996fb92427ae41e4649b934ca495991b7852b855");
			
			
			mockedWebClient.Setup(x => x.GetJson<User>(
				new Uri("https://platform.shopyourway.com/products/search"), parameters))
				.Returns(result);

			var client = new SywClient(mockedWebClientBuilder.Object, new OnlineTokenProvider("", ""), new PlatformSettings());
			var user = client.Get<User>("products/search", new { q = "batman" });
			Assert.AreEqual(user.Name,"batman");
		}
	}

	public class User
	{
		public string Name { get; set; }
	}
}
