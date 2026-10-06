using Dat.FileParsing;
using Definitions;
using Definitions.Database;
using Definitions.ObjectModels.Graphics;
using SixLabors.ImageSharp;
using System.IO.Compression;

namespace ObjectService.Services;

/// <summary>
/// Read-only access to an object's rendered image table (individual frames, the whole table as a zip,
/// and the table metadata). Split out of <see cref="ObjectQueryService"/> so the object CRUD service
/// is not also responsible for image rendering and caching.
/// </summary>
public interface IObjectImageService
{
	/// <summary>The whole image table as a zip, or the reason it cannot be exposed.</summary>
	Task<ObjectImageResult> GetImagesZipAsync(UniqueObjectId id, CancellationToken ct);

	/// <summary>One rendered image (frame), or the reason it cannot be exposed.</summary>
	Task<ObjectImageResult> GetImagePngAsync(UniqueObjectId id, int imageId, CancellationToken ct);

	/// <summary>The image-table metadata (frame count and dimensions), or the reason it cannot be exposed.</summary>
	Task<ObjectImageMetadataResult> GetImageMetadataAsync(UniqueObjectId id, CancellationToken ct);
}

/// <inheritdoc />
public sealed class ObjectImageService : IObjectImageService
{
	private readonly LocoDbContext _db;
	private readonly ServerFolderManager _sfm;
	private readonly ILogger<ObjectImageService> _logger;
	private readonly IObjectImageCache _imageCache;

	public ObjectImageService(LocoDbContext db, ServerFolderManager sfm, ILogger<ObjectImageService> logger, IObjectImageCache imageCache)
	{
		_db = db;
		_sfm = sfm;
		_logger = logger;
		_imageCache = imageCache;
	}

	public async Task<ObjectImageResult> GetImagesZipAsync(UniqueObjectId id, CancellationToken ct)
	{
		var (_, dat, outcome) = await ObjectResourceResolver.ResolvePrimaryDatAsync(_db, id, ct).ConfigureAwait(false);
		if (outcome != ObjectResourceOutcome.Ok)
		{
			return new(outcome);
		}

		if (dat == null)
		{
			return new(ObjectResourceOutcome.NotFound);
		}

		var cached = await _imageCache.GetZipAsync(dat.xxHash3, ct).ConfigureAwait(false);
		if (cached != null)
		{
			return new(ObjectResourceOutcome.Ok, cached, dat.xxHash3);
		}

		if (!ObjectResourceResolver.TryResolveObjectFilePath(_sfm, dat, out var objectFilePath))
		{
			return new(ObjectResourceOutcome.NotFound);
		}

		var elements = await LoadImageElementsAsync(objectFilePath, ct).ConfigureAwait(false);
		if (elements == null)
		{
			return new(ObjectResourceOutcome.NotFound);
		}

		var palette = PaletteMapLoader.LoadDefault();

		// Build the zip in memory so the bytes can be both returned and cached in one pass.
		using var ms = new MemoryStream();
		using (var archive = new ZipArchive(ms, ZipArchiveMode.Create, leaveOpen: true))
		{
			for (var i = 0; i < elements.Count; i++)
			{
				if (elements[i] is not { } element)
				{
					continue;
				}

				var entry = archive.CreateEntry(i + ".png", CompressionLevel.Optimal);
				await using var entryStream = entry.Open();
				await element.ToRgba(palette).SaveAsPngAsync(entryStream, ct).ConfigureAwait(false);
			}
		}

		var zip = ms.ToArray();
		await _imageCache.SetZipAsync(dat.xxHash3, zip, ct).ConfigureAwait(false);
		return new(ObjectResourceOutcome.Ok, zip, dat.xxHash3);
	}

	public async Task<ObjectImageResult> GetImagePngAsync(UniqueObjectId id, int imageId, CancellationToken ct)
	{
		var (_, dat, outcome) = await ObjectResourceResolver.ResolvePrimaryDatAsync(_db, id, ct).ConfigureAwait(false);
		if (outcome != ObjectResourceOutcome.Ok)
		{
			return new(outcome);
		}

		if (dat == null)
		{
			return new(ObjectResourceOutcome.NotFound);
		}

		var cached = await _imageCache.GetPngAsync(dat.xxHash3, imageId, ct).ConfigureAwait(false);
		if (cached != null)
		{
			return new(ObjectResourceOutcome.Ok, cached, dat.xxHash3);
		}

		if (!ObjectResourceResolver.TryResolveObjectFilePath(_sfm, dat, out var objectFilePath))
		{
			return new(ObjectResourceOutcome.NotFound);
		}

		var elements = await LoadImageElementsAsync(objectFilePath, ct).ConfigureAwait(false);
		if (elements == null || imageId < 0 || imageId >= elements.Count || elements[imageId] is null)
		{
			return new(ObjectResourceOutcome.NotFound);
		}

		var png = await ObjectImageRender.RenderFramePngAsync(elements[imageId], ct).ConfigureAwait(false);
		await _imageCache.SetPngAsync(dat.xxHash3, imageId, png, ct).ConfigureAwait(false);
		return new(ObjectResourceOutcome.Ok, png, dat.xxHash3);
	}

	public async Task<ObjectImageMetadataResult> GetImageMetadataAsync(UniqueObjectId id, CancellationToken ct)
	{
		var (_, dat, outcome) = await ObjectResourceResolver.ResolvePrimaryDatAsync(_db, id, ct).ConfigureAwait(false);
		if (outcome != ObjectResourceOutcome.Ok)
		{
			return new(outcome);
		}

		if (dat == null)
		{
			return new(ObjectResourceOutcome.NotFound);
		}

		var cached = await _imageCache.GetMetadataAsync(dat.xxHash3, ct).ConfigureAwait(false);
		if (cached != null)
		{
			return new(ObjectResourceOutcome.Ok, cached, dat.xxHash3);
		}

		if (!ObjectResourceResolver.TryResolveObjectFilePath(_sfm, dat, out var objectFilePath))
		{
			return new(ObjectResourceOutcome.NotFound);
		}

		var elements = await LoadImageElementsAsync(objectFilePath, ct).ConfigureAwait(false);
		if (elements == null)
		{
			return new(ObjectResourceOutcome.NotFound);
		}

		var metadata = ObjectImageRender.BuildMetadata(elements);
		await _imageCache.SetMetadataAsync(dat.xxHash3, metadata, ct).ConfigureAwait(false);
		return new(ObjectResourceOutcome.Ok, metadata, dat.xxHash3);
	}

	/// <summary>Full-loads a DAT file and returns its image-table elements, or <see langword="null"/> when it has no image table.</summary>
	private async Task<IReadOnlyList<GraphicsElement>?> LoadImageElementsAsync(string objectFilePath, CancellationToken ct)
	{
		byte[] datBytes;
		try
		{
			datBytes = await File.ReadAllBytesAsync(objectFilePath, ct).ConfigureAwait(false);
		}
		catch (Exception ex) when (ex is IOException or UnauthorizedAccessException)
		{
			_logger.LogWarning(ex, "Could not read object file {Path}", objectFilePath);
			return null;
		}

		var result = SawyerStreamReader.LoadFullObject(datBytes, _logger);
		return result.LocoObject?.ImageTable?.GraphicsElements;
	}
}
