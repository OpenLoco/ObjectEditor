using SixLabors.ImageSharp;
using SixLabors.ImageSharp.PixelFormats;

namespace Definitions.ObjectModels.Graphics.Dithering;

/// <summary>
/// Applies mask-aware, multi-pass dithering to an <see cref="Image{Rgba32}"/>, writing the resulting
/// 8-bit palette indices into an <see cref="IndexedImageFrame{Rgba32}"/>.
/// <para />
/// Dithering runs as separate, independent passes over mutually-exclusive categories so that error
/// diffusion never bleeds between them and company colours are preserved:
/// <list type="bullet">
/// <item>non-reserved (valid) colours &mdash; dithered against <c>ValidColours</c>;</item>
/// <item>company colour 1 (primary remap) &mdash; dithered against <c>PrimaryRemap</c>;</item>
/// <item>company colour 2 (secondary remap) &mdash; dithered against <c>SecondaryRemap</c>.</item>
/// </list>
/// Transparent, text-rendering and chunked-transparent ('other reserved') colours are ignored by the
/// dithering process entirely and retained verbatim.
/// </summary>
public static class MaskedDitherer
{
	/// <summary>The per-pixel category a source pixel belongs to.</summary>
	public enum ImageCategory
	{
		/// <summary>Fully transparent (palette index 0). Never dithered.</summary>
		Transparent,
		/// <summary>Text rendering / chunked-transparent reserved colours. Never dithered.</summary>
		Reserved,
		/// <summary>Regular, non-reserved colours. Dithered against <c>ValidColours</c>.</summary>
		Valid,
		/// <summary>Company colour 1 (primary remap). Dithered against <c>PrimaryRemap</c>.</summary>
		Primary,
		/// <summary>Company colour 2 (secondary remap). Dithered against <c>SecondaryRemap</c>.</summary>
		Secondary,
	}

	/// <summary>
	/// Applies <paramref name="method"/> dithering to <paramref name="source"/>, writing the chosen
	/// palette indices into <paramref name="destination"/> (whose <see cref="IndexedImageFrame.Palette"/>
	/// must be the full 256-colour <see cref="PaletteMap"/>).
	/// </summary>
	public static void Dither(
		Image<Rgba32> source,
		PaletteMap paletteMap,
		IndexedImageFrame<Rgba32> destination,
		DitheringMethod method)
	{
		var width = source.Width;
		var height = source.Height;
		var pixelCount = width * height;

		var category = new ImageCategory[pixelCount];
		var initial = new byte[pixelCount];
		for (var i = 0; i < pixelCount; ++i)
		{
			initial[i] = 0xFF;
		}

		var colourToIndex = BuildColourToIndexMap(paletteMap);

		for (var y = 0; y < height; ++y)
		{
			for (var x = 0; x < width; ++x)
			{
				var i = (y * width) + x;
				var pixel = source[x, y];

				// Binarize alpha exactly as ColorToPaletteIndex did: translucency becomes transparent.
				if (pixel.A != 255)
				{
					category[i] = ImageCategory.Transparent;
					initial[i] = 0;
					continue;
				}

				byte? exactIndex = null;
				if (colourToIndex.TryGetValue(Color.FromPixel(pixel), out var idx))
				{
					exactIndex = idx;
				}

				var cat = CategoryForIndex(exactIndex);
				category[i] = cat;
				if (cat == ImageCategory.Transparent)
				{
					initial[i] = 0;
				}
				else if (cat == ImageCategory.Reserved)
				{
					// Other reserved (text rendering / chunked transparent) must be preserved verbatim.
					initial[i] = exactIndex ?? 0;
				}
			}
		}

		// Three independent dithering passes over mutually-exclusive categories; the results are combined.
		DitherCategory(source, category, ImageCategory.Secondary, paletteMap.SecondaryRemap, initial, method);
		DitherCategory(source, category, ImageCategory.Primary, paletteMap.PrimaryRemap, initial, method);
		DitherCategory(source, category, ImageCategory.Valid, paletteMap.ValidColours, initial, method);

		for (var y = 0; y < height; ++y)
		{
			var row = destination.GetWritablePixelRowSpanUnsafe(y);
			var rowBase = y * width;
			for (var x = 0; x < width; ++x)
			{
				row[x] = initial[rowBase + x];
			}
		}
	}

	/// <summary>Builds an exact colour&rarr;index lookup for the full palette.</summary>
	static Dictionary<Color, byte> BuildColourToIndexMap(PaletteMap paletteMap)
	{
		var map = new Dictionary<Color, byte>();
		for (var i = 0; i < paletteMap.Palette.Length; ++i)
		{
			map[paletteMap.Palette[i].Color] = paletteMap.Palette[i].Index;
		}

		return map;
	}

