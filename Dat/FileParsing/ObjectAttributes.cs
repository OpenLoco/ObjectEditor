using Dat.Data;
using Definitions.ObjectModels;

namespace Dat.FileParsing;

public static class ObjectAttributes
{
	public static int StructSize<T>() where T : ILocoStruct
		=> AttributeHelper.Get<LocoStructSizeAttribute>(typeof(T))!.Size;

	//public static ObjectType ObjectType<T>() // where T : ILocoStruct
	//	=> AttributeHelper.Get<LocoStructTypeAttribute>(typeof(T))!.ObjectType;

	//public static ObjectType ObjectType(ILocoStruct str) // where T : ILocoStruct
	//	=> AttributeHelper.Get<LocoStructTypeAttribute>(str.GetType())!.ObjectType;

	//public static string[] StringTableNames<T>() // where T : ILocoStruct
	//	=> AttributeHelper.Get<LocoStringTableAttribute>(typeof(T))!.Strings;

	public static string[] StringTable(DatObjectType objectType)
		=> objectType switch
		{
			DatObjectType.InterfaceSkin => ["Name"],
			DatObjectType.Sound => ["Name"],
			DatObjectType.Currency => ["Name", "PrefixSymbol", "SuffixSymbol"],
			DatObjectType.Steam => ["Name"],
			DatObjectType.CliffEdge => ["Name"],
			DatObjectType.Water => ["Name"],
			DatObjectType.Land => ["Name"],
			DatObjectType.TownNames => ["Name"],
			DatObjectType.Cargo => ["Name", "UnitsAndCargoName", "UnitNameSingular", "UnitNamePlural"],
			DatObjectType.Wall => ["Name"],
			DatObjectType.TrackSignal => ["Name", "Description"],
			DatObjectType.LevelCrossing => ["Name"],
			DatObjectType.StreetLight => ["Name"],
			DatObjectType.Tunnel => ["Name"],
			DatObjectType.Bridge => ["Name"],
			DatObjectType.TrackStation => ["Name"],
			DatObjectType.TrackExtra => ["Name"],
			DatObjectType.Track => ["Name"],
			DatObjectType.RoadStation => ["Name"],
			DatObjectType.RoadExtra => ["Name"],
			DatObjectType.Road => ["Name"],
			DatObjectType.Airport => ["Name"],
			DatObjectType.Dock => ["Name"],
			DatObjectType.Vehicle => ["Name"],
			DatObjectType.Tree => ["Name"],
			DatObjectType.Snow => ["Name"],
			DatObjectType.Climate => ["Name"],
			DatObjectType.HillShapes => ["Name"],
			DatObjectType.Building => ["Name"],
			DatObjectType.Scaffolding => ["Name"],
			DatObjectType.Industry => ["Name", "var_02", "<unused>", "NameClosingDown", "NameUpProduction", "NameDownProduction", "NameSingular", "NamePlural"],
			DatObjectType.Region => ["Name"],
			DatObjectType.Competitor => ["FullName", "LastName"],
			DatObjectType.ScenarioText => ["Name", "Details"],
			_ => throw new NotSupportedException($"Object type {objectType} is not supported.")
		};

	public static int StructSize(DatObjectType objectType)
		=> objectType switch
		{
			DatObjectType.InterfaceSkin => 0x18,
			DatObjectType.Sound => 0x0C,
			DatObjectType.Currency => 0x0C,
			DatObjectType.Steam => 0x28,
			DatObjectType.CliffEdge => 0x06,
			DatObjectType.Water => 0x0E,
			DatObjectType.Land => 0x1E,
			DatObjectType.TownNames => 0x1A,
			DatObjectType.Cargo => 0x1F,
			DatObjectType.Wall => 0x0A,
			DatObjectType.TrackSignal => 0x1E,
			DatObjectType.LevelCrossing => 0x12,
			DatObjectType.StreetLight => 0x0C,
			DatObjectType.Tunnel => 0x06,
			DatObjectType.Bridge => 0x2C,
			DatObjectType.TrackStation => 0xAE,
			DatObjectType.TrackExtra => 0x12,
			DatObjectType.Track => 0x36,
			DatObjectType.RoadStation => 0x6E,
			DatObjectType.RoadExtra => 0x12,
			DatObjectType.Road => 0x30,
			DatObjectType.Airport => 0xBA,
			DatObjectType.Dock => 0x28,
			DatObjectType.Vehicle => 0x15E,
			DatObjectType.Tree => 0x4C,
			DatObjectType.Snow => 0x06,
			DatObjectType.Climate => 0x0A,
			DatObjectType.HillShapes => 0x0E,
			DatObjectType.Building => 0xBE,
			DatObjectType.Scaffolding => 0x12,
			DatObjectType.Industry => 0xF4,
			DatObjectType.Region => 0x12,
			DatObjectType.Competitor => 0x38,
			DatObjectType.ScenarioText => 0x06,
			_ => throw new NotSupportedException($"Object type {objectType} is not supported.")
		};
}
