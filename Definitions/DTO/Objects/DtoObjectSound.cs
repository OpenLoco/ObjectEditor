using Definitions.Database;
using Definitions.ObjectModels.Objects.Sound;

namespace Definitions.DTO;

public class DtoObjectSound : IDtoSubObject
{
	public uint8_t ShouldLoop { get; set; }
	public uint32_t Volume { get; set; }
	public SoundObjectData SoundObjectData { get; set; } = null!;
	public UniqueObjectId Id { get; set; }
}
