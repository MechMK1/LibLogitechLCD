using System;
using System.Drawing;

namespace LibLogitechLCD
{
	/// <summary>
	/// Represents a LogitechLCD keyboard.
	/// </summary>
	public class Keyboard
	{
		/// <summary>
		/// Size in pixels of a Black/White display
		/// </summary>
		public static readonly Size BlackWhiteDisplaySize = new Size(160, 43);

		/// <summary>
		/// Size in pixels of a QVGA display
		/// </summary>
		public static readonly Size QVGADisplaySize = new Size(320, 240);

		/// <summary>
		/// Gets the internal device handle of the keyboard.
		/// </summary>
		public int DeviceHandle { get; private set; }

		/// <summary>
		/// Gets a reference to the underlying connection which opened this Keyboard.
		/// </summary>
		public Connection Connection { get; private set; }

		/// <summary>
		/// The type of the keyboard.
		/// G510 Keyboards are black/white. G19 Keyboards are QVGA.
		/// </summary>
		public Type KeyboardType { get; private set; }

		/// <summary>
		/// Opens a new keyboard from an existing API connection.
		/// </summary>
		/// <param name="connection">The connection to the API.</param>
		/// <param name="keyboardType">The type of the keyboard to open</param>
		/// <returns>An initialized Keyboard</returns>
		public static Keyboard Open(Connection connection, Keyboard.Type keyboardType)
		{
			if (!API.IsInitialized)
			{
				throw new InvalidOperationException("API not initialized");
			}
			if (connection == null || !connection.IsConnected)
			{
				throw new InvalidOperationException("Connection invalid (likely already closed before)");
			}
			if (connection.Keyboard != null)
			{
				throw new InvalidOperationException("Connection already has a keyboard associated");
			}

			int tmp;

			//Switch on keyboard types, since it might be possible to sneak invalid values in via casts.
			switch (keyboardType)
			{
				case Type.BlackWhite:
				case Type.QVGA:
					tmp = DMcLgLCD.LcdOpenByType(connection.ConnectionHandle, (int)keyboardType);
					break;
				default:
					throw new InvalidOperationException("Invalid device type");
			}

			//Protip: Catch this error, since you might not be sure which keyboard your user has.
			if (tmp < 0)
			{
				throw new InvalidOperationException("API did not return a valid keyboard");
			}

			Keyboard k = new Keyboard() { DeviceHandle = tmp, Connection = connection, KeyboardType = keyboardType };

			connection.Keyboard = k;
			return k;
		}

		/// <summary>
		/// Close the connection to the keyboard.
		/// This must be called when the applet exits.
		/// </summary>
		public void Close()
		{
			if (!API.IsInitialized)
			{
				throw new InvalidOperationException("API not initialized");
			}
			if (this.Connection == null || !this.Connection.IsConnected)
			{
				throw new InvalidOperationException("Connection invalid (likely already closed before)");
			}
			if (this.Connection.Keyboard == null)
			{
				throw new InvalidOperationException("Connection has no keyboard associated");
			}

			uint result = DMcLgLCD.LcdClose(this.DeviceHandle);
			if (result != DMcLgLCD.ERROR_SUCCESS)
			{
				throw new InvalidOperationException("API returned error on disconnect. Error: " + result);
			}

			this.Connection.Keyboard = null;
			this.Connection = null;
			this.DeviceHandle = 0;
		}

		public void Draw(Bitmap bitmap)
		{
			if (!API.IsInitialized)
			{
				throw new InvalidOperationException("API not initialized");
			}
			if (this.Connection == null || !this.Connection.IsConnected)
			{
				throw new InvalidOperationException("Connection invalid (likely already closed before)");
			}
			if (this.Connection.Keyboard == null)
			{
				throw new InvalidOperationException("Connection has no keyboard associated");
			}
			if (bitmap == null)
			{
				throw new ArgumentNullException("bitmap", "Bitmap is null");
			}
			
			DMcLgLCD.LcdUpdateBitmap(this.DeviceHandle, bitmap.GetHbitmap(), (int)this.KeyboardType);
		}

		/// <summary>
		/// Keyboard types.
		/// Black/White is for G510 Keyboards
		/// QVGA is for G19 Keyboards
		/// </summary>
		public enum Type : int
		{
			BlackWhite = 1,
			QVGA = 2
		}
	}
}
