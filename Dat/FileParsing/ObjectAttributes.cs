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

	public static int StructSize(ObjectType objectType)
		=> objectType switch
		{
			ObjectType.InterfaceSkin => 0x18,
			ObjectType.Sound => 0x0C,
			ObjectType.Currency => 0x0C,
			ObjectType.Steam => 0x28,
			ObjectType.CliffEdge => 0x06,
			ObjectType.Water => 0x0E,
			ObjectType.Land => 0x1E,
			ObjectType.TownNames => 0x1A,
			ObjectType.Cargo => 0x1F,
			ObjectType.Wall => 0x0A,
			ObjectType.TrackSignal => 0x1E,
			ObjectType.LevelCrossing => 0x12,
			ObjectType.StreetLight => 0x0C,
			ObjectType.Tunnel => 0x06,
			ObjectType.Bridge => 0x2C,
			ObjectType.TrackStation => 0xAE,
			ObjectType.TrackExtra => 0x12,
			ObjectType.Track => 0x36,
			ObjectType.RoadStation => 0x6E,
			ObjectType.RoadExtra => 0x12,
			ObjectType.Road => 0x30,
			ObjectType.Airport => 0xBA,
			ObjectType.Dock => 0x28,
			ObjectType.Vehicle => 0x15E,
			ObjectType.Tree => 0x4C,
			ObjectType.Snow => 0x06,
			ObjectType.Climate => 0x0A,
			ObjectType.HillShapes => 0x0E,
			ObjectType.Building => 0xBE,
			ObjectType.Scaffolding => 0x12,
			ObjectType.Industry => 0xF4,
			ObjectType.Region => 0x12,
			ObjectType.Competitor => 0x38,
			ObjectType.ScenarioText => 0x06,
			_ => throw new NotSupportedException($"Object type {objectType} is not supported.")
		};
}
