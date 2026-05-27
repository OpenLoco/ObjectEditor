using System.Collections.ObjectModel;
using Definitions.Database;
using Microsoft.EntityFrameworkCore;

namespace Definitions;

// Thin in-memory container of ObjectIndexEntry projections. The local client
// builds this from the SQLite DB (LocalObjectIndexService) and the server builds
// it from its own DB. Kept as a class so existing UI bindings on Objects remain valid.
public class ObjectIndex
{
	public ObservableCollection<ObjectIndexEntry> Objects { get; init; } = [];

	public ObjectIndex()
	{ }

	public ObjectIndex(IEnumerable<ObjectIndexEntry> objects)
		=> Objects = [.. objects];

	public bool TryFind((string datName, uint datChecksum) key, out ObjectIndexEntry? entry)
	{
		entry = Objects.FirstOrDefault(x => x.DisplayName == key.datName && x.DatChecksum == key.datChecksum);
		return entry != null;
	}

	public bool TryFind(ulong xxHash3, out ObjectIndexEntry? entry)
	{
		entry = Objects.FirstOrDefault(x => x.xxHash3 == xxHash3);
		return entry != null;
	}

	public void Delete(Func<ObjectIndexEntry, bool> predicate)
	{
		foreach (var d in Objects.Where(predicate).ToList())
		{
			_ = Objects.Remove(d);
		}
	}

	// Synchronous projection from a LocoDbContext. Used by tooling that already
	// owns a context (DatabaseTools, DataQuery, tests). UI code should prefer
	// LocalObjectIndexService.BuildObjectIndexAsync().
	public static ObjectIndex FromDb(LocoDbContext db)
	{
		var rows = db.DatObjects.AsNoTracking()
			.Include(d => d.Object)
			.Select(d => new ObjectIndexEntry(
				d.DatName,
				d.FileName,
				d.Object.Id,
				d.DatChecksum,
				d.xxHash3,
				d.Object.ObjectType,
				d.Object.ObjectSource,
				d.Object.CreatedDate,
				d.Object.ModifiedDate,
				d.Object.VehicleType))
			.ToList();
		return new ObjectIndex(rows);
	}
}
