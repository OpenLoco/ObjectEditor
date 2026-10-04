using Definitions.ObjectModels.Graphics;
using SixLabors.ImageSharp;

namespace ObjectService.Services;

/// <summary>
/// Shared helpers for turning a decoded object image table into the PNGs and metadata stored by
/// <see cref="IObjectImageCache"/>. Used both at import time (to warm the cache while the object is
/// already decoded) and on the request path (to backfill frames the cache does not hold yet).
/// </summary>
public static class ObjectImageRender
{
	/// <summary>
	/// Builds the image-table metadata. Dimensions are the trimmed (non-transparent bounding-box) size,
	/// matching the PNGs produced by <see cref="RenderFramePngAsync"/>: fully transparent frames report 1×1.
	/// </summary>
	public static ObjectImageMetadata BuildMetadata(IReadOnlyList<GraphicsElement> elements)
	{
		var frames = new List<ObjectImageFrameInfo>(elements.Count);
		for (var i = 0; i < elements.Count; i++)
		{
			var element = elements[i];
			var crop = element.Indexed != null
				? GraphicsElementOperations.FindCropRegion(element.Indexed)
				: element.Rgba != null
					? GraphicsElementOperations.FindCropRegion(element.Rgba)
					: new Rectangle(0, 0, 1, 1);

			// A fully transparent frame trims to a single pixel (see GraphicsElement.TrimImage).
			var width = crop.Width > 0 && crop.Height > 0 ? crop.Width : 1;
			var height = crop.Width > 0 && crop.Height > 0 ? crop.Height : 1;
			frames.Add(new ObjectImageFrameInfo(i, width, height));
		}

		return new ObjectImageMetadata(elements.Count, frames);
	}

	/// <summary>Renders one frame to a trimmed PNG, decoding to RGBA only for palette-indexed frames.</summary>
	public static async Task<byte[]> RenderFramePngAsync(GraphicsElement element, CancellationToken ct)
	{
		var palette = PaletteMapLoader.LoadDefault();
		using var image = element.TrimmedToRgba(palette);
		using var ms = new MemoryStream();
		await image.SaveAsPngAsync(ms, ct);
		return ms.ToArray();
	}

	/// <summary>
	/// Warms the cache for an already-decoded object: stores the image-table metadata (cheap) and the
	/// first frame (the browse-page thumbnail). Remaining frames are rendered lazily on first request.
	/// Never throws - a failure to warm simply leaves the cache cold for that object.
	/// </summary>
	public static async Task WarmAsync(IObjectImageCache cache, ulong xxHash3, IReadOnlyList<GraphicsElement> elements, CancellationToken ct)
	{
		if (elements.Count == 0)
		{
			return;
		}

		try
		{
			await cache.SetMetadataAsync(xxHash3, BuildMetadata(elements), ct);

			if (elements[0] != null)
			{
				var png = await RenderFramePngAsync(elements[0], ct);
				await cache.SetPngAsync(xxHash3, 0, png, ct);
			}
		}
		catch (Exception ex) when (ex is not OperationCanceledException)
		{
			// Warming is best-effort: a malformed frame must never fail the import/upload that triggered it.
			System.Diagnostics.Debug.WriteLine($"Image cache warm failed for {xxHash3:x16}: {ex.Message}");
		}
	}
}
