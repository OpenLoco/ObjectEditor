using Definitions;
using Definitions.Database;
using Definitions.ObjectModels.Types;
using Microsoft.EntityFrameworkCore;
using ObjectService.RouteHandlers;

namespace ObjectService.Services;

/// <summary>
/// Shared resolution of an object's primary DAT file. Loads the header row and its primary
/// <see cref="TblDatObject"/> (enforcing the automatic image/file exposure rules - vanilla Locomotion
/// content and unavailable objects are <see cref="ObjectResourceOutcome.Forbidden"/>) and maps the
/// object index entry to a safe on-disk path. Used by both <see cref="ObjectQueryService"/> (file
/// download) and <see cref="ObjectImageService"/> (rendered images), which is why it is not a member
/// of either service.
/// </summary>
internal static class ObjectResourceResolver
{
	/// <summary>
	/// Loads the object row and its primary DAT mapping, or the reason it cannot be exposed.
	/// </summary>
	public static async Task<(TblObject? Obj, TblDatObject? Dat, ObjectResourceOutcome Outcome)> ResolvePrimaryDatAsync(LocoDbContext db, UniqueObjectId id, CancellationToken ct)
	{
		var obj = await db.Objects
			.AsNoTracking()
			.Include(x => x.DatObjects)
			.SingleOrDefaultAsync(x => x.Id == id, ct)
			.ConfigureAwait(false);

		if (obj == null)
		{
			return (null, null, ObjectResourceOutcome.NotFound);
		}

		if (!ObjectAvailabilityRules.IsDownloadable(obj.ObjectSource, obj.Availability))
		{
			return (obj, null, ObjectResourceOutcome.Forbidden);
		}

		return (obj, obj.DatObjects.FirstOrDefault(), ObjectResourceOutcome.Ok);
	}

	/// <summary>
	/// Resolves the on-disk path of an object's primary DAT file via the object index. The path is
	/// validated to stay under the Objects folder, since the index can hold either a relative or an
	/// absolute filename.
	/// </summary>
	public static bool TryResolveObjectFilePath(ServerFolderManager sfm, TblDatObject dat, out string path)
	{
		path = string.Empty;
		if (!sfm.ObjectIndex.TryFind((dat.DatName, dat.DatChecksum), out var entry)
			|| entry == null
			|| string.IsNullOrEmpty(entry.FileName)
			|| !RouteHelpers.TryGetSafePathUnderRoot(sfm.ObjectsFolder, entry.FileName, out path, out _))
		{
			return false;
		}

		return File.Exists(path);
	}
}
