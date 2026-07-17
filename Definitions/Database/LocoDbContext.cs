using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace Definitions.Database;

public class LocoDbContext : IdentityDbContext<TblUser, TblUserRole, UniqueObjectId>
{
	#region ReferenceData

	public DbSet<TblAuthor> Authors => Set<TblAuthor>();
	public DbSet<TblTag> Tags => Set<TblTag>();
	public DbSet<TblLicence> Licences => Set<TblLicence>();

	#endregion

	#region ObjectData

	public DbSet<TblObject> Objects => Set<TblObject>();
	public DbSet<TblStringTableRow> StringTable => Set<TblStringTableRow>();
	public DbSet<TblDatObject> DatObjects => Set<TblDatObject>();
	public DbSet<TblObjectMissing> ObjectsMissing => Set<TblObjectMissing>();

	#region Objects

	public DbSet<TblObjectAirport> ObjAirport => Set<TblObjectAirport>();

	public DbSet<TblObjectBridge> ObjBridge => Set<TblObjectBridge>();

	public DbSet<TblObjectBuilding> ObjBuilding => Set<TblObjectBuilding>();

	public DbSet<TblObjectCargo> ObjCargo => Set<TblObjectCargo>();

	public DbSet<TblObjectCliffEdge> ObjCliffEdge => Set<TblObjectCliffEdge>();

	public DbSet<TblObjectClimate> ObjClimate => Set<TblObjectClimate>();

	public DbSet<TblObjectCompetitor> ObjCompetitor => Set<TblObjectCompetitor>();

	public DbSet<TblObjectCurrency> ObjCurrency => Set<TblObjectCurrency>();

	public DbSet<TblObjectDock> ObjDock => Set<TblObjectDock>();

	public DbSet<TblObjectHillShapes> ObjHillShapes => Set<TblObjectHillShapes>();

	public DbSet<TblObjectIndustry> ObjIndustry => Set<TblObjectIndustry>();

	public DbSet<TblObjectInterface> ObjInterface => Set<TblObjectInterface>();

	public DbSet<TblObjectLand> ObjLand => Set<TblObjectLand>();

	public DbSet<TblObjectLevelCrossing> ObjLevelCrossing => Set<TblObjectLevelCrossing>();

	public DbSet<TblObjectRegion> ObjRegion => Set<TblObjectRegion>();

	public DbSet<TblObjectRoadExtra> ObjRoadExtra => Set<TblObjectRoadExtra>();

	public DbSet<TblObjectRoad> ObjRoad => Set<TblObjectRoad>();

	public DbSet<TblObjectRoadStation> ObjRoadStation => Set<TblObjectRoadStation>();

	public DbSet<TblObjectScaffolding> ObjScaffolding => Set<TblObjectScaffolding>();

	public DbSet<TblObjectScenarioText> ObjScenarioText => Set<TblObjectScenarioText>();

	public DbSet<TblObjectSnow> ObjSnow => Set<TblObjectSnow>();

	public DbSet<TblObjectSound> ObjSound => Set<TblObjectSound>();

	public DbSet<TblObjectSteam> ObjSteam => Set<TblObjectSteam>();

	public DbSet<TblObjectStreetLight> ObjStreetLight => Set<TblObjectStreetLight>();

	public DbSet<TblObjectTownNames> ObjTownNames => Set<TblObjectTownNames>();

	public DbSet<TblObjectTrackExtra> ObjTrackExtra => Set<TblObjectTrackExtra>();

	public DbSet<TblObjectTrack> ObjTrack => Set<TblObjectTrack>();

	public DbSet<TblObjectTrackSignal> ObjTrackSignal => Set<TblObjectTrackSignal>();

	public DbSet<TblObjectTrackStation> ObjTrackStation => Set<TblObjectTrackStation>();

	public DbSet<TblObjectTree> ObjTree => Set<TblObjectTree>();

	public DbSet<TblObjectTunnel> ObjTunnel => Set<TblObjectTunnel>();

	public DbSet<TblObjectVehicle> ObjVehicle => Set<TblObjectVehicle>();

	public DbSet<TblObjectWall> ObjWall => Set<TblObjectWall>();

	public DbSet<TblObjectWater> ObjWater => Set<TblObjectWater>();

	#endregion

	#endregion

	#region JunctionTables

