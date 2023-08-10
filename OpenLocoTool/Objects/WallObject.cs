
using System.ComponentModel;
using OpenLocoTool.DatFileParsing;
using OpenLocoTool.Headers;

namespace OpenLocoTool.Objects
{
	[Flags]
	public enum WallObjectFlags : uint8_t
	{
		none = 0,
		hasPrimaryColour = 1 << 0,
		unk1 = 1 << 1,
		onlyOnLevelLand = 1 << 2,
		unk3 = 1 << 3,
		unk4 = 1 << 4,
		unk5 = 1 << 5,
		hasSecondaryColour = 1 << 6,
		hasTertiaryColour = 1 << 7,
	};

	[TypeConverter(typeof(ExpandableObjectConverter))]
	public record WallObject(
		[property: LocoStructProperty(0x00)] string_id Name,
		[property: LocoStructProperty(0x02)] uint32_t Image,
		[property: LocoStructProperty(0x06)] uint8_t var_06,
		[property: LocoStructProperty(0x07)] WallObjectFlags Flags,
		[property: LocoStructProperty(0x08)] uint8_t Height,
		[property: LocoStructProperty(0x09)] uint8_t var_09
		) : ILocoStruct
	{
		public ObjectType ObjectType => ObjectType.wall;
		public static int ObjectStructSize => 0xA;
		public static ILocoStruct Read(ReadOnlySpan<byte> data) => throw new NotImplementedException();
		public ReadOnlySpan<byte> Write() => throw new NotImplementedException();
	}
}
