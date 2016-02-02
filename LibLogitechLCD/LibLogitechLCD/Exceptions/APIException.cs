using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LibLogitechLCD
{
	public class APIException : Exception
	{
		public uint ErrorCode { get; private set; }

		public APIException() : this(0) { }

		public APIException(string message) : base(message) { }

		public APIException(uint errorCode)
		{
			this.ErrorCode = errorCode;
		}
		public APIException(string message, uint errorCode) : base(message)
		{
			this.ErrorCode = errorCode;
		}
	}
}