	/// <summary>Classifies a source colour (matched exactly to a palette index, if any) into a category.</summary>
	static ImageCategory CategoryForIndex(byte? exactIndex)
	{
		if (exactIndex is byte e)
		{
			if (e == 0)
			{
				return ImageCategory.Transparent;
			}

			// Other reserved: text rendering (1..6) and chunked transparent (255) — never dithered.
			if ((e >= 1 && e <= 6) || e == 255)
			{
				return ImageCategory.Reserved;
			}

			// Company colour 1 — primary remap (7..9 and 246..254, exclusive range slicing).
			if ((e >= 7 && e <= 9) || (e >= 246 && e <= 254))
			{
				return ImageCategory.Primary;
			}

			// Company colour 2 — secondary remap (202..213).
			if (e >= 202 && e <= 213)
			{
				return ImageCategory.Secondary;
			}
		}

		// Everything else (valid palette entries and off-palette colours) is treated as non-reserved.
		return ImageCategory.Valid;
	}

	/// <summary>Runs one independent dithering pass over the pixels matching <paramref name="targetCategory"/>.</summary>
	static void DitherCategory(
		Image<Rgba32> source,
		ImageCategory[] category,
		ImageCategory targetCategory,
		(Color Color, byte Index)[] palette,
		byte[] outIndices,
		DitheringMethod method)
	{
		var width = source.Width;
		var height = source.Height;
		var pixelCount = width * height;

		// Mask of pixels belonging to this pass.
		var mask = new bool[pixelCount];
		var passCount = 0;
		for (var i = 0; i < pixelCount; ++i)
		{
			if (category[i] == targetCategory)
			{
				mask[i] = true;
				passCount++;
			}
		}

		if (passCount == 0)
		{
			return;
		}

		var paletteColors = palette.Select(c => c.Color).ToArray();

		if (IsErrorDiffusion(method))
		{
			ApplyErrorDiffusion(source, mask, palette, paletteColors, outIndices, width, height, method);
		}
		else if (method == DitheringMethod.Riemersma)
		{
			ApplyRiemersma(source, mask, palette, paletteColors, outIndices, width, height);
		}
		else
		{
			ApplySpatial(source, mask, palette, paletteColors, outIndices, width, height, method);
		}
	}

	/// <summary>Error-diffusion dithering that only diffuses error to pixels of the same category.</summary>
	static void ApplyErrorDiffusion(
		Image<Rgba32> source,
		bool[] mask,
		(Color Color, byte Index)[] palette,
		Color[] paletteColors,
		byte[] outIndices,
		int width, int height,
		DitheringMethod method)
	{
		var (kRows, kCols, offset) = KernelShape(method);
		var kernel = KernelData(method);
		var pixelCount = width * height;

		// Accumulated error at each pixel (0..255 colour space).
		var eR = new float[pixelCount];
		var eG = new float[pixelCount];
		var eB = new float[pixelCount];

		for (var y = 0; y < height; ++y)
		{
			for (var x = 0; x < width; ++x)
			{
				var idx = (y * width) + x;
				if (!mask[idx])
				{
					continue;
				}

				var p = source[x, y];
				var wantR = Math.Clamp(p.R + eR[idx], 0, 255);
				var wantG = Math.Clamp(p.G + eG[idx], 0, 255);
				var wantB = Math.Clamp(p.B + eB[idx], 0, 255);

				var nearest = NearestIndexRgb(wantR, wantG, wantB, paletteColors);
				outIndices[idx] = palette[nearest].Index;

				var t = palette[nearest].Color.ToPixel<Rgba32>();
				var errR = wantR - t.R;
				var errG = wantG - t.G;
				var errB = wantB - t.B;

				for (var ky = 0; ky < kRows; ++ky)
				{
					var targetY = y + ky;
					if (targetY >= height)
					{
						continue;
					}

					for (var kx = 0; kx < kCols; ++kx)
					{
						var coeff = kernel[(ky * kCols) + kx];
						if (coeff == 0)
						{
							continue;
						}

						var targetX = x + (kx - offset);
						if (targetX < 0 || targetX >= width)
						{
							continue;
						}

						var targetIdx = (targetY * width) + targetX;
						if (!mask[targetIdx])
						{
							// Do not diffuse error across category boundaries — keeps passes independent.
							continue;
						}

						eR[targetIdx] += errR * coeff;
						eG[targetIdx] += errG * coeff;
						eB[targetIdx] += errB * coeff;
					}
				}
			}
		}
	}

