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
			return MakeRequest<T>(GetServiceUrl(servicePath), parametersModel.GetParameters(), AddContextParameters, HttpMethod.Get);
		}

		public T Post<T>(string servicePath, object parametersModel = null)
		{
			return MakeRequest<T>(GetServiceUrl(servicePath), parametersModel.GetParameters(), AddContextParameters, HttpMethod.Post);
		}

		public dynamic Get(string servicePath, object parametersModel = null)
		{
			return MakeRequest(GetServiceUrl(servicePath), parametersModel.GetParameters(), AddContextParameters, HttpMethod.Get);
		}

		public dynamic Post(string servicePath, object parametersModel = null)
		{
			return MakeRequest(GetServiceUrl(servicePath), parametersModel.GetParameters(), AddContextParameters, HttpMethod.Post);
		}

		public T Post<T>(string servicePath, string parametersModel = null)
		{
			return Post<T>(GetServiceUrl(servicePath), parametersModel, AddContextParameters);
		}

		private T MakeRequest<T>(Uri serviceUrl, ICollection<KeyValuePair<string, object>> parameters, Action<NameValueCollection> applyExtraParameters, HttpMethod method)
		{
			var webClient = _webClientBuilder.Create();
			var serviceParameters = new NameValueCollection();
			var serviceParametersPost = new Dictionary<string, object>();

			applyExtraParameters?.Invoke(serviceParameters);
			if (parameters != null)
			{
				foreach (var parameter in parameters)
				{
					if (method == HttpMethod.Post)
						serviceParametersPost.Add(parameter.Key, parameter.Value);
					else
					{
						var value = _parametersTranslator.ToJson(parameter);
						serviceParameters.Add(parameter.Key, value);
					}
				}
			}
			try
			{
				return (method == HttpMethod.Post) ? JsonConvert.DeserializeObject<T>(webClient.UploadValues(ApplyExtraParametersToUrl(serviceUrl, serviceParameters), "POST", JsonConvert.SerializeObject(serviceParametersPost))) :
					webClient.GetJson<T>(serviceUrl, serviceParameters);
			}
			catch (WebException ex)
			{
				throw GeneratePlatformRequestException(ex);
			}
		}

		private dynamic MakeRequest(Uri serviceUrl, ICollection<KeyValuePair<string, object>> parameters, Action<NameValueCollection> applyExtraParameters, HttpMethod method)
		{
			var webClient = _webClientBuilder.Create();
			var serviceParameters = new NameValueCollection();
			var serviceParametersPost = new Dictionary<string, object>();

			applyExtraParameters?.Invoke(serviceParameters);
			if (parameters != null)
			{
				foreach (var parameter in parameters)
				{
					if (method == HttpMethod.Post)
						serviceParametersPost.Add(parameter.Key, parameter.Value);
					else
					{
						var value = _parametersTranslator.ToJson(parameter);
						serviceParameters.Add(parameter.Key, value);
					}
				}
			}
			try
			{
				return (method == HttpMethod.Post) ? JsonConvert.DeserializeObject(webClient.UploadValues(ApplyExtraParametersToUrl(serviceUrl, serviceParameters), "POST", JsonConvert.SerializeObject(serviceParametersPost))) :
					webClient.GetJson(serviceUrl, serviceParameters);
			}
			catch (WebException ex)
			{
				throw GeneratePlatformRequestException(ex);
			}
		}

		private T Post<T>(Uri serviceUrl, string parameters, Action<NameValueCollection> applyExtraParameters)
		{
			var webClient = _webClientBuilder.Create();
			var serviceParameters = new NameValueCollection();
			applyExtraParameters?.Invoke(serviceParameters);
			try
			{
				var response = webClient.UploadValues(ApplyExtraParametersToUrl(serviceUrl, serviceParameters), "POST", parameters);
				return JsonConvert.DeserializeObject<T>(response);
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

		private Uri ApplyExtraParametersToUrl(Uri serviceUrl, NameValueCollection serviceParameters)
		{
			var urlWithParameters = serviceUrl.ToString();
			if (serviceParameters == null) return new Uri(urlWithParameters);

			urlWithParameters += "?";
			foreach (var parameter in serviceParameters.Keys)
			{
				urlWithParameters = string.Format("{0}{1}={2}&", urlWithParameters, parameter, serviceParameters.Get(parameter.ToString()));
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
