using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Diagnostics;
using System.Drawing;
using System.Linq;

namespace LibLogitechLCD.Test
{
	[TestClass]
	public class UnitTest
	{
		/// <summary>
		/// Initialize the API and deinitialize it
		/// </summary>
		[TestMethod]
		public void TestAPIInitDeInit()
		{
			//Ensure the API is not initialized when we begin
			Assert.IsFalse(API.IsInitialized);

			//Initialize the API
			API.Initialize();

			//Ensure the API is initialized afterwards, e.g. initialization was successful
			Assert.IsTrue(API.IsInitialized);

			//Deinitialize the API
			API.DeInitialize();

			//Ensure the API is deinitialized afterwards, e.h. deinitialization was successful
			Assert.IsFalse(API.IsInitialized);
		}

		/// <summary>
		/// Initialize the API twice.
		/// 
		/// MUST throw an Exception.
		/// </summary>
		[TestMethod]
		[ExpectedException(typeof(InitializationException))]
		public void TestAPIDoubleInit()
		{
			//Ensure the API is not initialized when we begin
			Assert.IsFalse(API.IsInitialized);

			//Initialize the API
			API.Initialize();

			//Ensure the API is initialized afterwards, e.g. initialization was successful
			Assert.IsTrue(API.IsInitialized);

			//Initialize the API again.
			//This MUST throw an exception
			API.Initialize();

			//This must never be called. Unit test fails if so.
			Assert.Fail("API has been initialized twice");
		}

		/// <summary>
		/// Deinitialize the API before initializing it.
		/// 
		/// MUST throw an exception
		/// </summary>
		[TestMethod]
		[ExpectedException(typeof(InitializationException))]
		public void TestAPIDeInitBeforeInit()
		{
			//Ensure the API is not initialized when we begin
			Assert.IsFalse(API.IsInitialized);
			
			//Deinitialize the API
			//This MUST throw an exeption
			API.DeInitialize();

			//This must never be called. Unit test fails if so.
			Assert.Fail("API was deinitialized before initialization");
		}

		/// <summary>
		/// Create a connection and close it afterwards.
		/// </summary>
		[TestMethod]
		public void TestAPIGetConnection()
		{
			//Ensure the API is not initialized when we begin
			Assert.IsFalse(API.IsInitialized);

			//Initialize the API
			API.Initialize();

			//Ensure the API is initialized afterwards, e.g. initialization was successful
			Assert.IsTrue(API.IsInitialized);

			//Create a new connection to the API
			Connection connection = Connection.Connect("Unit Test");
			
			//Ensure the connection was successful and the handle is valid
			Assert.AreNotEqual(Connection.InvalidConnectionHandle, connection.ConnectionHandle);

			//Write the handle to the debug log
			Debug.WriteLine("Handler: " + connection.ConnectionHandle);

			//Ensure that the API has exactly one connection - which we just created before
			Assert.AreEqual<int>(1, API.Connections.Count);

			//Disconnect from the API
			connection.Disconnect();

			//Ensure that the disconnect was successful and that the connection knows it's not connected anymore.
			Assert.AreEqual(Connection.InvalidConnectionHandle, connection.ConnectionHandle);

			//Ensure that the connection has been removed from the API's connection pool
			Assert.AreEqual<int>(0, API.Connections.Count);

			//Deinitialize the API after every connection has been removed
			API.DeInitialize();

			//Ensure the deinitialization was successful
			Assert.IsFalse(API.IsInitialized);
		}

		/// <summary>
		/// Connect before the API is initialized
		/// 
		/// MUST throw an Exeption
		/// </summary>
		[TestMethod]
		[ExpectedException(typeof(InitializationException))]
		public void TestAPIConnectionBeforeInit()
		{
			//Ensure the API is not initialized when we begin
			Assert.IsFalse(API.IsInitialized);
			
			//Create a connection without initialization
			//This MUST throw an exception
			Connection connection = Connection.Connect("Unit Test - Fail");

			//This must never be called. Unit test fails if so.
			Assert.Fail("Connection created without initialization");
		}

		/// <summary>
		/// Attempt to deinitialize the API with a pending connection.
		/// </summary>
		[TestMethod]
		public void TestAPIDeInitWithPendingConnection()
		{
			//Ensure the API is not initialized when we begin
			Assert.IsFalse(API.IsInitialized);

			//Initialize the API
			API.Initialize();

			//Ensure the API is initialized afterwards, e.g. initialization was successful
			Assert.IsTrue(API.IsInitialized);

			//Create a new connection to the API
			Connection connection = Connection.Connect("Unit Test");
			
			//Ensure the connection was successful and the handle is valid
			Assert.AreNotEqual(Connection.InvalidConnectionHandle, connection.ConnectionHandle);

			//Write the handle to the debug log
			Debug.WriteLine("Handler: " + connection.ConnectionHandle);

			//Ensure that the API has exactly one connection - which we just created before
			Assert.AreEqual<int>(1, API.Connections.Count);

			try
			{
				//Deinitialize the API after every connection has been removed
				API.DeInitialize();

				//This must never be called. Unit test fails if so.
				Assert.Fail("API deinitialized with pending connection");
			}
			catch (InitializationException)
			{
				//Disconnect from the API
				connection.Disconnect();

				//Ensure that the disconnect was successful and that the connection knows it's not connected anymore.
				Assert.AreEqual(Connection.InvalidConnectionHandle, connection.ConnectionHandle);

				//Ensure that the connection has been removed from the API's connection pool
				Assert.AreEqual<int>(0, API.Connections.Count);

				//Deinitialize the API after every connection has been removed
				API.DeInitialize();

				//Ensure the deinitialization was successful
				Assert.IsFalse(API.IsInitialized);
			}
		}

