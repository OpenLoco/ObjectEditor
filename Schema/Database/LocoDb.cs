using Microsoft.EntityFrameworkCore;

namespace OpenLoco.Schema.Database
{
	public class LocoDb : DbContext
	{
		public LocoDb() => DbPath = GetDbPath();

		public LocoDb(DbContextOptions<LocoDb> options)
			: base(options) =>
			DbPath = GetDbPath();

		public DbSet<TblAuthor> Authors => Set<TblAuthor>();
		public DbSet<TblTag> Tags => Set<TblTag>();
		public DbSet<TblModpack> Modpacks => Set<TblModpack>();
		public DbSet<TblLocoObject> Objects => Set<TblLocoObject>();
		public DbSet<TblLicence> Licences => Set<TblLicence>();

		public string DbPath { get; }

		protected override void OnConfiguring(DbContextOptionsBuilder options)
			=> options.UseSqlite($"Data Source={DbPath}");

		public static string GetDbPath()
			=> Path.Join(
				Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData),
				"OpenLoco Object Editor",
				"loco.db");

		public bool DoesObjectExist(string originalName, uint originalChecksum)
		{
			// there's a unique constraint on the composite key index (OriginalName, OriginalChecksum), so check existence first so no exceptions
			// this isn't necessary since we're already filtering in LINQ, but if we were adding to a non-empty database, this would be necessary
			var existingEntityInDb = Objects
				.FirstOrDefault(e => e.OriginalName == originalName && e.OriginalChecksum == originalChecksum);

			var existingEntityInChangeTracker = ChangeTracker.Entries()
				.Where(e => e.State == EntityState.Added && e.Entity.GetType() == typeof(TblLocoObject))
				.Select(e => e.Entity as TblLocoObject)
				.FirstOrDefault(e => e!.OriginalName == originalName && e.OriginalChecksum == originalChecksum);

			return existingEntityInDb != null || existingEntityInChangeTracker != null;

		}
	}
}
