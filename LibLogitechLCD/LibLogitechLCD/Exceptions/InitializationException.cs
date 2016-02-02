using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LibLogitechLCD
{
	[Serializable]
	public class InitializationException : APIException
	{
		public InitializationException() { }
		public InitializationException(string message) : base(message, 0) { }
	}
}