	/// <summary>Ordered / spatial dithering (Bayer, Ordered3x3, blue noise) — pure per-pixel thresholding.</summary>
	static void ApplySpatial(
		Image<Rgba32> source,
		bool[] mask,
		(Color Color, byte Index)[] palette,
		Color[] paletteColors,
		byte[] outIndices,
		int width, int height,
		DitheringMethod method)
	{
		var isBlueNoise = method == DitheringMethod.BlueNoise;
		var (threshold, modX, modY) = isBlueNoise
			? (null, 0, 0)
			: OrderedMatrix(MatrixLength(method));
		var spread = CalculatePaletteSpread(paletteColors.Length);
		var noise = isBlueNoise ? BlueNoiseTexture : null;

		for (var y = 0; y < height; ++y)
		{
			for (var x = 0; x < width; ++x)
			{
				var idx = (y * width) + x;
				if (!mask[idx])
				{
					continue;
				}

				var p = source[x, y];
				var factor = isBlueNoise
					? noise![(y % 32 * 32) + (x % 32)] * (64f / 255f)
					: spread * threshold![y % modY][x % modX];

				var wantR = Math.Clamp(p.R + factor, 0, 255);
				var wantG = Math.Clamp(p.G + factor, 0, 255);
				var wantB = Math.Clamp(p.B + factor, 0, 255);

				outIndices[idx] = palette[NearestIndexRgb(wantR, wantG, wantB, paletteColors)].Index;
			}
		}
	}

	/// <summary>Riemersma-style error diffusion along a Hilbert curve, masked to a single category.</summary>
	static void ApplyRiemersma(
		Image<Rgba32> source,
		bool[] mask,
		(Color Color, byte Index)[] palette,
		Color[] paletteColors,
		byte[] outIndices,
		int width, int height)
	{
		const int radius = 16;

		// Collect in-pass pixels in Hilbert traversal order so diffusion stays within the category.
		var order = 0;
		var maxDim = Math.Max(width, height);
		while ((1 << order) < maxDim)
		{
			order++;
		}

		var hilbertSize = 1 << order;
		var traverse = new List<int>();
		var hilbertLength = hilbertSize * hilbertSize;
		for (var i = 0; i < hilbertLength; ++i)
		{
			var (hx, hy) = HilbertIndexToCoordinates(i, hilbertSize);
			if (hx < width && hy < height)
			{
				var idx = (hy * width) + hx;
				if (mask[idx])
				{
					traverse.Add(idx);
				}
			}
		}

		if (traverse.Count == 0)
		{
			return;
		}

		var weights = new float[radius];
		var weightSum = 0f;
		for (var i = 0; i < radius; ++i)
		{
			weights[i] = MathF.Exp(-i / (radius / 4f));
			weightSum += weights[i];
		}
		for (var i = 0; i < radius; ++i)
		{
			weights[i] /= weightSum;
		}

		var count = traverse.Count;
		var eR = new float[count];
		var eG = new float[count];
		var eB = new float[count];

		for (var i = 0; i < count; ++i)
		{
			var idx = traverse[i];
			var p = source[idx % width, idx / width];
			var wantR = Math.Clamp(p.R + eR[i], 0, 255);
			var wantG = Math.Clamp(p.G + eG[i], 0, 255);
			var wantB = Math.Clamp(p.B + eB[i], 0, 255);

			var nearest = NearestIndexRgb(wantR, wantG, wantB, paletteColors);
			outIndices[idx] = palette[nearest].Index;

			var t = palette[nearest].Color.ToPixel<Rgba32>();
			var errR = wantR - t.R;
			var errG = wantG - t.G;
			var errB = wantB - t.B;

			for (var j = 0; j < radius && i + 1 + j < count; ++j)
			{
				eR[i + 1 + j] += errR * weights[j];
				eG[i + 1 + j] += errG * weights[j];
				eB[i + 1 + j] += errB * weights[j];
			}
		}
	}

	static (int x, int y) HilbertIndexToCoordinates(int index, int size)
	{
		var x = 0;
		var y = 0;
		var t = index;
		for (var s = 1; s < size; s <<= 1)
		{
			var rx = (t >> 1) & 1;
			var ry = (t ^ rx) & 1;
			if (ry == 0)
			{
				if (rx == 1)
				{
					x = s - 1 - x;
					y = s - 1 - y;
				}

				(x, y) = (y, x);
			}

			x += s * rx;
			y += s * ry;
			t >>= 2;
		}

		return (x, y);
	}

