using System.ComponentModel;
using OpenLocoTool.DatFileParsing;
using OpenLocoTool.Headers;

namespace OpenLocoTool.Objects
{
    [Flags]
	public enum IndustryObjectFlags : uint32_t
	{
		none = 0,
		builtInClusters = 1 << 0,
		builtOnHighGround = 1 << 1,
		builtOnLowGround = 1 << 2,
		builtOnSnow = 1 << 3,        // above summer snow line
		builtBelowSnowLine = 1 << 4, // below winter snow line
		builtOnFlatGround = 1 << 5,
		builtNearWater = 1 << 6,
		builtAwayFromWater = 1 << 7,
		builtOnWater = 1 << 8,
		builtNearTown = 1 << 9,
		builtAwayFromTown = 1 << 10,
		builtNearTrees = 1 << 11,
		builtRequiresOpenSpace = 1 << 12,
		oilfield = 1 << 13,
		mines = 1 << 14,
		canBeFoundedByPlayer = 1 << 16,
		requiresAllCargo = 1 << 17,
		unk18 = 1 << 18,
		unk19 = 1 << 19,
		hasShadows = 1 << 21,
		unk23 = 1 << 23,
		builtInDesert = 1 << 24,
		builtNearDesert = 1 << 25,
		unk27 = 1 << 27,
		flag_28 = 1 << 28,
	}

	[TypeConverter(typeof(ExpandableObjectConverter))]
	public record BuildingPartAnimation() : ILocoStruct
	{
		public int ObjectStructSize => 0x2;
		public static ILocoStruct Read(ReadOnlySpan<byte> data) => throw new NotImplementedException();
		public ReadOnlySpan<byte> Write() => throw new NotImplementedException();
	}

	[TypeConverter(typeof(ExpandableObjectConverter))]
	public record IndustryObjectUnk38() : ILocoStruct
	{
		public int ObjectStructSize => 0x2;
		public static ILocoStruct Read(ReadOnlySpan<byte> data) => throw new NotImplementedException();
		public ReadOnlySpan<byte> Write() => throw new NotImplementedException();
	}

	[TypeConverter(typeof(ExpandableObjectConverter))]
	public record IndustryObjectProductionRateRange() : ILocoStruct
	{
		public int ObjectStructSize => 0x4;
		public static ILocoStruct Read(ReadOnlySpan<byte> data) => throw new NotImplementedException();
		public ReadOnlySpan<byte> Write() => throw new NotImplementedException();
	}

	[TypeConverter(typeof(ExpandableObjectConverter))]
	public record IndustryObject() : ILocoStruct
	{
		public ObjectType ObjectType => ObjectType.industry;
		public int ObjectStructSize => 0xF4;
		public static ILocoStruct Read(ReadOnlySpan<byte> data) => throw new NotImplementedException();
		public ReadOnlySpan<byte> Write() => throw new NotImplementedException();
	}
}
