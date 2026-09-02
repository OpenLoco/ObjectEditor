using System.IO.Compression;
using Common;
using Definitions;
using Definitions.Database;
using Definitions.DTO;
using Definitions.DTO.Mappers;
using Definitions.ObjectModels.Types;
using Definitions.SourceData;
using Microsoft.EntityFrameworkCore;
using ObjectService.RouteHandlers;

namespace ObjectService.Services;

public interface IObjectPackService
{
	Task<IEnumerable<DtoItemPackEntry>> ListPacksAsync(CancellationToken ct);
	Task<IEnumerable<DtoItemPackDescriptor<DtoObjectEntry>>> GetPackAsync(UniqueObjectId id, CancellationToken ct);
	Task<(Stream? Stream, string FileName)> GetPackFileAsync(UniqueObjectId id, CancellationToken ct);
}

public class ObjectPackService : IObjectPackService
{
	private readonly LocoDbContext _db;
	private readonly ServerFolderManager _sfm;

	public ObjectPackService(LocoDbContext db, ServerFolderManager sfm)
	{
		_db = db;
		_sfm = sfm;
	}

	public async Task<IEnumerable<DtoItemPackEntry>> ListPacksAsync(CancellationToken ct)
	{
		var packs = await _db.ObjectPacks.Include(l => l.Licence).ToListAsync(ct);
		return packs.Select(x => x.ToDtoEntry()).OrderBy(x => x.Name);
	}

	public async Task<IEnumerable<DtoItemPackDescriptor<DtoObjectEntry>>> GetPackAsync(UniqueObjectId id, CancellationToken ct)
	{
		var packs = await _db.ObjectPacks.Where(x => x.Id == id).Include(l => l.Licence).Select(x => new ExpandedTblPack<TblObjectPack, TblObject>(x, x.Objects, x.Authors, x.Tags)).ToListAsync(ct);
		return packs.Select(x => x.ToDtoDescriptor()).OrderBy(x => x.Name);
	}

	public async Task<(Stream? Stream, string FileName)> GetPackFileAsync(UniqueObjectId id, CancellationToken ct)
	{
		var pack = await _db.ObjectPacks.Where(x => x.Id == id).Include(x => x.Objects).ThenInclude(o => o.DatObjects).SingleOrDefaultAsync(ct);
		if (pack == null)
		{
			return (null, string.Empty);
		}

		var path = Path.Combine(Path.GetTempPath(), $"{Guid.NewGuid():N}.zip");
		var zipStream = new FileStream(path, FileMode.Create, FileAccess.ReadWrite, FileShare.None, 4096, FileOptions.Asynchronous | FileOptions.DeleteOnClose);
		using (var archive = new ZipArchive(zipStream, ZipArchiveMode.Create, leaveOpen: true))
		{
			foreach (var obj in pack.Objects)
			{
				if (obj.ObjectSource is ObjectSource.LocomotionGoG or ObjectSource.LocomotionSteam)
				{
					continue;
				}

				if (obj.Availability == ObjectAvailability.Unavailable)
				{
					continue;
				}

				foreach (var dat in obj.DatObjects)
				{
					if (!_sfm.ObjectIndex.TryFind((dat.DatName, dat.DatChecksum), out var entry) || entry == null)
					{
						continue;
					}

					if (!RouteHelpers.TryGetSafeRelativePathUnderRoot(_sfm.ObjectsFolder, entry.FileName, out var fullPath, out var entryName))
					{
						continue;
					}

					if (!File.Exists(fullPath))
					{
						continue;
					}

					await using var fs = File.OpenRead(fullPath);
					var ze = archive.CreateEntry(entryName, CompressionLevel.Optimal);
					await using var es = ze.Open();
					await fs.CopyToAsync(es, ct);
				}
			}
		}
		zipStream.Position = 0;
		var dn = DownloadNameHelper.MakeSafeDownloadFileName(pack.Name, ".zip", "object-pack");
		return (zipStream, dn);
	}
}