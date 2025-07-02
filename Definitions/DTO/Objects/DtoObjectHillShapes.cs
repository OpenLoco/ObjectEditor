using OpenLoco.Dat.Objects;

namespace OpenLoco.Definitions.DTO
{
	public class DtoObjectHillShapes : IHasId
	{
		public uint8_t HillHeightMapCount { get; set; }
		public uint8_t MountainHeightMapCount { get; set; }
		public HillShapeFlags Flags { get; set; }
		public UniqueObjectId Id { get; set; }
	}
}
