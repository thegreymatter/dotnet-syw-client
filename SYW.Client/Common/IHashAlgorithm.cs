namespace Syw.Client.Common
{
	internal interface IHashAlgorithm
	{
		string Compute(byte[] bytes);
	}
}