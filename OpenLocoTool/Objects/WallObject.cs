
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
		[property: LocoStructProperty] string_id Name,
		[property: LocoStructProperty] uint32_t Image, // 0x02
		[property: LocoStructProperty] uint8_t var_06,
		[property: LocoStructProperty] WallObjectFlags Flags, // 0x07
		[property: LocoStructProperty] uint8_t Height,
		[property: LocoStructProperty] uint8_t var_09
		) : ILocoStruct
	{
		public ObjectType ObjectType => ObjectType.wall;
		public int ObjectStructSize => 0xA;
		public static ILocoStruct Read(ReadOnlySpan<byte> data) => throw new NotImplementedException();
		public ReadOnlySpan<byte> Write() => throw new NotImplementedException();
	}
}
