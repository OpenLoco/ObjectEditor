using Microsoft.EntityFrameworkCore;
using OpenLoco.Dat.Data;
using OpenLoco.Dat.Objects;
using OpenLoco.Dat.Types;

namespace OpenLoco.Definitions.Database
{
	public interface IConvertibleToTable<TTable, TDat>
	{
		static abstract TTable FromObject(TblObject tblObj, TDat datObo);
	}

	public interface IDtoSubObject
	{
		//IDbSubObject ToTbl();
	}

	public interface IDbSubObject
	{
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
			where TSubObject : DbSubObject, IConvertibleToTable<TSubObject, TDat>
			where TDat : ILocoStruct
		{
			var subObj = TSubObject.FromObject(parentObj, (TDat)datObj);

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

		public static async Task<string> Update(LocoDbContext db, TblObject obj, ILocoStruct locoStruct)
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
				_ => "unknown object type",
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
				_ => "unknown object type",
			};
	}
}
