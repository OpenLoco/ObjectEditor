using Definitions.Database;

namespace Definitions.DTO
{
	public class DtoObjectSound : IDtoSubObject
	{
		public uint8_t ShouldLoop { get; set; }
		public uint32_t Volume { get; set; }
		public UniqueObjectId Id { get; set; }
	}
}
