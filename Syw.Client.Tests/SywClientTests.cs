using System;
using System.Collections.Specialized;
using System.Data.Common;
using System.Net.Http;
using Moq;
using Newtonsoft.Json;
using NUnit.Framework;
using Syw.Client.WebClient;

namespace Syw.Client.Tests
{
    [TestFixture]
    public class SywClientTests
    {
	
        [Test]
        public void Get_GetCurrentUser_CreateCurrentUserRequest()
        {

            var mockedWebClientBuilder = new Mock<IWebClientBuilder>();
            var mockedWebClient = new Mock<IWebClient>();
            mockedWebClientBuilder.Setup(x => x.Create()).Returns(mockedWebClient.Object);

            var result = new User()
            {
                Name = "batman"
            };
            var parameters = new NameValueCollection
            {
                {"token", ""},
                {"hash", "e3b0c44298fc1c149afbf4c8996fb92427ae41e4649b934ca495991b7852b855"},
                {"q", "batman"}
            };

			
	        mockedWebClient.Setup(x => x.Request(
			        new Uri("https://platform.shopyourway.com/users/current"), HttpMethod.Get, null))
		        .Returns("{name:'batman'}");

            var client = new SywClient(mockedWebClientBuilder.Object, new OnlineTokenProvider("", ""), new PlatformSettings());
            var user = client.Get<User>("products/search", new { q = "batman" });
            Assert.AreEqual(user.Name,"batman");
        }
		
        [Test]
        public void Post_Search_CreateSearchRequest()
        {

            var mockedWebClientBuilder = new Mock<IWebClientBuilder>();
            var mockedWebClient = new Mock<IWebClient>();
            mockedWebClientBuilder.Setup(x => x.Create()).Returns(mockedWebClient.Object);

            var result = new User()
            {
                Name = "batman"
            };
            var parameters = new NameValueCollection
            {
                {"token", ""},
                {"hash", "e3b0c44298fc1c149afbf4c8996fb92427ae41e4649b934ca495991b7852b855"},
                {"name", "batman"}
            };

			
	        mockedWebClient.Setup(x => x.Request(
			        new Uri("https://platform.shopyourway.com/users/current"), HttpMethod.Get, null))
		        .Returns("{name:'batman'}");

            var client = new SywClient(mockedWebClientBuilder.Object, new OnlineTokenProvider("", ""), new PlatformSettings());
            var user = client.Post<User>("users/create", new { name = "batman" });

            Assert.AreEqual(user.Name,"batman");
        }

        [Test]
        public void Get_NoParameters_CreateRequestWithoutParameters()
        {

            var mockedWebClientBuilder = new Mock<IWebClientBuilder>();
            var mockedWebClient = new Mock<IWebClient>();
            mockedWebClientBuilder.Setup(x => x.Create()).Returns(mockedWebClient.Object);

            var parameters = new NameValueCollection
            {
                {"token", ""},
                {"hash", "e3b0c44298fc1c149afbf4c8996fb92427ae41e4649b934ca495991b7852b855"},
            };

            mockedWebClient.Setup(x => x.Request(
                    new Uri("https://platform.shopyourway.com/users/current"), HttpMethod.Get, null))
                .Returns("{name:'batman'}");

            var client = new SywClient(mockedWebClientBuilder.Object, new OnlineTokenProvider("", ""), new PlatformSettings());
            var user = client.Get<User>("users/current");
            Assert.AreEqual(user.Name,"batman");
        }
		
        [Test]
        public void DynamicGet_NoParameters_CreateRequestWithoutParameters()
        {

            var mockedWebClientBuilder = new Mock<IWebClientBuilder>();
            var mockedWebClient = new Mock<IWebClient>();
            mockedWebClientBuilder.Setup(x => x.Create()).Returns(mockedWebClient.Object);

            var result = new User()
            {
                Name = "batman"
            };
            var parameters = new NameValueCollection
            {
                {"token", ""},
                {"hash", "e3b0c44298fc1c149afbf4c8996fb92427ae41e4649b934ca495991b7852b855"},
            };


	        mockedWebClient.Setup(x => x.Request(
			        new Uri("https://platform.shopyourway.com/users/current"), HttpMethod.Get, null))
		        .Returns("{name:'batman'}");

            var client = new SywClient(mockedWebClientBuilder.Object, new OnlineTokenProvider("", ""), new PlatformSettings());
            var user = client.GetDynamicApi().Users.Current.Get();
            Assert.AreEqual(user.Name,"batman");
        }
        
        [Test]
        public void DynamicGet_WithParameters_CreateRequestWithParameters()
        {

            var mockedWebClientBuilder = new Mock<IWebClientBuilder>();
            var mockedWebClient = new Mock<IWebClient>();
            mockedWebClientBuilder.Setup(x => x.Create()).Returns(mockedWebClient.Object);

            var result = new User()
            {
                Name = "batman"
            };
            var parameters = new NameValueCollection
            {
                {"token", ""},
                {"hash", "e3b0c44298fc1c149afbf4c8996fb92427ae41e4649b934ca495991b7852b855"},
                {"id","3"}
            };


	        mockedWebClient.Setup(x => x.Request(
			        new Uri("https://platform.shopyourway.com/users/get"), HttpMethod.Get, null))
		        .Returns("{name:'batman'}");

            var client = new SywClient(mockedWebClientBuilder.Object, new OnlineTokenProvider("", ""), new PlatformSettings());
            var user = client.GetDynamicApi().Users.Get.Get(userId: 3);
            Assert.AreEqual(user.Name,"batman");
        }
    }
}