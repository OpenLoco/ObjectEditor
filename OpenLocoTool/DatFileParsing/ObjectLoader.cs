using OpenLocoTool.Headers;
using OpenLocoTool.Objects;

namespace OpenLocoTool.DatFileParsing
{
	public static class ObjectLoader
	{
		public static BaseObject LoadBridge(ReadOnlySpan<byte> data)
		{
			var bridgeObject = new BaseObject();

			var bridge = new BridgeObject
			{
				NoRoof = ByteReaderT.Read_uint8t(data, 0x02),
				var_06 = ByteReaderT.Read_uint16t(data, 0x06),
				SpanLength = ByteReaderT.Read_uint8t(data, 0x08),
				PillarSpacing = ByteReaderT.Read_uint8t(data, 0x09),
				MaxSpeed = ByteReaderT.Read_Speed16(data, 0x0A),
				MaxHeight = ByteReaderT.Read_MicroZ(data, 0x0C),
				CostIndex = ByteReaderT.Read_uint8t(data, 0x0D),
				BaseCostFactor = ByteReaderT.Read_int16t(data, 0x0E),
				HeightCostFactor = ByteReaderT.Read_int16t(data, 0x10),
				SellCostFactor = ByteReaderT.Read_int16t(data, 0x12),
				DisabledTrackCfg = ByteReaderT.Read_uint16t(data, 0x014),
				TrackNumCompatible = ByteReaderT.Read_uint8t(data, 0x1A),
				RoadNumCompatible = ByteReaderT.Read_uint8t(data, 0x22),
				DesignedYear = ByteReaderT.Read_uint16t(data, 0x2A)
			};
			bridgeObject.Object = bridge;

			var stringTableOffset = data[0x2C..];
			var (table, stBytesRead) = SawyerStreamReader.LoadStringTable(stringTableOffset, ObjectAttributes.StringTableNames<BridgeObject>());
			bridgeObject.StringTable = table;

			var variableDataOffset = stringTableOffset[stBytesRead..];
			var variableDataBytesRead = (bridge.TrackNumCompatible + bridge.RoadNumCompatible) * S5Header.StructLength;

			var graphicsDataOffset = variableDataOffset[variableDataBytesRead..];
			var (_, g1Table, _) = SawyerStreamReader.LoadImageTable(graphicsDataOffset);
			bridgeObject.GraphicsTable = g1Table;

			return bridgeObject;
		}
	}
}
