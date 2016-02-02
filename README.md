# LibLogitechLCD
Library to create applications for Logitech G510 and similar LCD keyboards

##General usage
In general, using LibLogitechLCD always looks as follows:
```csharp
API.Initialize();
  Connection connection = Connection.Connect("FooBar");
    Keyboard keyboard = Keyboard.Open(connection, Keyboard.Type.BlackWhite);
      keyboard.Draw(someBitmap);
    keyboard.Close();
  connection.Disconnect();
API.DeInitialize();
```

The indentation should make it clearer that the API has to be initialized first, then a new connection is created, which is then used to open a Keyboard to draw to.
You can use a Bitmap to draw on the Keyboard, just ensure it fits your display. `Keyboard` has handy static sizes built in for your pleasure.
Please don't forget that the Keyboard, the Connection and the API need to be closed again in reverse order.

For a more in-depth description, please check out the example I provided in this respository. It should run on any G510 keyboard. I couldn't test the G19 variant because I don't have a G19. Feel free to contribute.

##Licensing
LibLogitechLCD is built ontop of [DMcLgLCD](http://www.mangareader.com/dmclglcd.html) and the Logitech API. I cannot say for sure how the Logitech API and corresponding DLL's are licensed, but my own code is licensed under GNU LGPL v3.
