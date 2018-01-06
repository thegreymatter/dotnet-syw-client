using System;
using System.Collections.Generic;
using System.Text;

namespace Syw.Client.Common.Exceptions
{
	public class InvalidTokenException : Exception
	{
		public InvalidTokenException(Exception inner) :
			base("token has expired or is invalid", inner)
		{ }
	}

	public class RequestException : Exception
	{
		public int StatusCode { get; private set; }
		public string RequestId { get; private set; }

		public RequestException(int statusCode, string message, string requestId, Exception inner) : base(message, inner)
		{
			StatusCode = statusCode;
			RequestId = requestId;
		}
	}

	internal class RequestExceptionDto
	{
		public RequestErrorDto Error { get; set; }
	}

	internal class RequestErrorDto
	{
		public int StatusCode { get; set; }
		public string Message { get; set; }
		public string RequestId { get; set; }
	}

}
