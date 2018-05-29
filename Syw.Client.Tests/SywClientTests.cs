using System;
using System.Collections.Specialized;
using System.Net;
using System.Net.Http;
using System.Numerics;
using Moq;
using Syw.Client.Common.Exceptions;
using Syw.Client.WebClient;
using Xunit;

namespace Syw.Client.Tests
{
    public class SywClientTests
    {
	
        [Fact]
        public void Get_GetCurrentUser_CreateCurrentUserRequest()
        {

            var mockedWebClientBuilder = new Mock<IWebClientBuilder>();
            var mockedWebClient = new Mock<IWebClient>();
            mockedWebClientBuilder.Setup(x => x.Create()).Returns(mockedWebClient.Object);
	        mockedWebClient.Setup(x => x.Request(
			        new Uri("https://platform.shopyourway.com/products/search?token=1_2_tokenBeHere&hash=43b845c8a804093de9e87509ff6fe2c9e05f21593a3756e57ddb52db2055ba3c&q=batman"),
			        HttpMethod.Get, "{}"))
		        .Returns("{name:'batman'}");

            var client = new SywClient(mockedWebClientBuilder.Object, new OnlineTokenProvider("1_2_tokenBeHere", "secret"), new PlatformSettings());
            

	        var user = client.Get<User>("products/search", new { q = "batman" });

            Assert.Equal("batman",user.Name);
        }
		
	    [Fact]
        public void Post_Search_CreateSearchRequest()
        {

            var mockedWebClientBuilder = new Mock<IWebClientBuilder>();
            var mockedWebClient = new Mock<IWebClient>();
            mockedWebClientBuilder.Setup(x => x.Create()).Returns(mockedWebClient.Object);

	        mockedWebClient.Setup(x => x.Request(
			        new Uri("https://platform.shopyourway.com/users/create?token=1_2_tokenBeHere&hash=43b845c8a804093de9e87509ff6fe2c9e05f21593a3756e57ddb52db2055ba3c"),
			        HttpMethod.Post, "{\"name\":\"batman\"}"))
		        .Returns("{'name':'batman'}");

            var client = new SywClient(mockedWebClientBuilder.Object, new OnlineTokenProvider("1_2_tokenBeHere", "secret"), new PlatformSettings());
            var user = client.Post<User>("users/create", new { name = "batman" });

            Assert.Equal("batman",user.Name);
        }

	    [Fact]
        public void Get_NoParameters_CreateRequestWithoutParameters()
        {

            var mockedWebClientBuilder = new Mock<IWebClientBuilder>();
            var mockedWebClient = new Mock<IWebClient>();
            mockedWebClientBuilder.Setup(x => x.Create()).Returns(mockedWebClient.Object);

	        mockedWebClient.Setup(x => x.Request(
			        new Uri("https://platform.shopyourway.com/users/current?token=1_2_tokenBeHere&hash=43b845c8a804093de9e87509ff6fe2c9e05f21593a3756e57ddb52db2055ba3c"),
			        HttpMethod.Get, "{}"))
		        .Returns("{'name':'batman'}");

            var client = new SywClient(mockedWebClientBuilder.Object, new OnlineTokenProvider("1_2_tokenBeHere", "secret"), new PlatformSettings());
            var user = client.Get<User>("users/current");
            Assert.Equal("batman",user.Name);
        }
		
	    [Fact]
        public void DynamicGet_NoParameters_CreateRequestWithoutParameters()
        {

            var mockedWebClientBuilder = new Mock<IWebClientBuilder>();
            var mockedWebClient = new Mock<IWebClient>();
            mockedWebClientBuilder.Setup(x => x.Create()).Returns(mockedWebClient.Object);

	        mockedWebClient.Setup(x => x.Request(
			        new Uri("https://platform.shopyourway.com/Users/Current?token=1_2_tokenBeHere&hash=43b845c8a804093de9e87509ff6fe2c9e05f21593a3756e57ddb52db2055ba3c"),
			        HttpMethod.Get, "{}"))
		        .Returns("{'name':'batman'}");

            var client = new SywClient(mockedWebClientBuilder.Object, new OnlineTokenProvider("1_2_tokenBeHere", "secret"), new PlatformSettings());
            var user = client.GetDynamicApi().Users.Current.Get();
            Assert.Equal(user.name.ToString(),"batman");
        }
        
