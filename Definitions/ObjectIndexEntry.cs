using Definitions.ObjectModels.Objects.Vehicle;
using Definitions.ObjectModels.Types;

namespace Definitions;

// Flat projection of TblObject + TblDatObject used by the UI and various tooling.
// Built either from the local DB (LocalObjectIndexService.BuildObjectIndexAsync)
// or from server-side DTOs (online browse).
public record ObjectIndexEntry(
	string DisplayName,
	string? FileName,           // local-only: path under the user's object folder
	UniqueObjectId? Id,         // online-only: server-assigned id
	uint32_t? DatChecksum,
	ulong? xxHash3,
	ObjectType ObjectType,
	ObjectSource ObjectSource,
	DateOnly? CreatedDate,
	DateOnly? ModifiedDate,
	VehicleType? VehicleType = null)
{
	public string SimpleText => $"{DisplayName} | {FileName}";
}
