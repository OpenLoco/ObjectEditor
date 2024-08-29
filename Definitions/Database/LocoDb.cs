using Microsoft.EntityFrameworkCore;
using OpenLoco.Dat.Types;

namespace OpenLoco.Definitions.Database
{
	public class LocoDb : DbContext
	{
		public DbSet<TblAuthor> Authors => Set<TblAuthor>();
		public DbSet<TblTag> Tags => Set<TblTag>();
		public DbSet<TblModpack> Modpacks => Set<TblModpack>();
		public DbSet<TblLocoObject> Objects => Set<TblLocoObject>();
		public DbSet<TblLicence> Licences => Set<TblLicence>();

		public LocoDb()
		{ }

		public LocoDb(DbContextOptions<LocoDb> options) : base(options)
		{ }

		protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
		{
			if (!optionsBuilder.IsConfigured)
			{
				_ = optionsBuilder.UseSqlite("Data Source=Q:\\Games\\Locomotion\\Server\\loco.db");
			}
		}

		protected override void OnModelCreating(ModelBuilder modelBuilder) => modelBuilder.Entity<TblLocoObject>()
			.Property(b => b.UploadDate)
			.HasDefaultValueSql("datetime(datetime('now', 'localtime'), 'utc')"); // this is necessary, it seems like a bug in sqlite

		public bool DoesObjectExist(S5Header s5Header, out TblLocoObject? existingObject)
		 => DoesObjectExist(s5Header.Name, s5Header.Checksum, out existingObject);

		public bool DoesObjectExist(string originalName, uint originalChecksum, out TblLocoObject? existingObject)
		{
			// there's a unique constraint on the composite key index (OriginalName, OriginalChecksum), so check existence first so no exceptions
			// this isn't necessary since we're already filtering in LINQ, but if we were adding to a non-empty database, this would be necessary
			var existingEntityInDb = Objects
				.FirstOrDefault(e => e.OriginalName == originalName && e.OriginalChecksum == originalChecksum);

			var existingEntityInChangeTracker = ChangeTracker.Entries()
				.Where(e => e.State == EntityState.Added && e.Entity.GetType() == typeof(TblLocoObject))
				.Select(e => e.Entity as TblLocoObject)
				.FirstOrDefault(e => e!.OriginalName == originalName && e.OriginalChecksum == originalChecksum);

			existingObject = existingEntityInDb ?? existingEntityInChangeTracker;
			return existingObject != null;
		}
	}
}
