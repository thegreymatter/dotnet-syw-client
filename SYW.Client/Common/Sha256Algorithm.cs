using System.Linq;
using System.Security.Cryptography;
using System.Text;

namespace Syw.Client.Common
{
	internal class Sha256Algorithm : IHashAlgorithm
	{
		public string Compute(byte[] bytes)
		{
			var algorithm = SHA256.Create();
			return ToHexString(algorithm.ComputeHash(bytes)).ToLower();
		}

		public static string ToHexString(byte[] bytes)
		{
			return bytes.Aggregate(new StringBuilder(bytes.Length * 2), (sb, i) => sb.Append(i.ToString("x2"))).ToString();
		}
	}
}