using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Dynamic;
using System.Linq;
using System.Net.Http;
using Syw.Client.Common;
using Syw.Client.WebClient;

namespace Syw.Client
{
	public class SywClient
	{
		private PlatformProxy _proxy;


		public SywClient(IPlatformTokenProvider tokenProvider) : this(new WebClientBuilder(), tokenProvider, new PlatformSettings())
		{
		}

		public SywClient(IWebClientBuilder webClient, IPlatformTokenProvider tokenProvider, PlatformSettings settings)
		{
			_proxy = new PlatformProxy(webClient, new ParametersTranslator(), settings, tokenProvider);
		}


		public T Get<T>(string endpoint, object parametersModel = null)
		{
			return _proxy.Get<T>(endpoint, parametersModel);
		}

		public T Post<T>(string endpoint, object parametersModel = null)
		{
			return _proxy.Post<T>(endpoint, parametersModel);
		}

		public dynamic GetDynamicApi()
		{
			return new Api(_proxy);
		}


		public class Api : DynamicObject
		{
			private readonly PlatformProxy _proxy;
			private string url = "";

			internal Api(PlatformProxy proxy)
			{
				_proxy = proxy;
			}

			public override bool TryGetMember(GetMemberBinder binder, out object result)
			{
				url += "/" + binder.Name;
				result = this;
				return true;
			}

			public override bool TryInvokeMember(InvokeMemberBinder binder, object[] args, out object result)
			{
				var parameters = new Dictionary<string, object>();
				for (var index = 0; index < binder.CallInfo.ArgumentCount; index++)
				{
					parameters.Add(binder.CallInfo.ArgumentNames[index], args[index]);
				}

				if (binder.Name.Equals("Get"))
				{
					result = _proxy.Get(url, parameters);
					return true;
				}
				if (binder.Name.Equals("Post"))
				{
					result = _proxy.Get(url, parameters);
					return true;
				}
				result = new object();
				return false;
			}
		}



	}
}