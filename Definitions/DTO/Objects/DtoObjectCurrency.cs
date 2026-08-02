using Definitions.Database.Base;

namespace Definitions.DTO.Objects;

public class DtoObjectCurrency : IDtoSubObject
{
	public uint8_t Separator { get; set; }
	public uint8_t Factor { get; set; }
	public UniqueObjectId Id { get; set; }
}
