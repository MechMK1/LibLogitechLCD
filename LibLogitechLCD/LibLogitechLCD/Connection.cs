using System;

namespace LibLogitechLCD
{
	/// <summary>
	/// Represents a connection to the API
	/// </summary>
	public class Connection
	{
		/// <summary>
		/// The internal handle of the connection.
		/// </summary>
		public int ConnectionHandle { get; private set; }
		/// <summary>
		/// Determines whether or not the connection is still valid
		/// </summary>
		public bool IsConnected { get; private set; }

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
				throw new InvalidOperationException("API not initialized");
			}

			int handle;

			// Get the internal handle
			//If the extended flag is set, 
			handle = extended
				//get the extended connection,
				? DMcLgLCD.LcdConnectEx(appFriendlyName, 0, 0)
				//otherwise get the regular one
				: DMcLgLCD.LcdConnect(appFriendlyName, 0, 0);
			
			Connection con = new Connection() { ConnectionHandle = handle, IsConnected = true, Keyboard = null };
			API.connections.Add(con);
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
				throw new InvalidOperationException("API not initialized");
			}
			if (!this.IsConnected)
			{
				throw new InvalidOperationException("Connection invalid (likely already closed before)");
			}
			if (this.Keyboard != null)
			{
				throw new InvalidOperationException("Attempting to close connection, but keyboards are still open");
			}

			uint result = DMcLgLCD.LcdDisconnect(this.ConnectionHandle);
			if (result == DMcLgLCD.ERROR_SUCCESS)
			{
				this.IsConnected = false;
				API.connections.Remove(this);
			}
			else
			{
				throw new Exception("Error disconnecting from API. Error: " + result);
			}
		}
	}
}
