using SixLabors.ImageSharp;
using SixLabors.ImageSharp.PixelFormats;

namespace Definitions.ObjectModels.Graphics;

public static class PaletteMapLoader
{
	public const string EmbeddedPaletteResourceName = "Core.palette.png";

	public static Image<Rgba32> LoadDefaultImage()
	{
		using var stream = typeof(PaletteMapLoader).Assembly.GetManifestResourceStream(EmbeddedPaletteResourceName)
			?? throw new InvalidOperationException($"Embedded palette resource \"{EmbeddedPaletteResourceName}\" was not found");

		return Image.Load<Rgba32>(stream);
	}

	public static PaletteMap LoadDefault()
		=> new(LoadDefaultImage());

	public static PaletteMap Load(string? filename)
		=> string.IsNullOrEmpty(filename)
			? LoadDefault()
			: new PaletteMap(filename);
}