	    [Fact]
        public void DynamicGet_WithParameters_CreateRequestWithParameters()
        {
	        var mockedWebClientBuilder = new Mock<IWebClientBuilder>();
	        var mockedWebClient = new Mock<IWebClient>();
	        mockedWebClientBuilder.Setup(x => x.Create()).Returns(mockedWebClient.Object);

	        mockedWebClient.Setup(x => x.Request(
			        new Uri("https://platform.shopyourway.com/Users/Search?token=1_2_tokenBeHere&hash=43b845c8a804093de9e87509ff6fe2c9e05f21593a3756e57ddb52db2055ba3c&q=batman"),
			        HttpMethod.Get, "{}"))
		        .Returns("{'name':'batman'}");

	        var client = new SywClient(mockedWebClientBuilder.Object, new OnlineTokenProvider("1_2_tokenBeHere", "secret"), new PlatformSettings());
	        var user = client.GetDynamicApi().Users.Search.Get(q:"batman");
	        Assert.Equal(user.name.ToString(),"batman");
        }

	    [Fact]
	    public void Get_TokenInvalid_ThrowTokenException()
	    {

		    var mockedWebClientBuilder = new Mock<IWebClientBuilder>();
		    var mockedWebClient = new Mock<IWebClient>();
		    mockedWebClientBuilder.Setup(x => x.Create()).Returns(mockedWebClient.Object);
		    var response = new MockedWebResponse(){Response = "{'Error':{'StatusCode':401,'Message':'token is invalid'}}"};
		    var webException = new WebException("excpetion", null, WebExceptionStatus.Success, response);

		    mockedWebClient.Setup(x => x.Request(
				    new Uri(
					    "https://platform.shopyourway.com/users/current?token=1_2_tokenBeHere&hash=43b845c8a804093de9e87509ff6fe2c9e05f21593a3756e57ddb52db2055ba3c"),
				    HttpMethod.Get, It.IsAny<string>()))
			    .Throws(webException);

		    var client = new SywClient(mockedWebClientBuilder.Object, new OnlineTokenProvider("1_2_tokenBeHere", "secret"), new PlatformSettings());
		   Assert.Throws(typeof(InvalidTokenException),()=> client.Get<User>("users/current"));
	    }

	    [Fact]
	    public void Get_ErrorInApiResponse_ThrowApiException()
	    {

		    var mockedWebClientBuilder = new Mock<IWebClientBuilder>();
		    var mockedWebClient = new Mock<IWebClient>();
		    mockedWebClientBuilder.Setup(x => x.Create()).Returns(mockedWebClient.Object);
		    var response = new MockedWebResponse(){Response = "{'Error':{'Message':'wrong string'}}"};
		    var webException = new WebException("excpetion", null, WebExceptionStatus.Success, response);

		    mockedWebClient.Setup(x => x.Request(
				    new Uri(
					    "https://platform.shopyourway.com/users/current?token=1_2_tokenBeHere&hash=43b845c8a804093de9e87509ff6fe2c9e05f21593a3756e57ddb52db2055ba3c"),
				    HttpMethod.Get, It.IsAny<string>()))
			    .Throws(webException);

		    var client = new SywClient(mockedWebClientBuilder.Object, new OnlineTokenProvider("1_2_tokenBeHere", "secret"), new PlatformSettings());
		    Assert.Throws(typeof(RequestException),()=> client.Get<User>("users/current"));
	    }

	    [Fact]
	    public void Get_ConnectionError_ThrowException()
	    {
		    var mockedWebClientBuilder = new Mock<IWebClientBuilder>();
		    var mockedWebClient = new Mock<IWebClient>();
		    mockedWebClientBuilder.Setup(x => x.Create()).Returns(mockedWebClient.Object);
		    var response = new MockedWebResponse(){Response = ""};
		    var webException = new WebException("excpetion", null, WebExceptionStatus.ConnectFailure, response);

		    mockedWebClient.Setup(x => x.Request(
				    new Uri(
					    "https://platform.shopyourway.com/users/current?token=1_2_tokenBeHere&hash=43b845c8a804093de9e87509ff6fe2c9e05f21593a3756e57ddb52db2055ba3c"),
				    HttpMethod.Get, It.IsAny<string>()))
			    .Throws(webException);

		    var client = new SywClient(mockedWebClientBuilder.Object, new OnlineTokenProvider("1_2_tokenBeHere", "secret"), new PlatformSettings());
		    Assert.Throws(typeof(WebException),()=> client.Get<User>("users/current"));
	    }
    }


}