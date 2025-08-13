using Definitions.ObjectModels.Objects.Vehicle;
using Definitions.ObjectModels.Types;

namespace Definitions.Database;

public class TblObject : DbCoreObject //<T> : DbCoreObject where T : DbSubObject
{
	//[NotMapped]
	//public DbSubObject SubObject => GetSubObject<T>

	//public async Task<T> GetSubObject<T>(LocoDbContext context)
	//	where T : DbSubObject
	//	=> (T)await DbSubObjectHelper.GetDbSetForType(context, ObjectType);

	public UniqueObjectId SubObjectId { get; set; } // FK id

	public ObjectType ObjectType { get; set; } // don't need to set explicitly - can be inferred from T type

	public ObjectSource ObjectSource { get; set; }

	public VehicleType? VehicleType { get; set; }

	public ObjectAvailability Availability { get; set; }

	public ICollection<TblObjectPack> ObjectPacks { get; set; } = [];

	public ICollection<TblDatObject> DatObjects { get; set; } = []; // the DAT objects that created or reference this OpenLoco object. May be 0, may be multiple

	public ICollection<TblStringTableRow> StringTable { get; set; } = [];
}
