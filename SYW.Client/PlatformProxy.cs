using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Text;
using Newtonsoft.Json;
using Syw.Client.Common;
using Syw.Client.Common.Exceptions;
using Syw.Client.WebClient;

namespace Syw.Client
{
	internal class PlatformProxy
	{
		private readonly IWebClientBuilder _webClientBuilder;
		private readonly IParametersTranslator _parametersTranslator;
		private readonly PlatformSettings _platformSettings;
		private readonly IPlatformTokenProvider _platformTokenProvider;

		public PlatformProxy(IWebClientBuilder webClientBuilder, IParametersTranslator parametersTranslator, PlatformSettings platformSettings, IPlatformTokenProvider platformTokenProvider)
		{
			_webClientBuilder = webClientBuilder;
			_parametersTranslator = parametersTranslator;
			_platformSettings = platformSettings;
			_platformTokenProvider = platformTokenProvider;
		}

		public T Get<T>(string servicePath, object parametersModel = null)
		{
			return JsonConvert.DeserializeObject<T>(MakeRequest(GetServiceUrl(servicePath), parametersModel.GetParameters(), AddContextParameters, HttpMethod.Get));
		}

		public T Post<T>(string servicePath, object parametersModel = null)
		{
			return JsonConvert.DeserializeObject<T>(MakeRequest(GetServiceUrl(servicePath), parametersModel.GetParameters(), AddContextParameters, HttpMethod.Post));
		}

		public dynamic Get(string servicePath, object parametersModel = null)
		{
			return JsonConvert.DeserializeObject(MakeRequest(GetServiceUrl(servicePath), parametersModel.GetParameters(), AddContextParameters, HttpMethod.Get));
		}

		public dynamic Post(string servicePath, object parametersModel = null)
		{
			return JsonConvert.DeserializeObject(MakeRequest(GetServiceUrl(servicePath), parametersModel.GetParameters(), AddContextParameters, HttpMethod.Post));
		}

		private string MakeRequest(Uri serviceUrl, ICollection<KeyValuePair<string, object>> parameters, Action<NameValueCollection> applyExtraParameters, HttpMethod method)
		{
			var webClient = _webClientBuilder.Create();
			var queryParameters = new NameValueCollection();
			var bodyParameters = new Dictionary<string, object>();
			
			applyExtraParameters?.Invoke(queryParameters);
			if (parameters != null)
			{
				foreach (var parameter in parameters)
				{
					if (method != HttpMethod.Get)
						bodyParameters.Add(parameter.Key, parameter.Value);
					else
					{
						var value = _parametersTranslator.ToJson(parameter);
						queryParameters.Add(parameter.Key, value);
					}
				}
			}
			try
			{
				return webClient.Request(ApplyQueryParametersToUrl(serviceUrl, queryParameters), method,
					JsonConvert.SerializeObject(bodyParameters));
			}
			catch (WebException ex)
			{
				throw GeneratePlatformRequestException(ex);
			}
		}

	
		private Exception GeneratePlatformRequestException(WebException ex)
		{
			try
			{
				var readError = ReadError(ex);

				var errorDto = JsonConvert.DeserializeObject<RequestExceptionDto>(readError).Error;
				if (errorDto.StatusCode == 401)
					return new InvalidTokenException(ex);

				return new RequestException(errorDto.StatusCode, errorDto.Message, errorDto.RequestId, ex);
			}
			catch (Exception)
			{
				return ex;
			}
		}

		private string ReadError(WebException ex)
		{
			using (var reader = new StreamReader(ex.Response.GetResponseStream(), Encoding.UTF8))
			{
				return reader.ReadToEnd();
			}
		}

		private void AddContextParameters(NameValueCollection serviceParameters)
		{
			serviceParameters.Add("token", _platformTokenProvider.Get());
			serviceParameters.Add("hash", _platformTokenProvider.GetHash());
		}

		private Uri ApplyQueryParametersToUrl(Uri serviceUrl, NameValueCollection serviceParameters)
		{
			var urlWithParameters = serviceUrl.ToString();
			if (serviceParameters == null) return new Uri(urlWithParameters);
			urlWithParameters += "?";
			foreach (var parameter in serviceParameters.Keys)
			{
				urlWithParameters = $"{urlWithParameters}{parameter}={serviceParameters.Get(parameter.ToString())}&";
			}
			urlWithParameters = urlWithParameters.Remove(urlWithParameters.Length - 1);
			return new Uri(urlWithParameters);
		}

		private Uri GetServiceUrl(string servicePath)
		{
			return new Uri(_platformSettings.ApiUrl, servicePath);
		}
	}
}