	public DbSet<TblBridgeCompatibleTrack> BridgeCompatibleTracks => Set<TblBridgeCompatibleTrack>();
	public DbSet<TblBridgeCompatibleRoad> BridgeCompatibleRoads => Set<TblBridgeCompatibleRoad>();
	public DbSet<TblVehicleCompatibleVehicle> VehicleCompatibleVehicles => Set<TblVehicleCompatibleVehicle>();
	public DbSet<TblVehicleRequiredTrackExtra> VehicleRequiredTrackExtras => Set<TblVehicleRequiredTrackExtra>();
	public DbSet<TblVehicleStartSound> VehicleStartSounds => Set<TblVehicleStartSound>();
	public DbSet<TblBuildingProducedCargo> BuildingProducedCargos => Set<TblBuildingProducedCargo>();
	public DbSet<TblBuildingConsumedCargo> BuildingConsumedCargos => Set<TblBuildingConsumedCargo>();
	public DbSet<TblIndustryProducedCargo> IndustryProducedCargos => Set<TblIndustryProducedCargo>();
	public DbSet<TblIndustryRequiredCargo> IndustryRequiredCargos => Set<TblIndustryRequiredCargo>();
	public DbSet<TblIndustryWallType> IndustryWallTypes => Set<TblIndustryWallType>();
	public DbSet<TblTrackTrackMod> TrackTrackMods => Set<TblTrackTrackMod>();
	public DbSet<TblTrackSignal> TrackSignals => Set<TblTrackSignal>();
	public DbSet<TblTrackCompatibleTrackRoad> TrackCompatibleTrackRoads => Set<TblTrackCompatibleTrackRoad>();
	public DbSet<TblTrackBridge> TrackBridges => Set<TblTrackBridge>();
	public DbSet<TblTrackStation> TrackStations => Set<TblTrackStation>();
	public DbSet<TblRoadBridge> RoadBridges => Set<TblRoadBridge>();
	public DbSet<TblRoadStation> RoadStations => Set<TblRoadStation>();
	public DbSet<TblRoadRoadMod> RoadRoadMods => Set<TblRoadRoadMod>();
	public DbSet<TblRoadCompatibleTrackRoad> RoadCompatibleTrackRoads => Set<TblRoadCompatibleTrackRoad>();
	public DbSet<TblTrackStationCompatibleTrack> TrackStationCompatibleTracks => Set<TblTrackStationCompatibleTrack>();
	public DbSet<TblTrackSignalCompatibleTrack> TrackSignalCompatibleTracks => Set<TblTrackSignalCompatibleTrack>();
	public DbSet<TblRoadStationCompatibleRoad> RoadStationCompatibleRoads => Set<TblRoadStationCompatibleRoad>();
	public DbSet<TblRegionCargoInfluence> RegionCargoInfluences => Set<TblRegionCargoInfluence>();
	public DbSet<TblRegionDependentObject> RegionDependentObjects => Set<TblRegionDependentObject>();
	public DbSet<TblSteamSoundEffect> SteamSoundEffects => Set<TblSteamSoundEffect>();

	#endregion

	#region NormalizedTables

	public DbSet<TblVehicleEmitterAnimation> VehicleEmitterAnimations => Set<TblVehicleEmitterAnimation>();

	#endregion

	#region Other

	public DbSet<TblObjectPack> ObjectPacks => Set<TblObjectPack>();
	public DbSet<TblSC5File> SC5Files => Set<TblSC5File>();
	public DbSet<TblSC5FilePack> SC5FilePacks => Set<TblSC5FilePack>();

	#endregion

	public LocoDbContext()
	{ }

	public LocoDbContext(DbContextOptions<LocoDbContext> options) : base(options)
	{ }

	public const string DefaultDb = "Q:\\Games\\Locomotion\\Database\\loco-test.db";

	protected override void OnConfiguring(DbContextOptionsBuilder builder)
	{
		if (!builder.IsConfigured)
		{
			_ = builder.UseSqlite($"Data Source={DefaultDb}");
		}
	}

	public static LocoDbContext? GetDbFromFile(string path) // path is the full/absolute file path
	{
		if (!string.IsNullOrEmpty(path) && File.Exists(path))
		{
			var builder = new DbContextOptionsBuilder<LocoDbContext>();
			_ = builder.UseSqlite($"Data Source={path}");
			return new LocoDbContext(builder.Options);
		}

		return null;
	}

	protected override void ConfigureConventions(ModelConfigurationBuilder configurationBuilder)
	{
		base.ConfigureConventions(configurationBuilder);
		// Store all enum values as text strings
		_ = configurationBuilder.Properties<Enum>().HaveConversion<string>();
	}

