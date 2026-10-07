using Definitions.ObjectModels.Graphics;
using NUnit.Framework;
using ObjectService.Services;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.PixelFormats;

namespace Tests;

[TestFixture]
public class ObjectImageRenderTests
{
	/// <summary>10×10 image with a 2px transparent border, so its opaque core is 6×6.</summary>
	static GraphicsElement BorderedElement(byte r, byte g, byte b)
	{
		var image = new Image<Rgba32>(10, 10);
		for (var y = 2; y < 8; y++)
		{
			for (var x = 2; x < 8; x++)
			{
				image[x, y] = new Rgba32(r, g, b, 255);
			}
		}

		// FromRgba takes ownership of the image; disposing the element disposes the pixels.
		return GraphicsElement.FromRgba(GraphicsElementFlags.IsBgr24, image);
	}

	[Test]
	public void BuildMetadata_ReportsTrimmedDimensions()
	{
		using var element = BorderedElement(255, 0, 0);

		var metadata = ObjectImageRender.BuildMetadata([element]);

		Assert.That(metadata.Count, Is.EqualTo(1));
		Assert.That(metadata.Frames[0].Index, Is.EqualTo(0));
		Assert.That(metadata.Frames[0].Width, Is.EqualTo(6));
		Assert.That(metadata.Frames[0].Height, Is.EqualTo(6));
	}

	[Test]
	public void BuildMetadata_FullyTransparentFrame_IsOneByOne()
	{
		var image = new Image<Rgba32>(4, 4);
		using var element = GraphicsElement.FromRgba(GraphicsElementFlags.IsBgr24, image);

		var metadata = ObjectImageRender.BuildMetadata([element]);

		Assert.That(metadata.Frames[0].Width, Is.EqualTo(1));
		Assert.That(metadata.Frames[0].Height, Is.EqualTo(1));
	}

	[Test]
	public async Task RenderFramePngAsync_DimensionsMatchMetadata()
	{
		using var element = BorderedElement(0, 255, 0);
		var metadata = ObjectImageRender.BuildMetadata([element]);

		var png = await ObjectImageRender.RenderFramePngAsync(element, CancellationToken.None);

		using var decoded = Image.Load(png);
		Assert.That(decoded.Width, Is.EqualTo(metadata.Frames[0].Width));
		Assert.That(decoded.Height, Is.EqualTo(metadata.Frames[0].Height));
	}
}
