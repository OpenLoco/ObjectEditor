using System.ComponentModel;
using OpenLoco.ObjectEditor.Data;
using OpenLoco.ObjectEditor.DatFileParsing;

namespace OpenLoco.ObjectEditor.Objects
{
	[Flags]
	public enum WallObjectFlags : uint8_t
	{
		None = 0,
		HasPrimaryColour = 1 << 0,
		HasGlass = 1 << 1,  // unused? only for rct2?
		OnlyOnLevelLand = 1 << 2,
		DoubleSided = 1 << 3, // unused? only for rct2?
		Door = 1 << 4, // unused? only for rct2?
		LongDoorAnimation = 1 << 5, // unused? only for rct2?
		HasSecondaryColour = 1 << 6,
		HasTertiaryColour = 1 << 7, // unused? only for rct2?
	};

	// this is copied from OpenRCT2: https://github.com/OpenRCT2/OpenRCT2/blob/develop/src/openrct2/object/WallSceneryEntry.h
	[Flags]
	public enum WallObjectFlags2 : uint8_t
	{
		None = 0,
		NoSelectPrimaryColour = 1 << 0,
		DoorSoundMask = 1 << 1,  // unused? only for rct2?
		DoorSoundShift = 1 << 2, // unused? only for rct2?
		Opaque = 1 << 3, // unused? only for rct2?
		Animated = 1 << 4, // unused? only for rct2?
	};

	[TypeConverter(typeof(ExpandableObjectConverter))]
	[LocoStructSize(0x0A)]
	[LocoStructType(ObjectType.Wall)]
	[LocoStringTable("Name")]
	public record WallObject(
		[property: LocoStructOffset(0x00), LocoString, Browsable(false)] string_id Name,
		[property: LocoStructOffset(0x02), Browsable(false)] image_id Image,
		[property: LocoStructOffset(0x06), LocoPropertyMaybeUnused] uint8_t ToolId,
		[property: LocoStructOffset(0x07)] WallObjectFlags Flags,
		[property: LocoStructOffset(0x08)] uint8_t Height,
		[property: LocoStructOffset(0x09), LocoPropertyMaybeUnused] WallObjectFlags2 Flags2
		) : ILocoStruct
	{
		public bool Validate() => throw new NotImplementedException();
	}
}
