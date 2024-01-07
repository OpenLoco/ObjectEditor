using System.ComponentModel;
using OpenLocoTool.DatFileParsing;
using OpenLocoTool.Headers;

namespace OpenLocoTool.Objects
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
		uint8_t var_06,
		WallObjectFlags flags,
		uint8_t height,
		uint8_t var_09)
		: ILocoStruct
	{

		//[LocoStructOffset(0x00), LocoString, Browsable(false)] string_id Name,
		//[LocoStructOffset(0x02)] public image_id Image,
		[LocoStructOffset(0x06), LocoPropertyMaybeUnused] public uint8_t var_06 { get; set; } = var_06;
		[LocoStructOffset(0x07)] public WallObjectFlags Flags { get; set; } = flags;
		[LocoStructOffset(0x08)] public uint8_t Height { get; set; } = height;
		[LocoStructOffset(0x09), LocoPropertyMaybeUnused] public uint8_t var_09 { get; set; } = var_09;
	}
}
