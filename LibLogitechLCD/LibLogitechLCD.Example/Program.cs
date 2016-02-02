using System;
using System.Drawing;
using System.Drawing.Drawing2D;

namespace LibLogitechLCD.Example
{
	class Program
	{
		static void Main(string[] args)
		{
			try
			{
				//Initialize the API
				//This must be done exactly one per application, before making any other calls
				API.Initialize();
			}
			catch (InitializationException ex)
			{
				//If initialization fails, print an error and return
				Console.Error.WriteLine(ex.Message);
				return;
			}

			Connection connection;
			try
			{
				//Try creating a new connection to the API
				connection = Connection.Connect("LibLogitechLCD Example");
			}
			catch (APIException ex)
			{
				//If connection fails, deinitialize the API, print an error and return
				Console.Error.WriteLine(ex.Message);
				API.DeInitialize();
				return;
			}

			Keyboard keyboard;
			try
			{
				//Open Keyboard
	
				//This opens the next keyboard of the desired type.
				//BlackWhite is for G510 while QVGA is for higher-resolution G19 keyboards
				keyboard = Keyboard.Open(connection);
			}
			catch (APIException ex)
			{
				//If no keyboard could be opened, close the connection, deinitialize the API and return
				Console.Error.WriteLine(ex.Message);
				connection.Disconnect();
				API.DeInitialize();
				return;
			}

			//Create a new Bitmap the size of the Display
			//This bitmap represents the display of the keyboard
			Bitmap LCD = new Bitmap(Keyboard.BlackWhiteDisplaySize.Width, Keyboard.BlackWhiteDisplaySize.Height);
			
			//Create a Graphics to allow us to draw on the bitmap.
			//This is not necessary to use the API, but instead just a way to generate a bitmap.
			Graphics g = Graphics.FromImage(LCD);

			//Generate a Font object to help us draw a string
			Font font = new Font("Arial", 7, FontStyle.Regular);

			//Make a placeholder for all the pressed buttons
			Keyboard.Buttons buttons;

			//This is necessary to tell System.Drawing that we write to a low-resolution display
			//Otherwise, text would be barely readable
			g.TextRenderingHint = System.Drawing.Text.TextRenderingHint.AntiAliasGridFit;

			//Initialize the bitmap all transparent and draw it out
			g.Clear(Keyboard.TransparentColor);
			keyboard.Draw(LCD);

			//Make our applet the active applet. Otherwise, users would have to select it manually.
			//NOTE: Only works after the first Draw instruction has been used, hence the above "empty draw"
			//WARNING: Never use this in an update loop, since it would continuously "steal" the keyboard to display your app
			keyboard.BringToFront();

			do
			{
				//Clear the Bitmap. White is "transparent", Black is "opaque"
				g.Clear(Color.White);

				buttons = keyboard.ReadButtons();

				//Draw a circle and two lines of text
				g.DrawEllipse(Pens.Black, new Rectangle(130, 20, 20, 20));
				g.DrawString("Press any button to display them", font, SystemBrushes.WindowText, 0, 0);
				g.DrawString("Exit with Button 4 or Cancel", font, SystemBrushes.WindowText, 0, 10);
				g.DrawString("You pressed these buttons: ", font, SystemBrushes.WindowText, 0, 20);
				g.DrawString(buttons.ToString(), font, Brushes.Black, 0, 30);

				//Ensure that changes are actually committed to the Bitmap
				g.Flush();

				//Draw on the keyboard. You might need to switch applets to see the new running applet
				keyboard.Draw(LCD);

				
			} while (!(buttons.HasFlag(Keyboard.Buttons.Button4) || buttons.HasFlag(Keyboard.Buttons.Cancel)));
			
			//Dispose the graphic
			g.Dispose();

			//Dispose the bitmap
			LCD.Dispose();

			//Close the keyboard
			keyboard.Close();

			//Close the connection
			connection.Disconnect();

			//Deinitialize the API
			API.DeInitialize();
		}
	}
}
