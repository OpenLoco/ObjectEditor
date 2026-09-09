using SixLabors.ImageSharp;
using SixLabors.ImageSharp.PixelFormats;
using SixLabors.ImageSharp.Processing.Processors.Dithering;
using System.Numerics;

namespace Definitions.ObjectModels.Graphics.Dithering;

/// <summary><see cref="IDither"/> implementation that uses a Hilbert space-filling curve
/// for error-diffusion dithering, reducing directional streak artifacts.</summary>
public sealed class RiemersmaDither : IDither
{
	private const int Radius = 16;

	/// <summary>Applies Riemersma-style error-diffusion dithering using a Hilbert curve traversal
	/// and exponential decay error weights to subsequent pixels along the curve.</summary>
	public void ApplyPaletteDither<TPaletteDitherImageProcessor, TPixel>(
	in TPaletteDitherImageProcessor processor,
	ImageFrame<TPixel> source,
	Rectangle bounds)
	where TPaletteDitherImageProcessor : struct, IPaletteDitherImageProcessor<TPixel>
	where TPixel : unmanaged, IPixel<TPixel>
	{
		var width = bounds.Width;
		var height = bounds.Height;
		var pixels = width * height;

		// Build Hilbert traversal order
		var maxDim = Math.Max(width, height);
		var order = 0;
		while ((1 << order) < maxDim)
		{
			order++;
		}

		var hilbertSize = 1 << order;
		var hilbertLength = hilbertSize * hilbertSize;
		var traverse = new List<(int x, int y, int flatIdx)>(pixels);
		for (var i = 0; i < hilbertLength; i++)
		{
			var (hx, hy) = HilbertIndexToCoordinates(i, hilbertSize);
			if (hx < width && hy < height)
			{
				traverse.Add((bounds.Left + hx, bounds.Top + hy, (hy * width) + hx));
			}
		}

		// Copy source pixels into float buffers (in [0,1] scaled-vector space)
		var rBuf = new float[pixels];
		var gBuf = new float[pixels];
		var bBuf = new float[pixels];

		for (var i = 0; i < traverse.Count; i++)
		{
			var (tx, ty, tIdx) = traverse[i];
			var px = source[tx, ty];
			var vec = px.ToScaledVector4(); // returns [0,1]-scaled components
			rBuf[tIdx] = vec.X;
			gBuf[tIdx] = vec.Y;
			bBuf[tIdx] = vec.Z;
		}

		// Exponential decay weights for error distribution along the curve
		var weights = new float[Radius];
		var weightSum = 0f;

		for (var i = 0; i < Radius; i++)
		{
			weights[i] = MathF.Exp(-i / (Radius / 4f));
			weightSum += weights[i];
		}

		for (var i = 0; i < Radius; i++)
		{
			weights[i] /= weightSum;
		}

		// Error buffers (circular) in [0,1] scaled-vector space
		var errBufX = new float[Radius];
		var errBufY = new float[Radius];
		var errBufZ = new float[Radius];

		for (var i = 0; i < traverse.Count; i++)
		{
			var (tx, ty, tIdx) = traverse[i];

			// Apply accumulated error, clamp to [0,1] scaled-vector range
			var rx = Math.Clamp(rBuf[tIdx] + errBufX[0], 0, 1);
			var gx = Math.Clamp(gBuf[tIdx] + errBufY[0], 0, 1);
			var bx = Math.Clamp(bBuf[tIdx] + errBufZ[0], 0, 1);

			// Build a scaled-vector4 in [0,1] space
			var adjustedVec = new Vector4(rx, gx, bx, 1f);
			var adjustedPixel = TPixel.FromScaledVector4(adjustedVec);

			// Quantize using the palette processor
			var quantized = processor.GetPaletteColor(adjustedPixel);
			source[tx, ty] = quantized;

			// Calculate error in the same [0,1] scaled-vector space
			var qVec = quantized.ToScaledVector4();
			var errX = rx - qVec.X;
			var errY = gx - qVec.Y;
			var errZ = bx - qVec.Z;

			// Shift error buffers (circular shift left)
			for (var j = 0; j < Radius - 1; j++)
			{
				errBufX[j] = errBufX[j + 1];
				errBufY[j] = errBufY[j + 1];
				errBufZ[j] = errBufZ[j + 1];
			}

			errBufX[Radius - 1] = 0;
			errBufY[Radius - 1] = 0;
			errBufZ[Radius - 1] = 0;

			// Distribute error forward along the Hilbert curve
			for (var j = 0; j < Radius && i + 1 + j < traverse.Count; j++)
			{
				errBufX[j] += errX * weights[j];
				errBufY[j] += errY * weights[j];
				errBufZ[j] += errZ * weights[j];
			}
		}
	}

	/// <inheritdoc/>
	void IDither.ApplyQuantizationDither<TFrameQuantizer, TPixel>(
	ref TFrameQuantizer quantizer,
	ImageFrame<TPixel> source,
	IndexedImageFrame<TPixel> destination,
	Rectangle bounds)
	=> throw new NotSupportedException("RiemersmaDither only supports palette dithering via ApplyPaletteDither.");

	/// <summary>Converts a Hilbert traversal index to x/y coordinates for a power-of-two size.</summary>
	private static (int x, int y) HilbertIndexToCoordinates(int index, int size)
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
}
