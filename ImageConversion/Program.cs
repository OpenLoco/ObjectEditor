using System.Drawing;
using System.Drawing.Imaging;

foreach (var file in Directory.EnumerateFiles(AppContext.BaseDirectory, "*.png", SearchOption.AllDirectories))
{
	try
	{
		Bitmap newBitmap;

		using (var originalBitmap = new Bitmap(file))
		{
			newBitmap = new Bitmap(originalBitmap.Width, originalBitmap.Height, PixelFormat.Format32bppArgb);

			for (var y = 0; y < originalBitmap.Height; y++)
			{
				for (var x = 0; x < originalBitmap.Width; x++)
				{
					var pixelColor = originalBitmap.GetPixel(x, y);
					if (pixelColor.R == 0 && pixelColor.G == 0 && pixelColor.B == 255)
					{
						newBitmap.SetPixel(x, y, Color.Transparent);
					}
					else
					{
						newBitmap.SetPixel(x, y, pixelColor);
					}
				}
			}
		}

		newBitmap.Save(file, ImageFormat.Png);

		Console.WriteLine($"Processed {Path.GetFileName(file)} and saved successfully!");
	}
	catch (Exception ex)
	{
		Console.WriteLine("An error occurred: " + ex.Message);
		Console.WriteLine($"Please make sure {Path.GetFileName(file)} exists and is a valid PNG image file.");
	}
}
