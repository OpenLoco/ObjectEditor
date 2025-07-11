using Microsoft.EntityFrameworkCore;
using Dat.Data;
using Dat.Objects;
using Dat.Types;
using Definitions.DTO.Mappers;

namespace Definitions.Database;

public interface IConvertibleToTable<TTable, TDat>
{
	static abstract TTable FromObject(TblObject tblObj, TDat datObo);
}

public interface IDtoSubObject : IHasId
{
	//IDbSubObject ToTbl();
}

public interface IDbSubObject : IHasId
{
	public abstract TblObject Parent { get; set; }
	//IDtoSubObject ToDto();
}

[Index(nameof(Id), IsUnique = true)]
public abstract class DbSubObject : DbIdObject, IDbSubObject
{
	public required TblObject Parent { get; set; }

	//public abstract IDtoSubObject ToDto();
}

public abstract class DtoSubObject : DbIdObject, IDtoSubObject
{
	//public abstract IDbSubObject ToTbl();
}

public static class DbSubObjectHelper
{
	static async Task<string> AddOrUpdate<TSubObject, TDat>(LocoDbContext db, DbSet<TSubObject> subObjTable, TblObject parentObj, ILocoStruct datObj)
		where TSubObject : class, IDbSubObject, IConvertibleToTable<TSubObject, TDat>
		where TDat : ILocoStruct
	{
		var subObj = TSubObject.FromObject(parentObj, (TDat)datObj);
		return await AddOrUpdateCore<TSubObject, TDat>(db, subObjTable, parentObj, subObj);
	}

	static async Task<string> AddOrUpdateCore<TSubObject, TDat>(LocoDbContext db, DbSet<TSubObject> subObjTable, TblObject parentObj, TSubObject subObj)
		where TSubObject : class, IDbSubObject, IConvertibleToTable<TSubObject, TDat>
		where TDat : ILocoStruct
	{
		var existingSubObj = await subObjTable.SingleOrDefaultAsync(x => x.Id == parentObj.SubObjectId);
		if (existingSubObj != null)
		{
			subObj.Parent = parentObj;
			existingSubObj = subObj; // update it
			parentObj.SubObjectId = existingSubObj.Id;

			return $"Updated {parentObj.Id}-{existingSubObj.Id}";
		}
		else
		{
			var newSubObj = await subObjTable.AddAsync(subObj);
			_ = await db.SaveChangesAsync(); // must save object to obtain an id
			parentObj.SubObjectId = newSubObj.Entity.Id;

			return $"Added {parentObj.Id}-{newSubObj.Entity.Id}";
		}
	}

