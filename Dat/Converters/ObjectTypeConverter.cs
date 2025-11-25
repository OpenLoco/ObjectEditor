using Dat.Data;
using Dat.Types;
using Definitions.ObjectModels.Types;

namespace Dat.Converters;

public static class S5HeaderConverter
{
	public static S5Header Convert(this ObjectModelHeader objectModelHeader)
		=> new(objectModelHeader.Name, objectModelHeader.DatChecksum)
		{
			ObjectType = objectModelHeader.ObjectType.Convert(),
			ObjectSource = objectModelHeader.ObjectSource.Convert(),
		};

	public static ObjectModelHeader Convert(this S5Header s5Header)
		=> new(
			s5Header.Name,
			s5Header.ObjectType.Convert(),
			s5Header.ObjectSource.Convert(s5Header.Name, s5Header.Checksum),
			s5Header.Checksum);
}

public static class ObjectTypeConverter
{
	public static ObjectType Convert(this DatObjectType datObjectType)
		=> datObjectType switch
		{
			DatObjectType.InterfaceSkin => ObjectType.InterfaceSkin,
			DatObjectType.Sound => ObjectType.Sound,
			DatObjectType.Currency => ObjectType.Currency,
			DatObjectType.Steam => ObjectType.Steam,
			DatObjectType.CliffEdge => ObjectType.CliffEdge,
			DatObjectType.Water => ObjectType.Water,
			DatObjectType.Land => ObjectType.Land,
			DatObjectType.TownNames => ObjectType.TownNames,
			DatObjectType.Cargo => ObjectType.Cargo,
			DatObjectType.Wall => ObjectType.Wall,
			DatObjectType.TrackSignal => ObjectType.TrackSignal,
			DatObjectType.LevelCrossing => ObjectType.LevelCrossing,
			DatObjectType.StreetLight => ObjectType.StreetLight,
			DatObjectType.Tunnel => ObjectType.Tunnel,
			DatObjectType.Bridge => ObjectType.Bridge,
			DatObjectType.TrackStation => ObjectType.TrackStation,
			DatObjectType.TrackExtra => ObjectType.TrackExtra,
			DatObjectType.Track => ObjectType.Track,
			DatObjectType.RoadStation => ObjectType.RoadStation,
			DatObjectType.RoadExtra => ObjectType.RoadExtra,
			DatObjectType.Road => ObjectType.Road,
			DatObjectType.Airport => ObjectType.Airport,
			DatObjectType.Dock => ObjectType.Dock,
			DatObjectType.Vehicle => ObjectType.Vehicle,
			DatObjectType.Tree => ObjectType.Tree,
			DatObjectType.Snow => ObjectType.Snow,
			DatObjectType.Climate => ObjectType.Climate,
			DatObjectType.HillShapes => ObjectType.HillShapes,
			DatObjectType.Building => ObjectType.Building,
			DatObjectType.Scaffolding => ObjectType.Scaffolding,
			DatObjectType.Industry => ObjectType.Industry,
			DatObjectType.Region => ObjectType.Region,
			DatObjectType.Competitor => ObjectType.Competitor,
			DatObjectType.ScenarioText => ObjectType.ScenarioText,
			_ => throw new ArgumentOutOfRangeException(nameof(datObjectType), datObjectType, "Unknown Dat object type")
		};

	public static DatObjectType Convert(this ObjectType objectType)
		=> objectType switch
		{
			ObjectType.InterfaceSkin => DatObjectType.InterfaceSkin,
			ObjectType.Sound => DatObjectType.Sound,
			ObjectType.Currency => DatObjectType.Currency,
			ObjectType.Steam => DatObjectType.Steam,
			ObjectType.CliffEdge => DatObjectType.CliffEdge,
			ObjectType.Water => DatObjectType.Water,
			ObjectType.Land => DatObjectType.Land,
			ObjectType.TownNames => DatObjectType.TownNames,
			ObjectType.Cargo => DatObjectType.Cargo,
			ObjectType.Wall => DatObjectType.Wall,
			ObjectType.TrackSignal => DatObjectType.TrackSignal,
			ObjectType.LevelCrossing => DatObjectType.LevelCrossing,
			ObjectType.StreetLight => DatObjectType.StreetLight,
			ObjectType.Tunnel => DatObjectType.Tunnel,
			ObjectType.Bridge => DatObjectType.Bridge,
			ObjectType.TrackStation => DatObjectType.TrackStation,
			ObjectType.TrackExtra => DatObjectType.TrackExtra,
			ObjectType.Track => DatObjectType.Track,
			ObjectType.RoadStation => DatObjectType.RoadStation,
			ObjectType.RoadExtra => DatObjectType.RoadExtra,
			ObjectType.Road => DatObjectType.Road,
			ObjectType.Airport => DatObjectType.Airport,
			ObjectType.Dock => DatObjectType.Dock,
			ObjectType.Vehicle => DatObjectType.Vehicle,
			ObjectType.Tree => DatObjectType.Tree,
			ObjectType.Snow => DatObjectType.Snow,
			ObjectType.Climate => DatObjectType.Climate,
			ObjectType.HillShapes => DatObjectType.HillShapes,
			ObjectType.Building => DatObjectType.Building,
			ObjectType.Scaffolding => DatObjectType.Scaffolding,
			ObjectType.Industry => DatObjectType.Industry,
			ObjectType.Region => DatObjectType.Region,
			ObjectType.Competitor => DatObjectType.Competitor,
			ObjectType.ScenarioText => DatObjectType.ScenarioText,
			_ => throw new ArgumentOutOfRangeException(nameof(objectType), objectType, "Unknown Object type")
		};
}
