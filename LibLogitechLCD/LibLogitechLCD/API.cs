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
		/// Constant which represent successful calls to the base API
		/// </summary>
		internal const uint SUCCESS = 0;

		/// <summary>
		/// Gets a value whether the API is initialized or not
		/// </summary>
		public static bool IsInitialized { get; private set; }

		/// <summary>
		/// Gets a value whether the API is ready to be deinitialized.
		/// In order to be deinitialized, the API must be initialized and have no connections.
		/// </summary>
		public static bool CanBeDeInitialized
		{
			get {
				return (API.IsInitialized && API.Connections.Count == 0);
			}
		}

		/// <summary>
		/// List of current connections to the API
		/// </summary>
		internal static List<Connection> Connections { get; private set; }

		/// <summary>
		/// Static constructor
		/// </summary>
		static API()
		{
			Connections = new List<Connection>();
		}
	
		/// <summary>
		/// Initialize the API.
		/// This must be called exactly once per Application and must be called before all other API calls.
		/// </summary>
		public static void Initialize()
		{
			if (API.IsInitialized)
			{
				throw new InitializationException("API already initialized");
			}

			uint result;
			result = DMcLgLCD.LcdInit();

			//0 indicates a successful initialization. Any non-zero return value indicates an error
			if (result == API.SUCCESS)
			{
				API.IsInitialized = true;
			}
			else
			{
				throw new APIException("API Initialize return error. See error code for details", result);
			}
		}

		/// <summary>
		/// DeInitialize the API.
		/// This must be called exactly once per Application and must be called after all other API calls-
		/// </summary>
		public static void DeInitialize()
		{
			//Throw an Exception if the user attempts to de-initialize the API when it's not initialized
			if (!API.IsInitialized)
			{
				throw new InitializationException("API not initialized");
			}

			//Throw an Exception if the user attempts to de-initialize the API while there are active connections
			if (API.Connections.Count > 0)
			{
				throw new InitializationException("Cannot deinitialize while there are active connections");
			}

			uint result;
			result = DMcLgLCD.LcdDeInit();

			//0 indicates a successful initialization. Any non-zero return value indicates an error
			if (result == API.SUCCESS)
			{
				API.IsInitialized = false;
			}
			else
			{
				throw new APIException("API DeInitialize return error. See error code for details", result);
			}
		}

		/// <summary>
		/// Creates a copy of all open connections to the API. Removing a connection from this IEnumerable does not affect the API in any way.
		/// </summary>
		/// <returns>A copy of all open connections</returns>
		public static IEnumerable<Connection> GetOpenConnections()
		{
			return API.Connections.ToArray();
		}

		/// <summary>
		/// Returns the number of open connections.
		/// </summary>
		/// <returns>Number of open connections</returns>
		public static int GetOpenConnectionsCount()
		{
			return API.Connections.Count;
		}
	}
}
