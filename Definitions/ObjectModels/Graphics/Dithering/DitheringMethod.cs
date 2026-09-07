namespace Definitions.ObjectModels.Graphics.Dithering;

/// <summary>Available dithering algorithms for palette-indexed image conversion.</summary>
public enum DitheringMethod
{
	/// <summary>No dithering — nearest-neighbour palette mapping. Also used as the "no default" sentinel in editor settings.</summary>
	None = 0,
	// -- Built-in ImageSharp error-diffusion dithers ---------------------------------
	/// <summary>Classic error diffusion (7/16, 3/16, 5/16, 1/16 kernel).</summary>
	FloydSteinberg,
	/// <summary>Atkinson error diffusion — good contrast preservation.</summary>
	Atkinson,
	/// <summary>Burkes error diffusion.</summary>
	Burkes,
	/// <summary>Jarvis-Judice-Ninke error diffusion — 12-pixel kernel.</summary>
	JarvisJudiceNinke,
	/// <summary>Sierra-2 error diffusion — 2-row kernel.</summary>
	Sierra2,
	/// <summary>Sierra-3 error diffusion — 3-row kernel.</summary>
	Sierra3,
	/// <summary>Sierra Lite error diffusion — fast 2-row kernel.</summary>
	SierraLite,
	/// <summary>Stevenson-Arce error diffusion.</summary>
	StevensonArce,
	/// <summary>Stucki error diffusion — 12-pixel kernel.</summary>
	Stucki,

	// -- Built-in ImageSharp ordered dither ------------------------------------------
	/// <summary>Ordered dithering using Bayer threshold matrices — fast and deterministic.</summary>
	Bayer2x2,
	Bayer4x4,
	Bayer8x8,
	Bayer16x16,
	/// <summary>Ordered dithering using a 3x3 threshold matrix — fast and deterministic.</summary>
	Ordered3x3,

	// -- Custom dithers (not available in ImageSharp) --------------------------------
	/// <summary>Adds precomputed high-frequency (blue) noise before quantisation — avoids low-frequency artifacts.</summary>
	BlueNoise,
	/// <summary>Error diffusion along a Hilbert space-filling curve — reduces directional streak artifacts.</summary>
	Riemersma,
}
