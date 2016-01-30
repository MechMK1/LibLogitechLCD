using System;
using System.Collections.Generic;

namespace LibLogitechLCD
{
	/// <summary>
	/// API statically represents the Logitech Keyboard API.
	/// </summary>
	public static class API
	{
		/// <summary>
		/// Gets a value whether the API is initialized or not
		/// </summary>
		public static bool IsInitialized { get; private set; }

		/// <summary>
		/// List of current connections to the API
		/// </summary>
		internal static List<Connection> connections = new List<Connection>();

		/// <summary>
		/// Initialize the API.
		/// This must be called exactly once per Application and must be called before all other API calls.
		/// </summary>
		public static void Initialize()
		{
			if (API.IsInitialized)
			{
				throw new InvalidOperationException("API already initialized");
			}

			uint result;
			result = DMcLgLCD.LcdInit();

			if (result == 0)
			{
				API.IsInitialized = true;
			}
			else
			{
				throw new Exception("API Initialize return error code: " + result);
			}
		}

		/// <summary>
		/// DeInitialize the API.
		/// This must be called exactly once per Application and must be called after all other API calls-
		/// </summary>
		public static void DeInitialize()
		{
			if (!API.IsInitialized)
			{
				throw new InvalidOperationException("API not initialized");
			}
			if (API.connections.Count > 0)
			{
				throw new InvalidOperationException("Cannot deinitialize while there are active connections");
			}

			uint result;
			result = DMcLgLCD.LcdDeInit();

			if (result == 0)
			{
				API.IsInitialized = false;
			}
			else
			{
				throw new Exception("API DeInitialize return error code: " + result);
			}
		}
	}
}
