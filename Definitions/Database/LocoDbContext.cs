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

	#region GameDataFiles

	public DbSet<TblMusic> Music => Set<TblMusic>();
	public DbSet<TblSoundEffect> SoundEffects => Set<TblSoundEffect>();
	public DbSet<TblTutorial> Tutorials => Set<TblTutorial>();
	public DbSet<TblGraphics> Graphics => Set<TblGraphics>();
	public DbSet<TblScenario> Scenarios => Set<TblScenario>();

	#endregion

	#region Packs

	public DbSet<TblObjectPack> ObjectPacks => Set<TblObjectPack>();
	public DbSet<TblScenarioPack> ScenarioPacks => Set<TblScenarioPack>();

	#endregion

	public LocoDbContext()
	{ }

	public LocoDbContext(DbContextOptions<LocoDbContext> options) : base(options)
	{ }

	//public const string DefaultDb = "Q:\\Games\\Locomotion\\Database\\loco-test.db";
	public const string DefaultDb = "Q:\\Games\\Locomotion\\Server\\loco.db";

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

		JsonColumnConvention.Configure(configurationBuilder);
	}

	protected override void OnModelCreating(ModelBuilder modelBuilder)
	{
		base.OnModelCreating(modelBuilder);

		// Complex object-model properties (lists, nested objects, dictionaries, jagged arrays) are stored as JSON.
		JsonColumnConvention.Apply(modelBuilder);

		_ = modelBuilder.Entity<TblObject>()
			.Property(b => b.UploadedDate)
			.HasDefaultValueSql("date('now')"); // this is necessary, it seems like a bug in sqlite
		_ = modelBuilder.Entity<TblScenario>()
			.Property(b => b.UploadedDate)
			.HasDefaultValueSql("date('now')"); // this is necessary, it seems like a bug in sqlite
		_ = modelBuilder.Entity<TblObjectPack>()
			.Property(b => b.UploadedDate)
			.HasDefaultValueSql("date('now')"); // this is necessary, it seems like a bug in sqlite
		_ = modelBuilder.Entity<TblScenarioPack>()
			.Property(b => b.UploadedDate)
			.HasDefaultValueSql("date('now')"); // this is necessary, it seems like a bug in sqlite
		_ = modelBuilder.Entity<TblMusic>()
			.Property(b => b.UploadedDate)
			.HasDefaultValueSql("date('now')"); // this is necessary, it seems like a bug in sqlite
		_ = modelBuilder.Entity<TblSoundEffect>()
			.Property(b => b.UploadedDate)
			.HasDefaultValueSql("date('now')"); // this is necessary, it seems like a bug in sqlite
		_ = modelBuilder.Entity<TblTutorial>()
			.Property(b => b.UploadedDate)
			.HasDefaultValueSql("date('now')"); // this is necessary, it seems like a bug in sqlite
		_ = modelBuilder.Entity<TblGraphics>()
			.Property(b => b.UploadedDate)
			.HasDefaultValueSql("date('now')"); // this is necessary, it seems like a bug in sqlite
	}

	public bool DoesObjectExist(string datName, uint datChecksum, out TblObject? existingObject)
	{
		// The (DatName, DatChecksum) index is deliberately not unique: two binary-different files can
		// share the same S5 name and checksum, so this returns the first match rather than requiring one.
		// xxHash3 (see DoesObjectWithHashExist) is the authoritative identity of a file.
		var existingEntityInDb = DatObjects
			.Where(e => e.DatName == datName && e.DatChecksum == datChecksum)
			.Select(e => e.Object)
			.FirstOrDefault();

		var existingEntityInChangeTracker = ChangeTracker.Entries()
			.Where(e => e.State == EntityState.Added && e.Entity.GetType() == typeof(TblDatObject))
			.Select(e => e.Entity as TblDatObject)
			.FirstOrDefault(e => e!.DatName == datName && e.DatChecksum == datChecksum)?.Object;

		existingObject = existingEntityInDb ?? existingEntityInChangeTracker;
		return existingObject != null;
	}

	/// <summary>
	/// Finds the object already backed by a file with the given whole-file hash. <c>xxHash3</c> is the
	/// authoritative file identity, so a match means the file is a duplicate of one we already have and
	/// no new object/row should be created for it.
	/// </summary>
	public bool DoesObjectWithHashExist(ulong xxHash3, out TblObject? existingObject)
	{
		existingObject = DatObjects
			.Where(e => e.xxHash3 == xxHash3)
			.Select(e => e.Object)
			.FirstOrDefault();

		return existingObject != null;
	}

	/// <summary>
	/// Builds an object name from the S5 name and checksum. Because several binary-different objects may
	/// share that pair, the whole-file hash is appended when the plain name is already taken so the
	/// unique <c>Objects.Name</c> constraint still holds.
	/// </summary>
	public Task<string> GetUniqueObjectNameAsync(string s5Name, uint checksum, ulong xxHash3, CancellationToken ct = default)
	{
		var baseName = $"{s5Name}_{checksum}";
		return GetUniqueObjectNameCoreAsync(baseName, xxHash3, ct);
	}

	private async Task<string> GetUniqueObjectNameCoreAsync(string baseName, ulong xxHash3, CancellationToken ct)
	{
		if (!await Objects.AnyAsync(x => x.Name == baseName, ct))
		{
			return baseName;
		}

		var disambiguated = $"{baseName}_{xxHash3}";
		for (var suffix = 0; await Objects.AnyAsync(x => x.Name == disambiguated, ct); suffix++)
		{
			disambiguated = $"{baseName}_{xxHash3}_{suffix}";
		}

		return disambiguated;
	}
}
