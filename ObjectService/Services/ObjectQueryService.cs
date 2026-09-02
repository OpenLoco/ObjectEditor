using Dat.FileParsing;
using Definitions.Database;
using Definitions.DTO;
using Definitions.DTO.Mappers;
using Definitions.SourceData;
using Microsoft.EntityFrameworkCore;
using SixLabors.ImageSharp;
using System.IO.Compression;

namespace ObjectService.Services;

public interface IObjectQueryService
{
	Task<IEnumerable<DtoObjectEntry>> ListAsync(HttpContext context, CancellationToken ct);
	Task<DtoObjectPostResponse?> GetByIdAsync(UniqueObjectId id, CancellationToken ct);
	Task<DtoObjectPostResponse?> UpdateAsync(UniqueObjectId id, DtoObjectPostResponse request, CancellationToken ct);
	Task<byte[]?> GetImagesZipAsync(UniqueObjectId id, CancellationToken ct);
	Task<string?> GetFilePathAsync(UniqueObjectId id, CancellationToken ct);
}

public class ObjectQueryService : IObjectQueryService
{
	private readonly LocoDbContext _db;
	private readonly ServerFolderManager _sfm;
	private readonly ILogger<ObjectQueryService> _logger;

	public ObjectQueryService(LocoDbContext db, ServerFolderManager sfm, ILogger<ObjectQueryService> logger)
	{
		_db = db;
		_sfm = sfm;
		_logger = logger;
	}

	public async Task<IEnumerable<DtoObjectEntry>> ListAsync(HttpContext context, CancellationToken ct)
	{
		return await _db.Objects.Include(x => x.DatObjects).Select(x => x.ToDtoEntry()).ToListAsync(ct);
	}

	public async Task<DtoObjectPostResponse?> GetByIdAsync(UniqueObjectId id, CancellationToken ct)
	{
		var eObj = await _db.Objects.Where(x => x.Id == id).Include(x => x.Licence).Include(x => x.DatObjects).Include(x => x.StringTable).Select(x => new ExpandedTbl<TblObject, TblObjectPack>(x, x.Authors, x.Tags, x.ObjectPacks)).SingleOrDefaultAsync(ct);
		return eObj?.ToDtoDescriptor();
	}

	public async Task<DtoObjectPostResponse?> UpdateAsync(UniqueObjectId id, DtoObjectPostResponse request, CancellationToken ct)
	{
		var obj = await _db.Objects.Include(x => x.Licence).Include(x => x.Authors).Include(x => x.Tags).Include(x => x.ObjectPacks).Include(x => x.DatObjects).Include(x => x.StringTable).Where(x => x.Id == id).SingleOrDefaultAsync(ct);
		if (obj == null)
		{
			return null;
		}

		obj.Description = request.Description;
		obj.CreatedDate = request.CreatedDate;
		obj.ModifiedDate = request.ModifiedDate;
		obj.Availability = request.Availability;
		if (request.Licence == null)
		{
			obj.Licence = null;
		}
		else
		{
			obj.Licence = await _db.Licences.SingleOrDefaultAsync(l => l.Id == request.Licence.Id, ct);
		}

		if (request.Authors == null || request.Authors.Count == 0)
		{
			obj.Authors.Clear();
		}
		else
		{
			var ids = request.Authors.Select(a => a.Id).ToList();
			var items = await _db.Authors.Where(x => ids.Contains(x.Id)).ToListAsync(ct);
			obj.Authors.Clear();
			foreach (var i in items)
			{
				obj.Authors.Add(i);
			}
		}

		if (request.Tags == null || request.Tags.Count == 0)
		{
			obj.Tags.Clear();
		}
		else
		{
			var ids = request.Tags.Select(t => t.Id).ToList();
			var items = await _db.Tags.Where(x => ids.Contains(x.Id)).ToListAsync(ct);
			obj.Tags.Clear();
			foreach (var i in items)
			{
				obj.Tags.Add(i);
			}
		}

		if (request.ObjectPacks == null || request.ObjectPacks.Count == 0)
		{
			obj.ObjectPacks.Clear();
		}
		else
		{
			var ids = request.ObjectPacks.Select(p => p.Id).ToList();
			var items = await _db.ObjectPacks.Where(x => ids.Contains(x.Id)).ToListAsync(ct);
			obj.ObjectPacks.Clear();
			foreach (var i in items)
			{
				obj.ObjectPacks.Add(i);
			}
		}

		_ = await _db.SaveChangesAsync(ct);
		var expandedObj = new ExpandedTbl<TblObject, TblObjectPack>(obj, obj.Authors, obj.Tags, obj.ObjectPacks);
		return expandedObj.ToDtoDescriptor();
	}

	public async Task<byte[]?> GetImagesZipAsync(UniqueObjectId id, CancellationToken ct)
	{
		var obj = await _db.Objects.AsNoTracking().Include(x => x.DatObjects).SingleOrDefaultAsync(x => x.Id == id, ct);
		if (obj == null)
		{
			return null;
		}

		var datEntry = obj.DatObjects.FirstOrDefault();
		if (datEntry == null || !_sfm.ObjectIndex.TryFind((datEntry.DatName, datEntry.DatChecksum), out var indexEntry) || indexEntry == null || string.IsNullOrEmpty(indexEntry.FileName))
		{
			return null;
		}

		var objectFilePath = Path.Combine(_sfm.ObjectsFolder, indexEntry.FileName);
		if (!File.Exists(objectFilePath))
		{
			return null;
		}

		var datBytes = await File.ReadAllBytesAsync(objectFilePath, ct);
		var result = SawyerStreamReader.LoadFullObject(datBytes, _logger);
		if (result.LocoObject?.ImageTable == null)
		{
			return null;
		}

		var elements = result.LocoObject.ImageTable.GraphicsElements;
		var tempZipPath = Path.Combine(Path.GetTempPath(), Guid.NewGuid().ToString("N") + ".zip");
		using var zipStream = new FileStream(tempZipPath, FileMode.Create, FileAccess.ReadWrite, FileShare.None, 4096, FileOptions.Asynchronous | FileOptions.DeleteOnClose);
		using (var archive = new ZipArchive(zipStream, ZipArchiveMode.Create, leaveOpen: true))
		{
			for (int i = 0; i < elements.Count; i++)
			{
				var element = elements[i];
				if (element.Image == null)
				{
					continue;
				}

				var entry = archive.CreateEntry(i + ".png", CompressionLevel.Optimal);
				await using var entryStream = entry.Open();
				await element.Image.SaveAsPngAsync(entryStream, ct);
			}
		}
		zipStream.Position = 0;
		using var ms = new MemoryStream();
		await zipStream.CopyToAsync(ms, ct);
		return ms.ToArray();
	}

	public async Task<string?> GetFilePathAsync(UniqueObjectId id, CancellationToken ct)
	{
		var obj = await _db.Objects.Include(x => x.DatObjects).Where(x => x.Id == id).SingleOrDefaultAsync(ct);
		if (obj == null)
		{
			return null;
		}

		var dat = obj.DatObjects.First();
		if (!_sfm.ObjectIndex.TryFind((dat.DatName, dat.DatChecksum), out var entry) || entry == null)
		{
			return null;
		}

		if (string.IsNullOrEmpty(entry.FileName))
		{
			return null;
		}

		return Path.Combine(_sfm.ObjectsFolder, entry.FileName);
	}
}