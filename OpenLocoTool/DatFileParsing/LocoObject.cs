using System.ComponentModel;
using OpenLocoTool.Headers;
using OpenLocoTool.Objects;

namespace OpenLocoTool.DatFileParsing
{
	/*
	=== Dat File Format ===
	|-File-------------------------------|
	|-S5Header-|-DatHeader--|-ObjectData-|

	==============================================================================================================

	|-S5Header----------------|
	|-Flags-|-Name-|-Checksum-|

	|-DatHeader-------------|
	|-Encoding-|-Datalength-|

	|-ObjectData-----------------------------------------|
	|-Object-|-StringTable-|-VariableData-|-GraphicsData-|

	==============================================================================================================

	|-Object-|
	-- per-object

	|-StringTable-|
	|-String{n}---|

	|-VariableData-|
	-- per-object

	|-GraphicsData------------------------------|
	|-G1Header-|-G1Element32{n}-|-ImageBytes{n}-|

	==============================================================================================================

	|-String-----------------|
	|-Language-|-StringBytes-|

	|-G1Header---------------|
	|-Numentries-|-Totalsize-|

	|-G1Element32------------------------------------------------------|
	|-Offset-|-Width-|-Height-|-xOffset-|-yOffset-|-Flags-|-ZoomOffset-|

	|-ImageBytes-|
	-- offset by G1Element32.Offset
	*/

	[TypeConverter(typeof(ExpandableObjectConverter))]
	public interface ILocoStruct
	{
		static int StructSize { get; }
	}

	[TypeConverter(typeof(ExpandableObjectConverter))]
	public interface ILocoStructVariableData
	{
		ReadOnlySpan<byte> Load(ReadOnlySpan<byte> remainingData);

		ReadOnlySpan<byte> Save();
	}

	[TypeConverter(typeof(ExpandableObjectConverter))]
	public interface ILocoStructStringTablePostLoad
	{
		void LoadPostStringTable(StringTable stringTable);
	}

	[TypeConverter(typeof(ExpandableObjectConverter))]
	public interface ILocoObject
	{
		S5Header S5Header { get; set; }
		ObjectHeader ObjectHeader { get; set; }
		ILocoStruct Object { get; set; }
		StringTable StringTable { get; set; }
		G1Header G1Header { get; set; }
		List<G1Element32> G1Elements { get; set; }
	}

	[TypeConverter(typeof(ExpandableObjectConverter))]
	public class G1Dat
	{
		public G1Dat(G1Header g1Header, List<G1Element32> g1Elements)
		{
			G1Header = g1Header;
			G1Elements = g1Elements;
		}

		public G1Header G1Header { get; set; }
		public List<G1Element32> G1Elements { get; set; }
	}

	public static class ObjectTypeFixedSize
	{
		public static int GetSize(ObjectType objectType)
			=> objectType switch
			{
				ObjectType.Airport => AirportObject.StructSize,
				ObjectType.Bridge => BridgeObject.StructSize,
				ObjectType.Building => BuildingObject.StructSize,
				ObjectType.Cargo => CargoObject.StructSize,
				ObjectType.CliffEdge => CliffEdgeObject.StructSize,
				ObjectType.Climate => ClimateObject.StructSize,
				ObjectType.Competitor => CompetitorObject.StructSize,
				ObjectType.Currency => CurrencyObject.StructSize,
				ObjectType.Dock => DockObject.StructSize,
				ObjectType.HillShapes => HillShapesObject.StructSize,
				ObjectType.Industry => IndustryObject.StructSize,
				ObjectType.InterfaceSkin => InterfaceSkinObject.StructSize,
				ObjectType.Land => LandObject.StructSize,
				ObjectType.LevelCrossing => LevelCrossingObject.StructSize,
				ObjectType.Region => RegionObject.StructSize,
				ObjectType.RoadExtra => RoadExtraObject.StructSize,
				ObjectType.Road => RoadObject.StructSize,
				ObjectType.RoadStation => RoadStationObject.StructSize,
				ObjectType.Scaffolding => ScaffoldingObject.StructSize,
				ObjectType.ScenarioText => ScenarioTextObject.StructSize,
				ObjectType.Snow => SnowObject.StructSize,
				ObjectType.Sound => SoundObject.StructSize,
				ObjectType.Steam => SteamObject.StructSize,
				ObjectType.StreetLight => StreetLightObject.StructSize,
				ObjectType.TownNames => TownNamesObject.StructSize,
				ObjectType.TrackExtra => TrackExtraObject.StructSize,
				ObjectType.Track => TrackObject.StructSize,
				ObjectType.TrainSignal => TrainSignalObject.StructSize,
				ObjectType.TrainStation => TrainStationObject.StructSize,
				ObjectType.Tree => TreeObject.StructSize,
				ObjectType.Tunnel => TunnelObject.StructSize,
				ObjectType.Vehicle => VehicleObject.StructSize,
				ObjectType.Wall => WallObject.StructSize,
				ObjectType.Water => WaterObject.StructSize,
				_ => throw new ArgumentOutOfRangeException(nameof(objectType), $"unknown object type {objectType}")
			};
	}

	[TypeConverter(typeof(ExpandableObjectConverter))]
	public class LocoObject : ILocoObject
	{
		public LocoObject(S5Header s5Hdr, ObjectHeader objHdr, ILocoStruct obj, StringTable stringTable, G1Header g1Header, List<G1Element32> g1Elements)
		{
			S5Header = s5Hdr;
			ObjectHeader = objHdr;
			Object = obj;
			StringTable = stringTable;
			G1Header = g1Header;
			G1Elements = g1Elements;
		}
		public LocoObject(S5Header s5Hdr, ObjectHeader objHdr, ILocoStruct obj, StringTable stringTable)
		{
			S5Header = s5Hdr;
			ObjectHeader = objHdr;
			Object = obj;
			StringTable = stringTable;
			G1Header = null;
			G1Elements = null;
		}

		public S5Header S5Header { get; set; }
		public ObjectHeader ObjectHeader { get; set; }
		public ILocoStruct Object { get; set; }
		public StringTable StringTable { get; set; }
		public G1Header? G1Header { get; set; }
		public List<G1Element32>? G1Elements { get; set; }
	}
}
