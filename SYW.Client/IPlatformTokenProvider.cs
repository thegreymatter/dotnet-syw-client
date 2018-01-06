namespace Syw.Client
{
	public interface IPlatformTokenProvider
	{
		string Get();

		string GetHash();
	}
}