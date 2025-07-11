using Dat.Objects;
using Definitions.Database;

namespace Definitions.DTO
{
	public class DtoObjectHillShapes : IDtoSubObject
	{
		public uint8_t HillHeightMapCount { get; set; }
		public uint8_t MountainHeightMapCount { get; set; }
		public HillShapeFlags Flags { get; set; }
		public UniqueObjectId Id { get; set; }
	}
}
