using ReactiveUI;
using OpenLoco.ObjectEditor.DatFileParsing;
using System.Linq;
using ReactiveUI.Fody.Helpers;
using Avalonia.Media.Imaging;
using System.Collections.Generic;
using OpenLoco.ObjectEditor.Headers;
using OpenLoco.ObjectEditor;
using SixLabors.ImageSharp.PixelFormats;
using Avalonia;
using Avalonia.Platform;

namespace AvaGui.ViewModels
{
	public class ImageTableViewModel : ReactiveObject
	{
		public ImageTableViewModel(ILocoObject parent, PaletteMap paletteMap)
		{
			Parent = parent;
			PaletteMap = paletteMap;
		}
		ILocoObject Parent;

		[Reactive] public PaletteMap PaletteMap { get; set; }

		public Bitmap FirstImage { get => Images.FirstOrDefault(); }

		public IEnumerable<Bitmap> Images
		{
			get => CreateImages(Parent.G1Elements, PaletteMap);
			set { }
		}

		public static IEnumerable<Bitmap> CreateImages(IEnumerable<G1Element32> g1Elements, PaletteMap paletteMap)
		{
			foreach (var g1Element in g1Elements)
			{
				if (g1Element.ImageData.Length == 0)
				{
					//logger?.Info($"skipped loading g1 element {i} with 0 length");
					continue;
				}

				if (g1Element.Flags.HasFlag(G1ElementFlags.IsR8G8B8Palette))
				{
					yield return G1RGBToBitmap(g1Element);
				}
				else
				{
					yield return G1IndexedToBitmap(g1Element, paletteMap, true);
				}
			}
		}

		static Bitmap G1RGBToBitmap(G1Element32 g1Element)
		{
			var imageData = g1Element.ImageData;
			var writeableBitmap = new WriteableBitmap(
				new PixelSize(g1Element.Width, g1Element.Height),
				new Vector(96, 96),  // DPI
				PixelFormat.Rgba8888); // Or a suitable pixel format

			using (var lockedBitmap = writeableBitmap.Lock())
			{
				unsafe
				{
					var pointer = (uint*)lockedBitmap.Address; // Access pixel data directly

					for (var y = 0; y < g1Element.Height; y++)
					{
						for (var x = 0; x < g1Element.Width; x++)
						{
							// Calculate pixel index
							var index = x + (y * g1Element.Width);

							// Set pixel color (example: red)
							pointer[index] = 0xFFFF0000;
						}
					}
				}
			}

			return writeableBitmap;
		}

		static Bitmap G1IndexedToBitmap(G1Element32 g1Element, PaletteMap paletteMap, bool useTransparency = false)
		{
			var writeableBitmap = new WriteableBitmap(
				new PixelSize(g1Element.Width, g1Element.Height),
				new Vector(96, 96),  // DPI
				PixelFormat.Rgba8888); // Or a suitable pixel format

			using (var lockedBitmap = writeableBitmap.Lock())
			{
				unsafe
				{
					var ptr = (byte*)lockedBitmap.Address; // Access pixel data directly

					for (var y = 0; y < g1Element.Height; y++)
					{
						for (var x = 0; x < g1Element.Width; x++)
						{
							var index = x + (y * g1Element.Width);
							var paletteIndex = g1Element.ImageData[index];

							if (paletteIndex == 0 && useTransparency)
							{
								ptr += 4;
							}
							else
							{
								var colour = paletteMap.Palette[paletteIndex].Color;
								var pixel = colour.ToPixel<Rgb24>();

								//var ptr = (byte*)pointer;
								*ptr++ = pixel.R;
								*ptr++ = pixel.G;
								*ptr++ = pixel.B;
								*ptr++ = 255;
							}
						}
					}
				}
			}

			return writeableBitmap;
		}
	}
}
