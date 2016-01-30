using System;
using System.Drawing;
using System.Drawing.Drawing2D;

namespace LibLogitechLCD.Example
{
	class Program
	{
		static void Main(string[] args)
		{
			//Initialize the API
			//This must be done exactly one per application, before making any other calls
			API.Initialize();

			//Get Connection
			//This creates a new connection to the API.
			Connection connection = Connection.Connect("LibLogitechLCD Example");

			//Open Keyboard
			//This opens the next keyboard of the desired type.
			//BlackWhite is for G510 while QVGA is for higher-resolution G19 keyboards
			Keyboard keyboard = Keyboard.Open(connection, Keyboard.Type.BlackWhite);

			//Create a new Bitmap the size of the Display
			//This bitmap represents the display of the keyboard
			Bitmap LCD = new Bitmap(Keyboard.BlackWhiteDisplaySize.Width, Keyboard.BlackWhiteDisplaySize.Height);
			
			//Create a Graphics to allow us to draw on the bitmap.
			//This is not necessary to use the API, but instead just a way to generate a bitmap.
			Graphics g = Graphics.FromImage(LCD);

			//Generate a Font object to help us draw a string
			Font font = new Font("Arial", 7, FontStyle.Regular);
			
			//Clear the Bitmap. White is "transparent", Black is "opaque"
			g.Clear(Color.White);

			//This is necessary to tell System.Drawing that we write to a low-resolution display
			//Otherwise, text would be barely readable
			g.TextRenderingHint = System.Drawing.Text.TextRenderingHint.AntiAliasGridFit;
			
			//Draw a circle and two lines of text
			g.DrawEllipse(Pens.Black, new Rectangle(1, 1, 10, 10));
			g.DrawString("This is a test", font, SystemBrushes.WindowText, 0, 20);
			g.DrawString("Line 2", font, Brushes.Black, 0, 30);
			
			//Ensure that changes are actually committed to the Bitmap
			g.Flush();

			//Draw on the keyboard. You might need to switch applets to see the new running applet
			keyboard.Draw(LCD);

			//Dispose the graphic
			g.Dispose();

			//Dispose the bitmap
			LCD.Dispose();

			//Wait for user input. Your applet might want an idle loop here or similar
			Console.ReadLine();

			//Close the keyboard
			keyboard.Close();

			//Close the connection
			connection.Disconnect();

			//Deinitialize the API
			API.DeInitialize();
		}
	}
}
