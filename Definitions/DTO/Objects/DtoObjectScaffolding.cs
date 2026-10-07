using Definitions.Database;

namespace Definitions.DTO;

public class DtoObjectScaffolding : IDtoSubObject
{
	public List<uint16_t> SegmentHeights { get; set; } = [];
	public List<uint16_t> RoofHeights { get; set; } = [];
	public UniqueObjectId Id { get; set; }
}