	public static async Task<string> AddOrUpdate(LocoDbContext db, TblObject obj, IDbSubObject locoStruct)
		=> obj.ObjectType switch
		{
			ObjectType.Airport => await AddOrUpdateCore<TblObjectAirport, AirportObject>(db, db.ObjAirport, obj, (TblObjectAirport)locoStruct),
			ObjectType.Bridge => await AddOrUpdateCore<TblObjectBridge, BridgeObject>(db, db.ObjBridge, obj, (TblObjectBridge)locoStruct),
			ObjectType.Building => await AddOrUpdateCore<TblObjectBuilding, BuildingObject>(db, db.ObjBuilding, obj, (TblObjectBuilding)locoStruct),
			ObjectType.Cargo => await AddOrUpdateCore<TblObjectCargo, CargoObject>(db, db.ObjCargo, obj, (TblObjectCargo)locoStruct),
			ObjectType.CliffEdge => await AddOrUpdateCore<TblObjectCliffEdge, CliffEdgeObject>(db, db.ObjCliffEdge, obj, (TblObjectCliffEdge)locoStruct),
			ObjectType.Climate => await AddOrUpdateCore<TblObjectClimate, ClimateObject>(db, db.ObjClimate, obj, (TblObjectClimate)locoStruct),
			ObjectType.Competitor => await AddOrUpdateCore<TblObjectCompetitor, CompetitorObject>(db, db.ObjCompetitor, obj, (TblObjectCompetitor)locoStruct),
			ObjectType.Currency => await AddOrUpdateCore<TblObjectCurrency, CurrencyObject>(db, db.ObjCurrency, obj, (TblObjectCurrency)locoStruct),
			ObjectType.Dock => await AddOrUpdateCore<TblObjectDock, DockObject>(db, db.ObjDock, obj, (TblObjectDock)locoStruct),
			ObjectType.HillShapes => await AddOrUpdateCore<TblObjectHillShapes, HillShapesObject>(db, db.ObjHillShapes, obj, (TblObjectHillShapes)locoStruct),
			ObjectType.Industry => await AddOrUpdateCore<TblObjectIndustry, IndustryObject>(db, db.ObjIndustry, obj, (TblObjectIndustry)locoStruct),
			ObjectType.InterfaceSkin => await AddOrUpdateCore<TblObjectInterface, InterfaceSkinObject>(db, db.ObjInterface, obj, (TblObjectInterface)locoStruct),
			ObjectType.Land => await AddOrUpdateCore<TblObjectLand, LandObject>(db, db.ObjLand, obj, (TblObjectLand)locoStruct),
			ObjectType.LevelCrossing => await AddOrUpdateCore<TblObjectLevelCrossing, LevelCrossingObject>(db, db.ObjLevelCrossing, obj, (TblObjectLevelCrossing)locoStruct),
			ObjectType.Region => await AddOrUpdateCore<TblObjectRegion, RegionObject>(db, db.ObjRegion, obj, (TblObjectRegion)locoStruct),
			ObjectType.RoadExtra => await AddOrUpdateCore<TblObjectRoadExtra, RoadExtraObject>(db, db.ObjRoadExtra, obj, (TblObjectRoadExtra)locoStruct),
			ObjectType.Road => await AddOrUpdateCore<TblObjectRoad, RoadObject>(db, db.ObjRoad, obj, (TblObjectRoad)locoStruct),
			ObjectType.RoadStation => await AddOrUpdateCore<TblObjectRoadStation, RoadStationObject>(db, db.ObjRoadStation, obj, (TblObjectRoadStation)locoStruct),
			ObjectType.Scaffolding => await AddOrUpdateCore<TblObjectScaffolding, ScaffoldingObject>(db, db.ObjScaffolding, obj, (TblObjectScaffolding)locoStruct),
			ObjectType.ScenarioText => await AddOrUpdateCore<TblObjectScenarioText, ScenarioTextObject>(db, db.ObjScenarioText, obj, (TblObjectScenarioText)locoStruct),
			ObjectType.Snow => await AddOrUpdateCore<TblObjectSnow, SnowObject>(db, db.ObjSnow, obj, (TblObjectSnow)locoStruct),
			ObjectType.Sound => await AddOrUpdateCore<TblObjectSound, SoundObject>(db, db.ObjSound, obj, (TblObjectSound)locoStruct),
			ObjectType.Steam => await AddOrUpdateCore<TblObjectSteam, SteamObject>(db, db.ObjSteam, obj, (TblObjectSteam)locoStruct),
			ObjectType.StreetLight => await AddOrUpdateCore<TblObjectStreetLight, StreetLightObject>(db, db.ObjStreetLight, obj, (TblObjectStreetLight)locoStruct),
			ObjectType.TownNames => await AddOrUpdateCore<TblObjectTownNames, TownNamesObject>(db, db.ObjTownNames, obj, (TblObjectTownNames)locoStruct),
			ObjectType.TrackExtra => await AddOrUpdateCore<TblObjectTrackExtra, TrackExtraObject>(db, db.ObjTrackExtra, obj, (TblObjectTrackExtra)locoStruct),
			ObjectType.Track => await AddOrUpdateCore<TblObjectTrack, TrackObject>(db, db.ObjTrack, obj, (TblObjectTrack)locoStruct),
			ObjectType.TrackSignal => await AddOrUpdateCore<TblObjectTrackSignal, TrackSignalObject>(db, db.ObjTrackSignal, obj, (TblObjectTrackSignal)locoStruct),
			ObjectType.TrackStation => await AddOrUpdateCore<TblObjectTrackStation, TrackStationObject>(db, db.ObjTrackStation, obj, (TblObjectTrackStation)locoStruct),
			ObjectType.Tree => await AddOrUpdateCore<TblObjectTree, TreeObject>(db, db.ObjTree, obj, (TblObjectTree)locoStruct),
			ObjectType.Tunnel => await AddOrUpdateCore<TblObjectTunnel, TunnelObject>(db, db.ObjTunnel, obj, (TblObjectTunnel)locoStruct),
			ObjectType.Vehicle => await AddOrUpdateCore<TblObjectVehicle, VehicleObject>(db, db.ObjVehicle, obj, (TblObjectVehicle)locoStruct),
			ObjectType.Water => await AddOrUpdateCore<TblObjectWater, WaterObject>(db, db.ObjWater, obj, (TblObjectWater)locoStruct),
			ObjectType.Wall => await AddOrUpdateCore<TblObjectWall, WallObject>(db, db.ObjWall, obj, (TblObjectWall)locoStruct),
			_ => throw new NotImplementedException(),
		};