	protected override void OnModelCreating(ModelBuilder modelBuilder)
	{
		base.OnModelCreating(modelBuilder);

		//_ = modelBuilder.Entity<TblObject>()
		//	.HasAlternateKey(o => new { o.SubObjectId, o.ObjectType });

		// Configure the one-to-many relationship
		//modelBuilder.Entity<OrderItem>()
		//	.HasOne(oi => oi.Order) // OrderItem has one Order
		//	.WithMany(o => o.OrderItems) // Order has many OrderItems
		//	.HasForeignKey(oi => new { oi.OrderNumber, oi.CustomerCode }); // The composite foreign key on OrderItem

		_ = modelBuilder.Entity<TblObject>()
			.Property(b => b.UploadedDate)
			.HasDefaultValueSql("date('now')"); // this is necessary, it seems like a bug in sqlite
		_ = modelBuilder.Entity<TblSC5File>()
			.Property(b => b.UploadedDate)
			.HasDefaultValueSql("date('now')"); // this is necessary, it seems like a bug in sqlite
		_ = modelBuilder.Entity<TblObjectPack>()
			.Property(b => b.UploadedDate)
			.HasDefaultValueSql("date('now')"); // this is necessary, it seems like a bug in sqlite
		_ = modelBuilder.Entity<TblSC5FilePack>()
			.Property(b => b.UploadedDate)
			.HasDefaultValueSql("date('now')"); // this is necessary, it seems like a bug in sqlite

		// Junction table composite keys
		_ = modelBuilder.Entity<TblBridgeCompatibleTrack>().HasKey(x => new { x.BridgeId, x.TrackId });
		_ = modelBuilder.Entity<TblBridgeCompatibleRoad>().HasKey(x => new { x.BridgeId, x.RoadId });
		_ = modelBuilder.Entity<TblVehicleCompatibleVehicle>().HasKey(x => new { x.VehicleId, x.CompatibleVehicleId });
		_ = modelBuilder.Entity<TblVehicleRequiredTrackExtra>().HasKey(x => new { x.VehicleId, x.TrackExtraId });
		_ = modelBuilder.Entity<TblVehicleStartSound>().HasKey(x => new { x.VehicleId, x.SoundId });
		_ = modelBuilder.Entity<TblBuildingProducedCargo>().HasKey(x => new { x.BuildingId, x.CargoId });
		_ = modelBuilder.Entity<TblBuildingConsumedCargo>().HasKey(x => new { x.BuildingId, x.CargoId });
		_ = modelBuilder.Entity<TblIndustryProducedCargo>().HasKey(x => new { x.IndustryId, x.CargoId });
		_ = modelBuilder.Entity<TblIndustryRequiredCargo>().HasKey(x => new { x.IndustryId, x.CargoId });
		_ = modelBuilder.Entity<TblIndustryWallType>().HasKey(x => new { x.IndustryId, x.WallId });
		_ = modelBuilder.Entity<TblTrackTrackMod>().HasKey(x => new { x.TrackId, x.TrackExtraId });
		_ = modelBuilder.Entity<TblTrackSignal>().HasKey(x => new { x.TrackId, x.TrackSignalId });
		_ = modelBuilder.Entity<TblTrackCompatibleTrackRoad>().HasKey(x => new { x.TrackId, x.CompatibleTrackRoadId });
		_ = modelBuilder.Entity<TblTrackBridge>().HasKey(x => new { x.TrackId, x.BridgeId });
		_ = modelBuilder.Entity<TblTrackStation>().HasKey(x => new { x.TrackId, x.TrackStationId });
		_ = modelBuilder.Entity<TblRoadBridge>().HasKey(x => new { x.RoadId, x.BridgeId });
		_ = modelBuilder.Entity<TblRoadStation>().HasKey(x => new { x.RoadId, x.RoadStationId });
		_ = modelBuilder.Entity<TblRoadRoadMod>().HasKey(x => new { x.RoadId, x.RoadExtraId });
		_ = modelBuilder.Entity<TblRoadCompatibleTrackRoad>().HasKey(x => new { x.RoadId, x.CompatibleTrackRoadId });
		_ = modelBuilder.Entity<TblTrackStationCompatibleTrack>().HasKey(x => new { x.TrackStationId, x.TrackId });
		_ = modelBuilder.Entity<TblTrackSignalCompatibleTrack>().HasKey(x => new { x.TrackSignalId, x.TrackId });
		_ = modelBuilder.Entity<TblRoadStationCompatibleRoad>().HasKey(x => new { x.RoadStationId, x.RoadId });
		_ = modelBuilder.Entity<TblRegionCargoInfluence>().HasKey(x => new { x.RegionId, x.CargoId });
		_ = modelBuilder.Entity<TblRegionDependentObject>().HasKey(x => new { x.RegionId, x.DependentObjectId });
		_ = modelBuilder.Entity<TblSteamSoundEffect>().HasKey(x => new { x.SteamId, x.SoundId });
	}

	public bool DoesObjectExist(string datName, uint datChecksum, out TblObject? existingObject)
	{
		// there's a unique constraint on the composite key index (DatName, DatChecksum), so check existence first so no exceptions
		// this isn't necessary since we're already filtering in LINQ, but if we were adding to a non-empty database, this would be necessary
		var existingEntityInDb = DatObjects
			.SingleOrDefault(e => e.DatName == datName && e.DatChecksum == datChecksum)?.Object;

		var existingEntityInChangeTracker = ChangeTracker.Entries()
			.Where(e => e.State == EntityState.Added && e.Entity.GetType() == typeof(TblDatObject))
			.Select(e => e.Entity as TblDatObject)
			.SingleOrDefault(e => e!.DatName == datName && e.DatChecksum == datChecksum)?.Object;

		existingObject = existingEntityInDb ?? existingEntityInChangeTracker;
		return existingObject != null;
	}
}
