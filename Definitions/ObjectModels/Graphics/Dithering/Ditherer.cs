using SixLabors.ImageSharp.Processing;
using SixLabors.ImageSharp.Processing.Processors.Dithering;

namespace Definitions.ObjectModels.Graphics.Dithering;

/// <summary>Maps <see cref="DitheringMethod"/> values to ImageSharp <see cref="IDither"/> implementations,
/// and provides custom dither implementations for algorithms not available in ImageSharp.</summary>
public static class DitherFactory
{
	/// <summary>Creates the <see cref="IDither"/> instance for the given method.</summary>
	public static IDither Create(DitheringMethod method)
		=> method switch
		{
			DitheringMethod.FloydSteinberg => KnownDitherings.FloydSteinberg,
			DitheringMethod.Atkinson => KnownDitherings.Atkinson,
			DitheringMethod.Burkes => KnownDitherings.Burks,
			DitheringMethod.JarvisJudiceNinke => KnownDitherings.JarvisJudiceNinke,
			DitheringMethod.Sierra2 => KnownDitherings.Sierra2,
			DitheringMethod.Sierra3 => KnownDitherings.Sierra3,
			DitheringMethod.SierraLite => KnownDitherings.SierraLite,
			DitheringMethod.StevensonArce => KnownDitherings.StevensonArce,
			DitheringMethod.Stucki => KnownDitherings.Stucki,
			DitheringMethod.Bayer2x2 => KnownDitherings.Bayer2x2,
			DitheringMethod.Bayer4x4 => KnownDitherings.Bayer4x4,
			DitheringMethod.Bayer8x8 => KnownDitherings.Bayer8x8,
			DitheringMethod.Bayer16x16 => KnownDitherings.Bayer16x16,
			DitheringMethod.Ordered3x3 => KnownDitherings.Ordered3x3,
			DitheringMethod.BlueNoise => BlueNoiseDither.Instance,
			DitheringMethod.Riemersma => new RiemersmaDither(),
			_ => throw new ArgumentOutOfRangeException(nameof(method)),
		};
}
