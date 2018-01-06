using Syw.Client.Common;

namespace Syw.Client
{
	public class OfflineTokenProvider: IPlatformTokenProvider
	{
		private readonly string _token;
		private readonly string _hash;

		public OfflineTokenProvider(string token, string hash)
		{
			_token = token;
			_hash = hash;
		}

		public string Get()
		{
			return _token;
		}

		public string GetHash()
		{
			return _hash;
		}
	}

	public class OnlineTokenProvider : IPlatformTokenProvider
	{
		private readonly string _token;
		private readonly string _appSecret;

		public OnlineTokenProvider(string token, string appSecret)
		{
			_token = token;
			_appSecret = appSecret;
		}

		public string Get()
		{
			return _token;
		}

		public string GetHash()
		{
			return new SignatureBuilder()
				.Append(_token)
				.Append(_appSecret)
				.Create();
		}
	}
}