	public static async Task<string> AddOrUpdate(LocoDbContext db, TblObject obj, ILocoStruct locoStruct)
		=> obj.ObjectType switch
		{
			ObjectType.Airport => await AddOrUpdate<TblObjectAirport, AirportObject>(db, db.ObjAirport, obj, (AirportObject)locoStruct),
			ObjectType.Bridge => await AddOrUpdate<TblObjectBridge, BridgeObject>(db, db.ObjBridge, obj, (BridgeObject)locoStruct),
			ObjectType.Building => await AddOrUpdate<TblObjectBuilding, BuildingObject>(db, db.ObjBuilding, obj, (BuildingObject)locoStruct),
			ObjectType.Cargo => await AddOrUpdate<TblObjectCargo, CargoObject>(db, db.ObjCargo, obj, (CargoObject)locoStruct),
			ObjectType.CliffEdge => await AddOrUpdate<TblObjectCliffEdge, CliffEdgeObject>(db, db.ObjCliffEdge, obj, (CliffEdgeObject)locoStruct),
			ObjectType.Climate => await AddOrUpdate<TblObjectClimate, ClimateObject>(db, db.ObjClimate, obj, (ClimateObject)locoStruct),
			ObjectType.Competitor => await AddOrUpdate<TblObjectCompetitor, CompetitorObject>(db, db.ObjCompetitor, obj, (CompetitorObject)locoStruct),
			ObjectType.Currency => await AddOrUpdate<TblObjectCurrency, CurrencyObject>(db, db.ObjCurrency, obj, (CurrencyObject)locoStruct),
			ObjectType.Dock => await AddOrUpdate<TblObjectDock, DockObject>(db, db.ObjDock, obj, (DockObject)locoStruct),
			ObjectType.HillShapes => await AddOrUpdate<TblObjectHillShapes, HillShapesObject>(db, db.ObjHillShapes, obj, (HillShapesObject)locoStruct),
			ObjectType.Industry => await AddOrUpdate<TblObjectIndustry, IndustryObject>(db, db.ObjIndustry, obj, (IndustryObject)locoStruct),
			ObjectType.InterfaceSkin => await AddOrUpdate<TblObjectInterface, InterfaceSkinObject>(db, db.ObjInterface, obj, (InterfaceSkinObject)locoStruct),
			ObjectType.Land => await AddOrUpdate<TblObjectLand, LandObject>(db, db.ObjLand, obj, (LandObject)locoStruct),
			ObjectType.LevelCrossing => await AddOrUpdate<TblObjectLevelCrossing, LevelCrossingObject>(db, db.ObjLevelCrossing, obj, (LevelCrossingObject)locoStruct),
			ObjectType.Region => await AddOrUpdate<TblObjectRegion, RegionObject>(db, db.ObjRegion, obj, (RegionObject)locoStruct),
			ObjectType.RoadExtra => await AddOrUpdate<TblObjectRoadExtra, RoadExtraObject>(db, db.ObjRoadExtra, obj, (RoadExtraObject)locoStruct),
			ObjectType.Road => await AddOrUpdate<TblObjectRoad, RoadObject>(db, db.ObjRoad, obj, (RoadObject)locoStruct),
			ObjectType.RoadStation => await AddOrUpdate<TblObjectRoadStation, RoadStationObject>(db, db.ObjRoadStation, obj, (RoadStationObject)locoStruct),
			ObjectType.Scaffolding => await AddOrUpdate<TblObjectScaffolding, ScaffoldingObject>(db, db.ObjScaffolding, obj, (ScaffoldingObject)locoStruct),
			ObjectType.ScenarioText => await AddOrUpdate<TblObjectScenarioText, ScenarioTextObject>(db, db.ObjScenarioText, obj, (ScenarioTextObject)locoStruct),
			ObjectType.Snow => await AddOrUpdate<TblObjectSnow, SnowObject>(db, db.ObjSnow, obj, (SnowObject)locoStruct),
			ObjectType.Sound => await AddOrUpdate<TblObjectSound, SoundObject>(db, db.ObjSound, obj, (SoundObject)locoStruct),
			ObjectType.Steam => await AddOrUpdate<TblObjectSteam, SteamObject>(db, db.ObjSteam, obj, (SteamObject)locoStruct),
			ObjectType.StreetLight => await AddOrUpdate<TblObjectStreetLight, StreetLightObject>(db, db.ObjStreetLight, obj, (StreetLightObject)locoStruct),
			ObjectType.TownNames => await AddOrUpdate<TblObjectTownNames, TownNamesObject>(db, db.ObjTownNames, obj, (TownNamesObject)locoStruct),
			ObjectType.TrackExtra => await AddOrUpdate<TblObjectTrackExtra, TrackExtraObject>(db, db.ObjTrackExtra, obj, (TrackExtraObject)locoStruct),
			ObjectType.Track => await AddOrUpdate<TblObjectTrack, TrackObject>(db, db.ObjTrack, obj, (TrackObject)locoStruct),
			ObjectType.TrackSignal => await AddOrUpdate<TblObjectTrackSignal, TrackSignalObject>(db, db.ObjTrackSignal, obj, (TrackSignalObject)locoStruct),
			ObjectType.TrackStation => await AddOrUpdate<TblObjectTrackStation, TrackStationObject>(db, db.ObjTrackStation, obj, (TrackStationObject)locoStruct),
			ObjectType.Tree => await AddOrUpdate<TblObjectTree, TreeObject>(db, db.ObjTree, obj, (TreeObject)locoStruct),
			ObjectType.Tunnel => await AddOrUpdate<TblObjectTunnel, TunnelObject>(db, db.ObjTunnel, obj, (TunnelObject)locoStruct),
			ObjectType.Vehicle => await AddOrUpdate<TblObjectVehicle, VehicleObject>(db, db.ObjVehicle, obj, (VehicleObject)locoStruct),
			ObjectType.Water => await AddOrUpdate<TblObjectWater, WaterObject>(db, db.ObjWater, obj, (WaterObject)locoStruct),
			ObjectType.Wall => await AddOrUpdate<TblObjectWall, WallObject>(db, db.ObjWall, obj, (WallObject)locoStruct),
			_ => throw new NotImplementedException(),
		};

