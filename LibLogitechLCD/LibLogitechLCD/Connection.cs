using System;

namespace LibLogitechLCD
{
	/// <summary>
	/// Represents a connection to the API
	/// </summary>
	public class Connection
	{
		/// <summary>
		/// Constant representation of an invalid connection handle
		/// </summary>
		public const int InvalidConnectionHandle = -1;

		/// <summary>
		/// The internal handle of the connection.
		/// </summary>
		public int ConnectionHandle { get; private set; }

		/// <summary>
		/// Checks how many keyboards are open via this connection.
		/// Only one keyboard per connection is allowed.
		/// </summary>
		public Keyboard Keyboard { get; internal set; }

		/// <summary>
		/// Private constructor.
		/// Create new connections via Connection.Connect()
		/// </summary>
		private Connection() { }

		/// <summary>
		/// Create a new connection to the API
		/// </summary>
		/// <param name="appFriendlyName">The display name of the Applet</param>
		/// <param name="extended">Determines whether or not extended keys of the G19-Keyboard should be used.</param>
		/// <returns>The new connection to the API</returns>
		public static Connection Connect(string appFriendlyName, bool extended = false)
		{
			if (!API.IsInitialized)
			{
				throw new InitializationException("API not initialized");
			}

			int handle;

			// Get the internal handle
			//If the extended flag is set, 
			handle = extended
				//get the extended connection,
				? DMcLgLCD.LcdConnectEx(appFriendlyName, 0, 0)
				//otherwise get the regular one
				: DMcLgLCD.LcdConnect(appFriendlyName, 0, 0);

			if (handle == Connection.InvalidConnectionHandle)
			{
				throw new APIException("Could not create connection to API");
			}


			Connection con = new Connection() { ConnectionHandle = handle };
			API.Connections.Add(con);
			return con;
		}


		/// <summary>
		/// Close this connection and invalidate it.
		/// </summary>
		/// <returns></returns>
		public void Disconnect()
		{
			if (!API.IsInitialized)
			{
				throw new InitializationException("API not initialized");
			}
			if (this.ConnectionHandle == Connection.InvalidConnectionHandle)
			{
				throw new APIException("Connection invalid (likely already closed before)");
			}
			if (this.Keyboard != null)
			{
				throw new APIException("Attempting to close connection, but keyboards are still open");
			}

			uint result = DMcLgLCD.LcdDisconnect(this.ConnectionHandle);
			if (result == API.SUCCESS)
			{
				//Invalidate connection handle
				this.ConnectionHandle = Connection.InvalidConnectionHandle;

				//Remove the connection from the API connection pool
				API.Connections.Remove(this);
			}
			else
			{
				throw new APIException("Error occurred while disconnecting from API. See error code for details", result);
			}
		}
	}
}
