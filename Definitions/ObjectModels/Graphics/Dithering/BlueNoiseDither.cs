using SixLabors.ImageSharp;
using SixLabors.ImageSharp.PixelFormats;
using SixLabors.ImageSharp.Processing.Processors.Dithering;

namespace Definitions.ObjectModels.Graphics.Dithering;

/// <summary><see cref="IDither"/> implementation that adds precomputed blue noise
/// before palette quantisation to avoid low-frequency artifacts.</summary>
public sealed class BlueNoiseDither : IDither
{
	/// <summary>Singleton instance.</summary>
	public static readonly BlueNoiseDither Instance = new();

	private static readonly float[] NoiseTexture = GenerateBlueNoise32x32();

	private static float[] GenerateBlueNoise32x32()
	{
		const int size = 32;
		var rng = new Random(0x2E4B1A6D);
		var noise = new float[size * size];

		for (var i = 0; i < noise.Length; i++)
		{
			noise[i] = (float)rng.NextDouble();
		}

		// High-pass filtering iterations to remove low frequencies
		for (var iter = 0; iter < 4; iter++)
		{
			var blurred = new float[size * size];
			for (var y = 0; y < size; y++)
			{
				for (var x = 0; x < size; x++)
				{
					var sum = 0f;
					for (var dy = -1; dy <= 1; dy++)
					{
						for (var dx = -1; dx <= 1; dx++)
						{
							var nx = (x + dx + size) % size;
							var ny = (y + dy + size) % size;
							sum += noise[(ny * size) + nx];
						}
					}

					blurred[(y * size) + x] = sum / 9f;
				}
			}

			for (var i = 0; i < noise.Length; i++)
			{
				noise[i] = Math.Clamp(noise[i] - blurred[i] + 0.5f, 0f, 1f);
			}
		}

		for (var i = 0; i < noise.Length; i++)
		{
			noise[i] -= 0.5f;
		}

		return noise;
	}

	/// <inheritdoc/>
	public void ApplyPaletteDither<TPaletteDitherImageProcessor, TPixel>(
	in TPaletteDitherImageProcessor processor,
	ImageFrame<TPixel> source,
	Rectangle bounds)
	where TPaletteDitherImageProcessor : struct, IPaletteDitherImageProcessor<TPixel>
	where TPixel : unmanaged, IPixel<TPixel>
	{
		const int noiseSize = 32;
		const float spread = 64f / 255f; // spread in [0,1] scaled-vector units
		var noiseTex = NoiseTexture;
		var proc = processor; // copy 'in' parameter for use inside lambda

		source.ProcessPixelRows(accessor =>
		{
			for (var y = bounds.Top; y < bounds.Bottom; y++)
			{
				var row = accessor.GetRowSpan(y);
				for (var x = bounds.Left; x < bounds.Right; x++)
				{
					var pixel = row[x];
					var noiseVal = noiseTex[(y % noiseSize * noiseSize) + (x % noiseSize)];
					var offset = noiseVal * spread; // noiseVal = -0.5..+0.5, spread ~ 0.25

					// Add blue noise in scaled-vector space [0,1]
					var vec = pixel.ToScaledVector4();
					vec.X = Math.Clamp(vec.X + offset, 0, 1);
					vec.Y = Math.Clamp(vec.Y + offset, 0, 1);
					vec.Z = Math.Clamp(vec.Z + offset, 0, 1);

					var adjusted = TPixel.FromScaledVector4(vec);
					row[x] = proc.GetPaletteColor(adjusted);
				}
			}
		});
	}

	/// <inheritdoc/>
	void IDither.ApplyQuantizationDither<TFrameQuantizer, TPixel>(
	ref TFrameQuantizer quantizer,
	ImageFrame<TPixel> source,
	IndexedImageFrame<TPixel> destination,
	Rectangle bounds)
	{
		throw new NotSupportedException(
		"BlueNoiseDither only supports palette dithering via ApplyPaletteDither.");
	}
}