	static bool IsErrorDiffusion(DitheringMethod method)
		=> method switch
		{
			DitheringMethod.FloydSteinberg => true,
			DitheringMethod.Atkinson => true,
			DitheringMethod.Burkes => true,
			DitheringMethod.JarvisJudiceNinke => true,
			DitheringMethod.Sierra2 => true,
			DitheringMethod.Sierra3 => true,
			DitheringMethod.SierraLite => true,
			DitheringMethod.StevensonArce => true,
			DitheringMethod.Stucki => true,
			_ => false,
		};

	static int MatrixLength(DitheringMethod method)
		=> method switch
		{
			DitheringMethod.Bayer2x2 => 2,
			DitheringMethod.Bayer4x4 => 4,
			DitheringMethod.Bayer8x8 => 8,
			DitheringMethod.Bayer16x16 => 16,
			_ => 3,
		};

	static int CalculatePaletteSpread(int colors)
		=> (int)(255 / Math.Max(1, Math.Pow(colors, 1.0 / 3) - 1));

	static int NearestIndexRgb(float r, float g, float b, Color[] paletteColors)
	{
		var best = 0;
		var bestDist = float.MaxValue;
		for (var i = 0; i < paletteColors.Length; ++i)
		{
			var p = paletteColors[i].ToPixel<Rgba32>();
			var drf = p.R - r;
			var dgf = p.G - g;
			var dbf = p.B - b;
			var dist = (drf * drf) + (dgf * dgf) + (dbf * dbf);
			if (dist < bestDist)
			{
				bestDist = dist;
				best = i;
			}
		}

		return best;
	}

	// Error-diffusion kernels stored flat (row-major), in the same layout ImageSharp's ErrorDither uses:
	// row 0 is the current pixel's row; each element lands at (x + col - offset, y + row).
	// Each kernel is described by (rows, cols, offset).
	static (int rows, int cols, int offset) KernelShape(DitheringMethod method)
		=> method switch
		{
			DitheringMethod.Atkinson => (3, 4, 1),
			DitheringMethod.Burkes => (2, 5, 2),
			DitheringMethod.JarvisJudiceNinke => (3, 5, 2),
			DitheringMethod.Sierra2 => (2, 5, 2),
			DitheringMethod.Sierra3 => (3, 5, 2),
			DitheringMethod.SierraLite => (2, 3, 1),
			DitheringMethod.StevensonArce => (4, 7, 3),
			DitheringMethod.Stucki => (3, 5, 2),
			_ => (2, 3, 1),
		};

	static float[] KernelData(DitheringMethod method)
		=> method switch
		{
			DitheringMethod.Atkinson => AtkinsonKernel,
			DitheringMethod.Burkes => BurkesKernel,
			DitheringMethod.JarvisJudiceNinke => JarvisJudiceNinkeKernel,
			DitheringMethod.Sierra2 => Sierra2Kernel,
			DitheringMethod.Sierra3 => Sierra3Kernel,
			DitheringMethod.SierraLite => SierraLiteKernel,
			DitheringMethod.StevensonArce => StevensonArceKernel,
			DitheringMethod.Stucki => StuckiKernel,
			_ => FloydSteinbergKernel,
		};

	static readonly float[] FloydSteinbergKernel =
	[
		0, 0, 7f / 16f,
		3f / 16f, 5f / 16f, 1f / 16f,
	];

	static readonly float[] AtkinsonKernel =
	[
		0, 0, 1f / 8f, 1f / 8f,
		1f / 8f, 1f / 8f, 1f / 8f, 0,
		0, 1f / 8f, 0, 0,
	];

	static readonly float[] BurkesKernel =
	[
		0, 0, 0, 8f / 32f, 4f / 32f,
		2f / 32f, 4f / 32f, 8f / 32f, 4f / 32f, 2f / 32f,
	];

	static readonly float[] JarvisJudiceNinkeKernel =
	[
		0, 0, 0, 7f / 48f, 5f / 48f,
		3f / 48f, 5f / 48f, 7f / 48f, 5f / 48f, 3f / 48f,
		1f / 48f, 3f / 48f, 5f / 48f, 3f / 48f, 1f / 48f,
	];

	static readonly float[] Sierra2Kernel =
	[
		0, 0, 0, 4f / 16f, 3f / 16f,
		1f / 16f, 2f / 16f, 3f / 16f, 2f / 16f, 1f / 16f,
	];

