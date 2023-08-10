
using System.ComponentModel;
using OpenLocoTool.DatFileParsing;
using OpenLocoTool.Headers;

namespace OpenLocoTool.Objects
{
	[Flags]
	public enum TrainStationFlags : uint8_t
	{
		None = 0,
		Recolourable = 1 << 0,
		unk1 = 1 << 1, // Has no canopy??
	};

	[TypeConverter(typeof(ExpandableObjectConverter))]
	public record TrainStationObject(
		[property: LocoStructProperty(0x00)] string_id Name,
		[property: LocoStructProperty(0x02)] uint8_t PaintStyle,
		[property: LocoStructProperty(0x03)] uint8_t var_03,
		[property: LocoStructProperty(0x04)] uint16_t TrackPieces,
		[property: LocoStructProperty(0x06)] int16_t BuildCostFactor,
		[property: LocoStructProperty(0x08)] int16_t SellCostFactor,
		[property: LocoStructProperty(0x0A)] uint8_t CostIndex,
		[property: LocoStructProperty(0x0B)] uint8_t var_0B,
		[property: LocoStructProperty(0x0C)] RoadStationFlags Flags,
		[property: LocoStructProperty(0x0D)] uint8_t var_0D,
		[property: LocoStructProperty(0x0E)] uint32_t Image,
		[property: LocoStructProperty(0x12), LocoArrayLength(4)] uint32_t[] var_12,
		[property: LocoStructProperty(0x22)] uint8_t NumCompatible,
		[property: LocoStructProperty(0x23), LocoArrayLength(7)] uint8_t[] Mods,
		[property: LocoStructProperty(0x2A)] uint16_t DesignedYear,
		[property: LocoStructProperty(0x2C)] uint16_t ObsoleteYear
		//[property: LocoStructProperty(0x2E)] const std::byte* CargoOffsetBytes[4][4]
		//[property: LocoStructProperty(0x??)] const std::byte* var_6E[16]
		) : ILocoStruct
	{
		public ObjectType ObjectType => ObjectType.trainStation;
		public static int ObjectStructSize => 0xAE;
		public static ILocoStruct Read(ReadOnlySpan<byte> data) => throw new NotImplementedException();
		public ReadOnlySpan<byte> Write() => throw new NotImplementedException();
	}
}
