using Definitions.ObjectModels.Types;
using System.ComponentModel.DataAnnotations;
using System.Diagnostics.CodeAnalysis;

namespace Definitions.ObjectModels.Objects.Steam;

public class SteamObject : ILocoStruct
{
	public uint8_t NumStationaryTicks { get; set; }
	public uint8_t SpriteWidth { get; set; }
	public uint8_t SpriteHeightNegative { get; set; }
	public uint8_t SpriteHeightPositive { get; set; }
	public SteamObjectFlags Flags { get; set; }
	public uint32_t var_0A { get; set; }
	public List<SteamImageAndHeight> FrameInfoType0 { get; set; } = [];
	public List<SteamImageAndHeight> FrameInfoType1 { get; set; } = [];

#pragma warning disable IL2026 // LengthAttribute constructor uses reflection to get 'Count' on non-ICollection types; our properties use List<T> which implements ICollection so Count is preserved.
	[Length(9, 9)]
	public List<ObjectModelHeader> SoundEffects { get; set; } = [];
#pragma warning restore IL2026

	public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
		=> [];
}
