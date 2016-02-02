using System;
using System.Drawing;

namespace LibLogitechLCD
{
	/// <summary>
	/// Represents a LogitechLCD keyboard.
	/// </summary>
	public class Keyboard
	{
		public static readonly Color TransparentColor = Color.White;
		public static readonly Color OpaqueColor = Color.Black;
		public static readonly Brush TransparentBrush = Brushes.White;
		public static readonly Brush OpaqueBrush = Brushes.Black;

		/// <summary>
		/// Constant representation of an invalid connection handle
		/// </summary>
		public const int InvalidDeviceHandle = -1;

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
		/// Opens a new keyboard from an existing API connection with the specified type.
		/// </summary>
		/// <<param name="connection">The connection to the API.</param>
		/// <returns>An initialized Keyboard</returns>
		public static Keyboard Open(Connection connection)
		{
			if (!API.IsInitialized)
			{
				throw new InitializationException("API not initialized");
			}
			if (connection == null || connection.ConnectionHandle == Connection.InvalidConnectionHandle)
			{
				throw new APIException("Connection invalid (likely already closed before)");
			}
			if (connection.Keyboard != null)
			{
				throw new APIException("Connection already has a keyboard associated");
			}

			Keyboard keyboard = Keyboard.Open(connection, Type.QVGA);
			if (keyboard == null)
			{
				keyboard = Keyboard.Open(connection, Type.BlackWhite);
				if (keyboard == null)
				{
					throw new APIException("API did not return a valid keyboard");
				}
			}

			return keyboard;
		}

		/// <summary>
		/// Opens a new keyboard from an existing API connection and attempts to autodetect the keyboard type.
		/// </summary>
		/// <param name="connection">The connection to the API.</param>
		/// <param name="keyboardType">The type of the keyboard to open</param>
		/// <returns>An initialized Keyboard</returns>
		private static Keyboard Open(Connection connection, Keyboard.Type keyboardType)
		{
			if (!API.IsInitialized)
			{
				throw new InitializationException("API not initialized");
			}
			if (connection == null || connection.ConnectionHandle == Connection.InvalidConnectionHandle)
			{
				throw new APIException("Connection invalid (likely already closed before)");
			}
			if (connection.Keyboard != null)
			{
				throw new APIException("Connection already has a keyboard associated");
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
					throw new APIException("Invalid device type");
			}

			//Protip: Catch this error, since you might not be sure which keyboard your user has.
			if (tmp == Keyboard.InvalidDeviceHandle)
			{
				return null;
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
				throw new InitializationException("API not initialized");
			}
			if (this.Connection == null || this.Connection.ConnectionHandle == Connection.InvalidConnectionHandle)
			{
				throw new APIException("Connection invalid (likely already closed before)");
			}
			if (this.Connection.Keyboard == null) //<-- Should actually point to "this". Just a safety measure in case of screwd up objects
			{
				throw new APIException("Connection has no keyboard associated");
			}

			uint result = DMcLgLCD.LcdClose(this.DeviceHandle);
			if (result != API.SUCCESS)
			{
				throw new APIException("API returned error on disconnect. See error code for details", result);
			}

			//Make the connection forget about our keyboard
			this.Connection.Keyboard = null;
			
			//Make the keyboard forget about the connection
			this.Connection = null;

			//Invalidate device handle
			this.DeviceHandle = Keyboard.InvalidDeviceHandle;
		}

		public void Draw(Bitmap bitmap)
		{
			if (!API.IsInitialized)
			{
				throw new InitializationException("API not initialized");
			}
			if (this.Connection == null || this.Connection.ConnectionHandle == Connection.InvalidConnectionHandle)
			{
				throw new APIException("Connection invalid (likely already closed before)");
			}
			if (this.DeviceHandle == Keyboard.InvalidDeviceHandle)
			{
				throw new APIException("Keyboard has invalid device handle");
			}
			if (bitmap == null)
			{
				throw new ArgumentNullException("bitmap", "Bitmap is null");
			}
			
			uint result = DMcLgLCD.LcdUpdateBitmap(this.DeviceHandle, bitmap.GetHbitmap(), (int)this.KeyboardType);
			if (result != API.SUCCESS)
			{
				throw new APIException("Error drawing on screen", result);
			}
		}

		public void BringToFront()
		{
			if (!API.IsInitialized)
			{
				throw new InvalidOperationException("API not initialized");
			}
			if (this.Connection == null || this.Connection.ConnectionHandle == Connection.InvalidConnectionHandle)
			{
				throw new InvalidOperationException("Connection invalid (likely already closed before)");
			}
			if (this.Connection.Keyboard == null)
			{
				throw new InvalidOperationException("Connection has no keyboard associated");
			}

			uint result = DMcLgLCD.LcdSetAsLCDForegroundApp(this.DeviceHandle, DMcLgLCD.LGLCD_FORE_YES);
			if (result != API.SUCCESS)
			{
				throw new APIException("Error bringing applet to foreground", result);
			}
		}

		public Buttons ReadButtons()
		{
			if (!API.IsInitialized)
			{
				throw new InvalidOperationException("API not initialized");
			}
			if (this.Connection == null || this.Connection.ConnectionHandle == Connection.InvalidConnectionHandle)
			{
				throw new InvalidOperationException("Connection invalid (likely already closed before)");
			}
			if (this.Connection.Keyboard == null)
			{
				throw new InvalidOperationException("Connection has no keyboard associated");
			}

			return (Buttons)DMcLgLCD.LcdReadSoftButtons(this.DeviceHandle);
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

		[Flags]
		public enum Buttons : uint
		{
			None = 0,

			//Default G510 keys
			Button1			= 0x0001,
			Button2			= 0x0002,
			Button3			= 0x0004,
			Button4			= 0x0008,

			//Extended G19 Keys
			//Only work when Connection.Connect() was called with the extended parameter
			Left	= 0x0100,
			Right	= 0x0200,
			OK		= 0x0400,
			Cancel	= 0x0800,
			Up		= 0x1000,
			Down	= 0x2000,
			Menu	= 0x4000
		}
	}
}
