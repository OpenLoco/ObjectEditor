using System.ComponentModel;
using OpenLocoObjectEditor.Data;
using OpenLocoObjectEditor.DatFileParsing;

namespace OpenLocoObjectEditor.Objects
{
	[Flags]
	public enum WallObjectFlags : uint8_t
	{
		None = 0,
		HasPrimaryColour = 1 << 0,
		unk1 = 1 << 1,
		OnlyOnLevelLand = 1 << 2,
		unk3 = 1 << 3,
		unk4 = 1 << 4,
		unk5 = 1 << 5,
		HasSecondaryColour = 1 << 6,
		HasTertiaryColour = 1 << 7,
	};

	[TypeConverter(typeof(ExpandableObjectConverter))]
	[LocoStructSize(0x0A)]
	[LocoStructType(ObjectType.Wall)]
	[LocoStringTable("Name")]
	public class WallObject(
		[property: LocoStructOffset(0x00), LocoString, Browsable(false)] string_id Name,
		[property: LocoStructOffset(0x02), Browsable(false)] image_id Image,
		[property: LocoStructOffset(0x06), LocoPropertyMaybeUnused] uint8_t var_06,
		[property: LocoStructOffset(0x07)] WallObjectFlags Flags,
		[property: LocoStructOffset(0x08)] uint8_t Height,
		[property: LocoStructOffset(0x09), LocoPropertyMaybeUnused] uint8_t var_09
	) : ILocoStruct;
}
