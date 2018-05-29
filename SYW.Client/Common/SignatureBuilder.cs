using System.Collections.Generic;

namespace Syw.Client.Common
{
	public class SignatureBuilder
	{
		private readonly IHashAlgorithm _hashAlgorithm;
		private readonly List<byte> _input = new List<byte>();

		public SignatureBuilder()
		{
			_hashAlgorithm = new Sha256Algorithm();
		}

		public SignatureBuilder Append(IList<byte> bytes)
		{
			_input.AddRange(bytes);
			return this;
		}

		public string Create()
		{
			return _hashAlgorithm.Compute(_input.ToArray());
		}
	}
}