	static readonly float[] Sierra3Kernel =
	[
		0, 0, 0, 5f / 32f, 3f / 32f,
		2f / 32f, 4f / 32f, 5f / 32f, 4f / 32f, 2f / 32f,
		0, 2f / 32f, 3f / 32f, 2f / 32f, 0,
	];

	static readonly float[] SierraLiteKernel =
	[
		0, 0, 2f / 4f,
		1f / 4f, 1f / 4f, 0,
	];

	static readonly float[] StevensonArceKernel =
	[
		0, 0, 0, 0, 0, 32f / 200f, 0,
		12f / 200f, 0, 26f / 200f, 0, 30f / 200f, 0, 16f / 200f,
		0, 12f / 200f, 0, 26f / 200f, 0, 12f / 200f, 0,
		5f / 200f, 0, 12f / 200f, 0, 12f / 200f, 0, 5f / 200f,
	];

	static readonly float[] StuckiKernel =
	[
		0, 0, 0, 8f / 42f, 4f / 42f,
		2f / 42f, 4f / 42f, 8f / 42f, 4f / 42f, 2f / 42f,
		1f / 42f, 2f / 42f, 4f / 42f, 2f / 42f, 1f / 42f,
	];

	/// <summary>Builds an ordered (Bayer) threshold matrix of the given side length, matching ImageSharp.</summary>
	static (float[][], int, int) OrderedMatrix(int length)
	{
		var bayer = CreateBayerMatrixBase(length);
		var m2 = length * length;
		var threshold = new float[length][];
		for (var y = 0; y < length; ++y)
		{
			threshold[y] = new float[length];
			for (var x = 0; x < length; ++x)
			{
				threshold[y][x] = ((bayer[y][x] + 1) / m2) - 0.5f;
			}
		}

		return (threshold, length, length);
	}

	static int[][] CreateBayerMatrixBase(int length)
	{
		var exponent = 0;
		var bayerLength = 0;
		do
		{
			exponent++;
			bayerLength = 1 << exponent;
		}
		while (length > bayerLength);

		var matrix = new int[length][];
		var i = 0;
		for (var y = 0; y < length; ++y)
		{
			matrix[y] = new int[length];
			for (var x = 0; x < length; ++x)
			{
				matrix[y][x] = Bayer(i / length, i % length, exponent);
				i++;
			}
		}

		var maxValue = bayerLength * bayerLength;
		var missing = 0;
		for (var v = 0; v < maxValue; ++v)
		{
			var found = false;
			for (var y = 0; y < length && !found; ++y)
			{
				for (var x = 0; x < length; ++x)
				{
					if (matrix[y][x] == v)
					{
						matrix[y][x] -= missing;
						found = true;
						break;
					}
				}
			}

			if (!found)
			{
				missing++;
			}
		}

		return matrix;
	}

	static int Bayer(int x, int y, int order)
	{
		var result = 0;
		for (var i = 0; i < order; ++i)
		{
			var xOddXorYOdd = (x & 1) ^ (y & 1);
			var xOdd = x & 1;
			result = ((result << 1 | xOddXorYOdd) << 1) | xOdd;
			x >>= 1;
			y >>= 1;
		}

		return result;
	}

	/// <summary>A 32x32 blue-noise texture (same generation as <see cref="BlueNoiseDither"/>).</summary>
	static readonly float[] BlueNoiseTexture = GenerateBlueNoise32x32();

	static float[] GenerateBlueNoise32x32()
	{
		const int size = 32;
		var rng = new Random(0x2E4B1A6D);
		var noise = new float[size * size];
		for (var i = 0; i < noise.Length; ++i)
		{
			noise[i] = (float)rng.NextDouble();
		}

		for (var iter = 0; iter < 4; iter++)
		{
			var blurred = new float[size * size];
			for (var y = 0; y < size; ++y)
			{
				for (var x = 0; x < size; ++x)
				{
					var sum = 0f;
					for (var dy = -1; dy <= 1; ++dy)
					{
						for (var dx = -1; dx <= 1; ++dx)
						{
							var nx = (x + dx + size) % size;
							var ny = (y + dy + size) % size;
							sum += noise[(ny * size) + nx];
						}
					}

					blurred[(y * size) + x] = sum / 9f;
				}
			}

			for (var i = 0; i < noise.Length; ++i)
			{
				noise[i] = Math.Clamp(noise[i] - blurred[i] + 0.5f, 0f, 1f);
			}
		}

		for (var i = 0; i < noise.Length; ++i)
		{
			noise[i] -= 0.5f;
		}

		return noise;
	}
}