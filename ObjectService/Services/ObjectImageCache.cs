using Microsoft.Extensions.Caching.Memory;
using System.Text.Json;

namespace ObjectService.Services;

/// <summary>A single rendered frame of an object's image table.</summary>
public sealed record ObjectImageFrameInfo(int Index, int Width, int Height);

/// <summary>The rendered image table metadata for one object file.</summary>
public sealed record ObjectImageMetadata(int Count, IReadOnlyList<ObjectImageFrameInfo> Frames);

/// <summary>
/// Cache for rendered object images. Entries are keyed by the object DAT file's <c>xxHash3</c> - the
/// authoritative, immutable content hash - so a cached image can never go stale: a re-uploaded or
/// edited file has a different hash and therefore a different cache key. Two objects built from the
/// same file share one cache entry, and no explicit invalidation is ever required.
/// <para>
/// Two tiers are used: an in-process <see cref="IMemoryCache"/> (hot, bounded by size) in front of a
/// disk folder under <c>GameData/Cache/images</c> (survives restarts, no RAM cost). Every entry is
/// regenerable from the source DAT, so the disk folder is safe to delete at any time.
/// </para>
/// </summary>
public interface IObjectImageCache
{
	/// <summary>Returns the rendered PNG for one frame, or <see langword="null"/> when it is not cached.</summary>
	Task<byte[]?> GetPngAsync(ulong xxHash3, int imageId, CancellationToken ct);

	/// <summary>Stores the rendered PNG for one frame in the memory and disk tiers.</summary>
	Task SetPngAsync(ulong xxHash3, int imageId, byte[] png, CancellationToken ct);

	/// <summary>Returns the image-table metadata (frame count and per-frame dimensions), or <see langword="null"/>.</summary>
	Task<ObjectImageMetadata?> GetMetadataAsync(ulong xxHash3, CancellationToken ct);

	/// <summary>Stores the image-table metadata.</summary>
	Task SetMetadataAsync(ulong xxHash3, ObjectImageMetadata metadata, CancellationToken ct);

	/// <summary>Returns the whole image table as a zip, or <see langword="null"/> when it is not cached.</summary>
	Task<byte[]?> GetZipAsync(ulong xxHash3, CancellationToken ct);

	/// <summary>Stores the whole image table as a zip.</summary>
	Task SetZipAsync(ulong xxHash3, byte[] zip, CancellationToken ct);
}

/// <inheritdoc />
public sealed class ObjectImageCache : IObjectImageCache
{
	const string MetadataFileName = "meta.json";
	const string ZipFileName = "images.zip";

	readonly ServerFolderManager _sfm;
	readonly IMemoryCache _memory;
	readonly ILogger<ObjectImageCache> _logger;

	public ObjectImageCache(ServerFolderManager sfm, IMemoryCache memory, ILogger<ObjectImageCache> logger)
	{
		_sfm = sfm;
		_memory = memory;
		_logger = logger;
	}

	public Task<byte[]?> GetPngAsync(ulong xxHash3, int imageId, CancellationToken ct)
		=> GetBinaryAsync(Key("img", xxHash3, imageId), PngPath(xxHash3, imageId), ct);

	public Task SetPngAsync(ulong xxHash3, int imageId, byte[] png, CancellationToken ct)
		=> SetBinaryAsync(Key("img", xxHash3, imageId), PngPath(xxHash3, imageId), png, ct);

	public Task<byte[]?> GetZipAsync(ulong xxHash3, CancellationToken ct)
		=> GetBinaryAsync(Key("zip", xxHash3), ZipPath(xxHash3), ct);

	public Task SetZipAsync(ulong xxHash3, byte[] zip, CancellationToken ct)
		=> SetBinaryAsync(Key("zip", xxHash3), ZipPath(xxHash3), zip, ct);