	public static IDtoSubObject GetDbSubForType(LocoDbContext db, ObjectType objectType, UniqueObjectId parentId)
		=> objectType switch
		{
			ObjectType.Airport => DtoObjectAirportMapper.ToDto(db.ObjAirport.SingleOrDefault(x => x.Parent.Id == parentId)),
			ObjectType.Bridge => DtoObjectBridgeMapper.ToDto(db.ObjBridge.SingleOrDefault(x => x.Parent.Id == parentId)),
			ObjectType.Building => DtoObjectBuildingMapper.ToDto(db.ObjBuilding.SingleOrDefault(x => x.Parent.Id == parentId)),
			ObjectType.Cargo => DtoObjectCargoMapper.ToDto(db.ObjCargo.SingleOrDefault(x => x.Parent.Id == parentId)),
			ObjectType.CliffEdge => DtoObjectCliffEdgeMapper.ToDto(db.ObjCliffEdge.SingleOrDefault(x => x.Parent.Id == parentId)),
			ObjectType.Climate => DtoObjectClimateMapper.ToDto(db.ObjClimate.SingleOrDefault(x => x.Parent.Id == parentId)),
			ObjectType.Competitor => DtoObjectCompetitorMapper.ToDto(db.ObjCompetitor.SingleOrDefault(x => x.Parent.Id == parentId)),
			ObjectType.Currency => DtoObjectCurrencyMapper.ToDto(db.ObjCurrency.SingleOrDefault(x => x.Parent.Id == parentId)),
			ObjectType.Dock => DtoObjectDockMapper.ToDto(db.ObjDock.SingleOrDefault(x => x.Parent.Id == parentId)),
			ObjectType.HillShapes => DtoObjectHillShapesMapper.ToDto(db.ObjHillShapes.SingleOrDefault(x => x.Parent.Id == parentId)),
			ObjectType.Industry => DtoObjectIndustryMapper.ToDto(db.ObjIndustry.SingleOrDefault(x => x.Parent.Id == parentId)),
			ObjectType.InterfaceSkin => DtoObjectInterfaceMapper.ToDto(db.ObjInterface.SingleOrDefault(x => x.Parent.Id == parentId)),
			ObjectType.Land => DtoObjectLandMapper.ToDto(db.ObjLand.SingleOrDefault(x => x.Parent.Id == parentId)),
			ObjectType.LevelCrossing => DtoObjectLevelCrossingMapper.ToDto(db.ObjLevelCrossing.SingleOrDefault(x => x.Parent.Id == parentId)),
			ObjectType.Region => DtoObjectRegionMapper.ToDto(db.ObjRegion.SingleOrDefault(x => x.Parent.Id == parentId)),
			ObjectType.RoadExtra => DtoObjectRoadExtraMapper.ToDto(db.ObjRoadExtra.SingleOrDefault(x => x.Parent.Id == parentId)),
			ObjectType.Road => DtoObjectRoadMapper.ToDto(db.ObjRoad.SingleOrDefault(x => x.Parent.Id == parentId)),
			ObjectType.RoadStation => DtoObjectRoadStationMapper.ToDto(db.ObjRoadStation.SingleOrDefault(x => x.Parent.Id == parentId)),
			ObjectType.Scaffolding => DtoObjectScaffoldingMapper.ToDto(db.ObjScaffolding.SingleOrDefault(x => x.Parent.Id == parentId)),
			ObjectType.ScenarioText => DtoObjectScenarioTextMapper.ToDto(db.ObjScenarioText.SingleOrDefault(x => x.Parent.Id == parentId)),
			ObjectType.Snow => DtoObjectSnowMapper.ToDto(db.ObjSnow.SingleOrDefault(x => x.Parent.Id == parentId)),
			ObjectType.Sound => DtoObjectSoundMapper.ToDto(db.ObjSound.SingleOrDefault(x => x.Parent.Id == parentId)),
			ObjectType.Steam => DtoObjectSteamMapper.ToDto(db.ObjSteam.SingleOrDefault(x => x.Parent.Id == parentId)),
			ObjectType.StreetLight => DtoObjectStreetLightMapper.ToDto(db.ObjStreetLight.SingleOrDefault(x => x.Parent.Id == parentId)),
			ObjectType.TownNames => DtoObjectTownNamesMapper.ToDto(db.ObjTownNames.SingleOrDefault(x => x.Parent.Id == parentId)),
			ObjectType.TrackExtra => DtoObjectTrackExtraMapper.ToDto(db.ObjTrackExtra.SingleOrDefault(x => x.Parent.Id == parentId)),
			ObjectType.Track => DtoObjectTrackMapper.ToDto(db.ObjTrack.SingleOrDefault(x => x.Parent.Id == parentId)),
			ObjectType.TrackSignal => DtoObjectTrackSignalMapper.ToDto(db.ObjTrackSignal.SingleOrDefault(x => x.Parent.Id == parentId)),
			ObjectType.TrackStation => DtoObjectTrackStationMapper.ToDto(db.ObjTrackStation.SingleOrDefault(x => x.Parent.Id == parentId)),
			ObjectType.Tree => DtoObjectTreeMapper.ToDto(db.ObjTree.SingleOrDefault(x => x.Parent.Id == parentId)),
			ObjectType.Tunnel => DtoObjectTunnelMapper.ToDto(db.ObjTunnel.SingleOrDefault(x => x.Parent.Id == parentId)),
			ObjectType.Vehicle => DtoObjectVehicleMapper.ToDto(db.ObjVehicle.SingleOrDefault(x => x.Parent.Id == parentId)),
			ObjectType.Water => DtoObjectWaterMapper.ToDto(db.ObjWater.SingleOrDefault(x => x.Parent.Id == parentId)),
			ObjectType.Wall => DtoObjectWallMapper.ToDto(db.ObjWall.SingleOrDefault(x => x.Parent.Id == parentId)),
			_ => throw new NotImplementedException(),
		};

