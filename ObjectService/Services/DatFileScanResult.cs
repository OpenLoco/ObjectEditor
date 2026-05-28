using Definitions.ObjectModels.Objects.Vehicle;
using Definitions.ObjectModels.Types;

namespace ObjectService.Services;

// Plain result for a single scanned .dat file. Mirrors the metadata the old
// ObjectIndexEntry stored, but lives in the Dat project so it can be produced
// without any database dependencies. The LocalObjectIndexService (in Definitions)
// is responsible for translating these into TblObject + TblDatObject rows.
public record DatFileScanResult(
	string FullPath,
	string RelativePath,
	string DatName,
	uint DatChecksum,
	ulong xxHash3,
	ObjectType ObjectType,
	ObjectSource ObjectSource,
	DateOnly CreatedDate,
	DateOnly ModifiedDate,
	VehicleType? VehicleType);
