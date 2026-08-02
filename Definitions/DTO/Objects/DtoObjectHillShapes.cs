using Definitions.Database.Base;

namespace Definitions.DTO.Objects;

public class DtoObjectHillShapes : IDtoSubObject
{
	public uint8_t HillHeightMapCount { get; set; }
	public uint8_t MountainHeightMapCount { get; set; }
	public bool IsHeightMap { get; set; }
	public UniqueObjectId Id { get; set; }
}
