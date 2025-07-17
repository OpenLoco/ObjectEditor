using Dat.Data;
using Dat.Objects;
using System.Text.Json.Serialization;

namespace Definitions.Index;

public record ObjectIndexEntry(
	string DisplayName,
	string? FileName, // only available in local mode
	UniqueObjectId? Id, // only available in online-mode
	uint32_t? DatChecksum, // only available if a DAT object
	ulong? xxHash3, // only available if a DAT object
	ObjectType ObjectType,
	ObjectSource ObjectSource,
	DateOnly? CreatedDate,
	DateOnly? ModifiedDate,
	VehicleType? VehicleType = null)
{
	[JsonIgnore]
	public string SimpleText
		=> $"{DisplayName} | {FileName}";
}
