using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging.Abstractions;
using NUnit.Framework;
using ObjectService;
using ObjectService.Services;

namespace Tests;

[TestFixture]
public class ObjectImageCacheTests
{
	static ServerFolderManager NewFolderManager(out string root)
	{
		root = Directory.CreateTempSubdirectory("ObjectImageCacheTest").FullName;
		return new ServerFolderManager(root);
	}

	static ObjectImageCache NewCache(ServerFolderManager sfm, IMemoryCache memory)
		=> new(sfm, memory, NullLogger<ObjectImageCache>.Instance);

	[Test]
	public async Task SetPngAsync_ThenGetPngAsync_RoundTrips()
	{
		var sfm = NewFolderManager(out _);
		using var memory = new MemoryCache(new MemoryCacheOptions { SizeLimit = 1024 * 1024 });
		var cache = NewCache(sfm, memory);
		var png = new byte[] { 1, 2, 3, 4, 5 };

		await cache.SetPngAsync(0xABCDEF, 7, png, CancellationToken.None);
		var result = await cache.GetPngAsync(0xABCDEF, 7, CancellationToken.None);

		Assert.That(result, Is.EqualTo(png));
	}

	[Test]
	public async Task GetPngAsync_ReadsFromDisk_AfterMemoryIsCleared()
	{
		var sfm = NewFolderManager(out _);
		var png = new byte[] { 9, 8, 7 };

		using (var memory = new MemoryCache(new MemoryCacheOptions { SizeLimit = 1024 * 1024 }))
		{
			var cache = NewCache(sfm, memory);
			await cache.SetPngAsync(0x1111, 0, png, CancellationToken.None);
		}

		// Fresh, empty memory cache - the value must come from the disk tier.
		using var freshMemory = new MemoryCache(new MemoryCacheOptions { SizeLimit = 1024 * 1024 });
		var freshCache = NewCache(sfm, freshMemory);
		var result = await freshCache.GetPngAsync(0x1111, 0, CancellationToken.None);

		Assert.That(result, Is.EqualTo(png));
	}

	[Test]
	public async Task GetPngAsync_ReturnsNull_ForUnknownEntry()
	{
		var sfm = NewFolderManager(out _);
		using var memory = new MemoryCache(new MemoryCacheOptions { SizeLimit = 1024 * 1024 });
		var cache = NewCache(sfm, memory);

		Assert.That(await cache.GetPngAsync(0x42, 0, CancellationToken.None), Is.Null);
	}

	[Test]
	public async Task ContentAddressed_ByHash_SoDifferentHashesDoNotCollide()
	{
		var sfm = NewFolderManager(out _);
		using var memory = new MemoryCache(new MemoryCacheOptions { SizeLimit = 1024 * 1024 });
		var cache = NewCache(sfm, memory);

		await cache.SetPngAsync(0x1, 0, [1], CancellationToken.None);
		await cache.SetPngAsync(0x2, 0, [2], CancellationToken.None);

		var first = await cache.GetPngAsync(0x1, 0, CancellationToken.None);
		var second = await cache.GetPngAsync(0x2, 0, CancellationToken.None);

		Assert.That(first, Is.EqualTo(new byte[] { 1 }));
		Assert.That(second, Is.EqualTo(new byte[] { 2 }));
	}

	[Test]
	public async Task Metadata_RoundTrips()
	{
		var sfm = NewFolderManager(out _);
		using var memory = new MemoryCache(new MemoryCacheOptions { SizeLimit = 1024 * 1024 });
		var cache = NewCache(sfm, memory);
		var metadata = new ObjectImageMetadata(2, [new ObjectImageFrameInfo(0, 4, 5), new ObjectImageFrameInfo(1, 1, 1)]);

		await cache.SetMetadataAsync(0x99, metadata, CancellationToken.None);
		var result = await cache.GetMetadataAsync(0x99, CancellationToken.None);

		Assert.That(result, Is.Not.Null);
		Assert.That(result!.Count, Is.EqualTo(2));
		Assert.That(result.Frames[1], Is.EqualTo(new ObjectImageFrameInfo(1, 1, 1)));
	}

	[Test]
	public async Task Zip_RoundTrips()
	{
		var sfm = NewFolderManager(out _);
		using var memory = new MemoryCache(new MemoryCacheOptions { SizeLimit = 1024 * 1024 });
		var cache = NewCache(sfm, memory);
		var zip = new byte[] { 80, 75, 3, 4 };

		await cache.SetZipAsync(0x55, zip, CancellationToken.None);

		Assert.That(await cache.GetZipAsync(0x55, CancellationToken.None), Is.EqualTo(zip));
	}

	[Test]
	public async Task SetPngAsync_WritesPngFileUnderHashFolder()
	{
		var sfm = NewFolderManager(out _);
		using var memory = new MemoryCache(new MemoryCacheOptions { SizeLimit = 1024 * 1024 });
		var cache = NewCache(sfm, memory);

		await cache.SetPngAsync(0x77, 3, [1, 2, 3], CancellationToken.None);

		Assert.That(File.Exists(Path.Combine(sfm.GetObjectImagesCacheFolder(0x77), "3.png")), Is.True);
	}
}
