using Dat.Data;
using Dat.Objects;
using Dat.Types;
using System.Reflection;

namespace Dat.FileParsing;

public static class AttributeHelper
{
	public static T? Get<T>(PropertyInfo p) where T : Attribute
	{
		var attributes = p.GetCustomAttributes(typeof(T), inherit: false);
		return attributes.Length == 1 ? attributes[0] as T : null;
	}

	public static T? Get<T>(Type t) where T : Attribute
		=> t.GetCustomAttribute<T>(inherit: false);

	public static bool Has<T>(PropertyInfo p) where T : Attribute
		=> p.GetCustomAttribute<T>(inherit: false) is not null;

	public static bool Has<T>(Type t) where T : Attribute
		=> t.GetCustomAttribute<T>(inherit: false) is not null;

	public static IEnumerable<PropertyInfo> GetAllPropertiesWithAttribute<T>(Type t) where T : Attribute
		=> t.GetProperties().Where(Has<T>);
}

public static class ObjectTypeMapping
{
	public static Type TypeToStruct(ObjectType objectType)
		=> objectType switch
		{
			ObjectType.Airport => typeof(AirportObject),
			ObjectType.Bridge => typeof(BridgeObject),
			ObjectType.Building => typeof(BuildingObject),
			ObjectType.Cargo => typeof(CargoObject),
			ObjectType.CliffEdge => typeof(CliffEdgeObject),
			ObjectType.Climate => typeof(ClimateObject),
			ObjectType.Competitor => typeof(CompetitorObject),
			ObjectType.Currency => typeof(CurrencyObject),
			ObjectType.Dock => typeof(DockObject),
			ObjectType.HillShapes => typeof(HillShapesObject),
			ObjectType.Industry => typeof(IndustryObject),
			ObjectType.InterfaceSkin => typeof(InterfaceSkinObject),
			ObjectType.Land => typeof(LandObject),
			ObjectType.LevelCrossing => typeof(LevelCrossingObject),
			ObjectType.Region => typeof(RegionObject),
			ObjectType.RoadExtra => typeof(RoadExtraObject),
			ObjectType.Road => typeof(RoadObject),
			ObjectType.RoadStation => typeof(RoadStationObject),
			ObjectType.Scaffolding => typeof(ScaffoldingObject),
			ObjectType.ScenarioText => typeof(ScenarioTextObject),
			ObjectType.Snow => typeof(SnowObject),
			ObjectType.Sound => typeof(SoundObject),
			ObjectType.Steam => typeof(SteamObject),
			ObjectType.StreetLight => typeof(StreetLightObject),
			ObjectType.TownNames => typeof(TownNamesObject),
			ObjectType.TrackExtra => typeof(TrackExtraObject),
			ObjectType.Track => typeof(TrackObject),
			ObjectType.TrackSignal => typeof(TrackSignalObject),
			ObjectType.TrackStation => typeof(TrackStationObject),
			ObjectType.Tree => typeof(TreeObject),
			ObjectType.Tunnel => typeof(TunnelObject),
			ObjectType.Vehicle => typeof(VehicleObject),
			ObjectType.Wall => typeof(WallObject),
			ObjectType.Water => typeof(WaterObject),
			_ => throw new ArgumentOutOfRangeException(nameof(objectType), $"unknown object type {objectType}")
		};

	public static ObjectType? StructToType(ILocoStruct locoStruct)
		=> AttributeHelper.Get<LocoStructTypeAttribute>(locoStruct.GetType())?.ObjectType;
}

public static class ObjectAttributes
{
	public static int StructSize<T>() where T : ILocoStruct
		=> AttributeHelper.Get<LocoStructSizeAttribute>(typeof(T))!.Size;

	public static ObjectType ObjectType<T>() // where T : ILocoStruct
		=> AttributeHelper.Get<LocoStructTypeAttribute>(typeof(T))!.ObjectType;

	public static ObjectType ObjectType(ILocoStruct str) // where T : ILocoStruct
		=> AttributeHelper.Get<LocoStructTypeAttribute>(str.GetType())!.ObjectType;

	public static string[] StringTableNames<T>() // where T : ILocoStruct
		=> AttributeHelper.Get<LocoStringTableAttribute>(typeof(T))!.Strings;
}