		/// <summary>
		/// Open a keyboard, then close it
		/// </summary>
		[TestMethod]
		public void TestAPIOpenKeyboard()
		{
			//Ensure the API is not initialized when we begin
			Assert.IsFalse(API.IsInitialized);

			//Initialize the API
			API.Initialize();

			//Ensure the API is initialized afterwards, e.g. initialization was successful
			Assert.IsTrue(API.IsInitialized);

			//Create a new connection to the API
			Connection connection = Connection.Connect("Unit Test");

			//Ensure the connection was successful and the handle is valid
			Assert.AreNotEqual(Connection.InvalidConnectionHandle, connection.ConnectionHandle);

			//Write the handle to the debug log
			Debug.WriteLine("Handler: " + connection.ConnectionHandle);

			//Ensure that the API has exactly one connection - which we just created before
			Assert.AreEqual<int>(1, API.Connections.Count);

			try
			{
				//Open a new Black/White keyboard
				Keyboard keyboard = Keyboard.Open(connection);
				Debug.WriteLine("Keyboard: " + keyboard.DeviceHandle);

				//Disconnect afterwards
				keyboard.Close();
			}
			catch (APIException)
			{
				Assert.Inconclusive("Opening a keyboard failed. This might be due to the user not having a G510 connected.");
			}


			//Disconnect from the API
			connection.Disconnect();

			//Ensure that the disconnect was successful and that the connection knows it's not connected anymore.
			Assert.AreEqual(Connection.InvalidConnectionHandle, connection.ConnectionHandle);
			Assert.IsNull(connection.Keyboard);

			//Ensure that the connection has been removed from the API's connection pool
			Assert.AreEqual<int>(0, API.Connections.Count);

			//Deinitialize the API after every connection has been removed
			API.DeInitialize();

			//Ensure the deinitialization was successful
			Assert.IsFalse(API.IsInitialized);
		}

		/// <summary>
		/// Open a Keyboard and draw on it
		/// </summary>
		[TestMethod]
		public void TestAPIKeyboardDraw()
		{

			//Ensure the API is not initialized when we begin
			Assert.IsFalse(API.IsInitialized);

			//Initialize the API
			API.Initialize();

			//Ensure the API is initialized afterwards, e.g. initialization was successful
			Assert.IsTrue(API.IsInitialized);

			//Create a new connection to the API
			Connection connection = Connection.Connect("Unit Test");

			//Ensure the connection was successful and the handle is valid
			Assert.AreNotEqual(Connection.InvalidConnectionHandle, connection.ConnectionHandle);

			//Write the handle to the debug log
			Debug.WriteLine("Handler: " + connection.ConnectionHandle);

			//Ensure that the API has exactly one connection - which we just created before
			Assert.AreEqual<int>(1, API.Connections.Count);


			try
			{
				//Open a new keyboard and attempt to detect the type
				Keyboard keyboard = Keyboard.Open(connection);
				Debug.WriteLine("Keyboard: " + keyboard.DeviceHandle);

				//Create a new Bitmap the size of the Display
				//This bitmap represents the display of the keyboard
				Bitmap LCD = new Bitmap(Keyboard.BlackWhiteDisplaySize.Width, Keyboard.BlackWhiteDisplaySize.Height);

				//Create a Graphics to allow us to draw on the bitmap.
				Graphics g = Graphics.FromImage(LCD);

				//Clear the Bitmap. White is "transparent", Black is "opaque"
				g.Clear(Color.White);

				//Draw a circle and two lines of text
				g.DrawEllipse(Pens.Black, new Rectangle(1, 1, 10, 10));

				//Ensure that changes are actually committed to the Bitmap
				g.Flush();

				//Draw on the keyboard.
				keyboard.Draw(LCD);

				//Dispose the graphic
				g.Dispose();

				//Dispose the bitmap
				LCD.Dispose();

				//Disconnect afterwards
				keyboard.Close();
			}
			catch (InvalidOperationException)
			{
				Assert.Inconclusive("Opening a keyboard failed. This might be due to the user not having a G510 connected.");
			}

			//Disconnect from the API
			connection.Disconnect();

			//Ensure that the disconnect was successful and that the connection knows it's not connected anymore.
			Assert.AreEqual(Connection.InvalidConnectionHandle, connection.ConnectionHandle);

			//Ensure that the connection has been removed from the API's connection pool
			Assert.AreEqual<int>(0, API.Connections.Count);

			//Deinitialize the API after every connection has been removed
			API.DeInitialize();

			//Ensure the deinitialization was successful
			Assert.IsFalse(API.IsInitialized);
		}

		/// <summary>
		/// Clean up after each test and ensure all connections are closed
		/// </summary>
		[TestCleanup]
		public void CleanUp()
		{
			//If the API was left initialized, we need to clean up
			if (API.IsInitialized)
			{
				//Write a debug message that cleanup is happening
				Debug.WriteLine("Initialized after test end!");

				//Get a static copy of all current connections
				Connection[] connections = API.Connections.ToArray();

				//Clean up for each connection
				foreach (Connection connection in connections)
				{
					//If the connection is still valid, clean it up
					if (connection.ConnectionHandle != Connection.InvalidConnectionHandle)
					{
						//If the connection has an associated keyboard, close it
						if (connection.Keyboard != null)
						{
							connection.Keyboard.Close();
						}

						//Close the connection
						connection.Disconnect();
					}
				}

				//After all connections are done, deinitialize
				API.DeInitialize();
			}
		}
	}
}
