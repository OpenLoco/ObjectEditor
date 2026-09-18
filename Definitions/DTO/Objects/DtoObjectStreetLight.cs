using Definitions.Database;

namespace Definitions.DTO;

public class DtoObjectStreetLight : IDtoSubObject
{
	public List<uint16_t> DesignedYears { get; set; } = [];
	public UniqueObjectId Id { get; set; }
}