	public static dynamic GetDbSetForType(LocoDbContext db, ObjectType objectType)
	=> objectType switch
	{
		ObjectType.Airport => db.ObjAirport,
		ObjectType.Bridge => db.ObjBridge,
		ObjectType.Building => db.ObjBuilding,
		ObjectType.Cargo => db.ObjCargo,
		ObjectType.CliffEdge => db.ObjCliffEdge,
		ObjectType.Climate => db.ObjClimate,
		ObjectType.Competitor => db.ObjCompetitor,
		ObjectType.Currency => db.ObjCurrency,
		ObjectType.Dock => db.ObjDock,
		ObjectType.HillShapes => db.ObjHillShapes,
		ObjectType.Industry => db.ObjIndustry,
		ObjectType.InterfaceSkin => db.ObjInterface,
		ObjectType.Land => db.ObjLand,
		ObjectType.LevelCrossing => db.ObjLevelCrossing,
		ObjectType.Region => db.ObjRegion,
		ObjectType.RoadExtra => db.ObjRoadExtra,
		ObjectType.Road => db.ObjRoad,
		ObjectType.RoadStation => db.ObjRoadStation,
		ObjectType.Scaffolding => db.ObjScaffolding,
		ObjectType.ScenarioText => db.ObjScenarioText,
		ObjectType.Snow => db.ObjSnow,
		ObjectType.Sound => db.ObjSound,
		ObjectType.Steam => db.ObjSteam,
		ObjectType.StreetLight => db.ObjStreetLight,
		ObjectType.TownNames => db.ObjTownNames,
		ObjectType.TrackExtra => db.ObjTrackExtra,
		ObjectType.Track => db.ObjTrack,
		ObjectType.TrackSignal => db.ObjTrackSignal,
		ObjectType.TrackStation => db.ObjTrackStation,
		ObjectType.Tree => db.ObjTree,
		ObjectType.Tunnel => db.ObjTunnel,
		ObjectType.Vehicle => db.ObjVehicle,
		ObjectType.Water => db.ObjWater,
		ObjectType.Wall => db.ObjWall,
		_ => throw new NotImplementedException(),
	};
}
