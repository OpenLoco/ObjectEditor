using SixLabors.ImageSharp;
using SixLabors.ImageSharp.PixelFormats;

namespace Definitions.ObjectModels.Graphics;

public static class PaletteMapLoader
{
	public const string EmbeddedPaletteResourceName = "Core.palette.png";

	static PaletteMap? _current;

	/// <summary>The process-wide palette map, initialised by the GUI on startup (or lazily to the default).
	/// Available anywhere without threading it through arguments.</summary>
	public static PaletteMap Current
		=> _current ??= LoadDefault();

	public static void SetCurrent(PaletteMap paletteMap)
	{
		ArgumentNullException.ThrowIfNull(paletteMap);
		_current = paletteMap;
	}

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
