using System;
using System.Text;

namespace Syw.Client.Common
{
	public static class SignatureBuilderExtensions
	{
		public static SignatureBuilder Append(this SignatureBuilder builder, long val)
		{
			var bytes = BitConverter.GetBytes(val);
			return builder.Append(bytes);
		}

		public static SignatureBuilder Append(this SignatureBuilder builder, string val)
		{
			var bytes = Encoding.UTF8.GetBytes(val);
			return builder.Append(bytes);
		}

		public static SignatureBuilder Append(this SignatureBuilder builder, Guid value)
		{
			return builder.Append(value.ToByteArray());
		}
	}
}