	public async Task<ObjectImageMetadata?> GetMetadataAsync(ulong xxHash3, CancellationToken ct)
	{
		var key = Key("meta", xxHash3);
		if (_memory.TryGetValue<ObjectImageMetadata>(key, out var cached) && cached != null)
		{
			return cached;
		}

		var path = MetadataPath(xxHash3);
		if (!File.Exists(path))
		{
			return null;
		}

		try
		{
			await using var stream = File.OpenRead(path);
			var metadata = await JsonSerializer.DeserializeAsync<ObjectImageMetadata>(stream, cancellationToken: ct).ConfigureAwait(false);
			if (metadata != null)
			{
				SetMemory(key, metadata, EstimateMetadataSize(metadata));
			}

			return metadata;
		}
		catch (Exception ex) when (ex is IOException or UnauthorizedAccessException or JsonException)
		{
			_logger.LogWarning(ex, "Could not read cached image metadata at {Path}", path);
			return null;
		}
	}

	public async Task SetMetadataAsync(ulong xxHash3, ObjectImageMetadata metadata, CancellationToken ct)
	{
		var key = Key("meta", xxHash3);
		SetMemory(key, metadata, EstimateMetadataSize(metadata));

		var path = MetadataPath(xxHash3);
		try
		{
			_ = Directory.CreateDirectory(Path.GetDirectoryName(path)!);
			// Write to a temp file then move, so a concurrent reader never sees a half-written file.
			var temp = path + ".tmp";
			await using (var stream = File.Create(temp))
			{
				await JsonSerializer.SerializeAsync(stream, metadata, cancellationToken: ct).ConfigureAwait(false);
			}

			File.Move(temp, path, overwrite: true);
		}
		catch (Exception ex) when (ex is IOException or UnauthorizedAccessException)
		{
			_logger.LogWarning(ex, "Could not write cached image metadata to {Path}", path);
		}
	}

	async Task<byte[]?> GetBinaryAsync(string memoryKey, string path, CancellationToken ct)
	{
		if (_memory.TryGetValue<byte[]>(memoryKey, out var cached) && cached != null)
		{
			return cached;
		}

		if (!File.Exists(path))
		{
			return null;
		}

		try
		{
			var bytes = await File.ReadAllBytesAsync(path, ct).ConfigureAwait(false);
			SetMemory(memoryKey, bytes, bytes.Length);
			return bytes;
		}
		catch (Exception ex) when (ex is IOException or UnauthorizedAccessException)
		{
			_logger.LogWarning(ex, "Could not read cached image at {Path}", path);
			return null;
		}
	}

	async Task SetBinaryAsync(string memoryKey, string path, byte[] bytes, CancellationToken ct)
	{
		SetMemory(memoryKey, bytes, bytes.Length);

		try
		{
			_ = Directory.CreateDirectory(Path.GetDirectoryName(path)!);
			// Write to a temp file then move, so a concurrent reader never sees a half-written file.
			var temp = path + ".tmp";
			await File.WriteAllBytesAsync(temp, bytes, ct).ConfigureAwait(false);
			File.Move(temp, path, overwrite: true);
		}
		catch (Exception ex) when (ex is IOException or UnauthorizedAccessException)
		{
			_logger.LogWarning(ex, "Could not write cached image to {Path}", path);
		}
	}

	void SetMemory(string key, object value, long size)
		=> _memory.Set(key, value, new MemoryCacheEntryOptions { Size = Math.Max(1, size) });

	static long EstimateMetadataSize(ObjectImageMetadata metadata) => 64 + (metadata.Frames.Count * 16L);

	static string Key(string prefix, ulong xxHash3, int imageId = 0)
		=> $"{prefix}:{xxHash3:x16}:{imageId}";

	string PngPath(ulong xxHash3, int imageId) => Path.Combine(_sfm.GetObjectImagesCacheFolder(xxHash3), $"{imageId}.png");

	string MetadataPath(ulong xxHash3) => Path.Combine(_sfm.GetObjectImagesCacheFolder(xxHash3), MetadataFileName);

	string ZipPath(ulong xxHash3) => Path.Combine(_sfm.GetObjectImagesCacheFolder(xxHash3), ZipFileName);
}
