using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using OpenLoco.Dat.Types;

namespace OpenLoco.Definitions.Database
{
	public class TblUser : IdentityUser<Guid>
	{ }

	public class TblUserRole : IdentityRole<Guid>
	{ }

	public record DtoRoleCreate(
		string Name);

	public record DtoRoleModify(
		Guid Id,
		string Name);

	public class LocoDbContext : IdentityDbContext<TblUser, TblUserRole, Guid>
	{
		#region ReferenceData

		public DbSet<TblAuthor> Authors => Set<TblAuthor>();
		public DbSet<TblTag> Tags => Set<TblTag>();
		public DbSet<TblLicence> Licences => Set<TblLicence>();

		#endregion

		#region UserData

		public DbSet<TblObject> Objects => Set<TblObject>();
		public DbSet<TblDatObject> DatObjects => Set<TblDatObject>();
		public DbSet<TblObjectPack> ObjectPacks => Set<TblObjectPack>();
		public DbSet<TblSC5File> SC5Files => Set<TblSC5File>();
		public DbSet<TblSC5FilePack> SC5FilePacks => Set<TblSC5FilePack>();

		#endregion

		#region Identity

		//public DbSet<TblIdentityUser> IdentityUsers => Set<TblIdentityUser>();

		//public DbSet<TblIdentityRole> IdentityRoles => Set<TblIdentityRole>();

		#endregion

		public LocoDbContext()
		{ }

		public LocoDbContext(DbContextOptions<LocoDbContext> options) : base(options)
		{ }

		public static string DefaultDb = "Q:\\Games\\Locomotion\\Database\\loco-test.db";

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

		protected override void OnModelCreating(ModelBuilder modelBuilder)
		{
			base.OnModelCreating(modelBuilder);

			_ = modelBuilder.Entity<TblObject>()
				.Property(b => b.UploadedDate)
				.HasDefaultValueSql("datetime(datetime('now', 'localtime'), 'utc')"); // this is necessary, it seems like a bug in sqlite
			_ = modelBuilder.Entity<TblSC5File>()
				.Property(b => b.UploadedDate)
				.HasDefaultValueSql("datetime(datetime('now', 'localtime'), 'utc')"); // this is necessary, it seems like a bug in sqlite
			_ = modelBuilder.Entity<TblObjectPack>()
				.Property(b => b.UploadedDate)
				.HasDefaultValueSql("datetime(datetime('now', 'localtime'), 'utc')"); // this is necessary, it seems like a bug in sqlite
			_ = modelBuilder.Entity<TblSC5FilePack>()
				.Property(b => b.UploadedDate)
				.HasDefaultValueSql("datetime(datetime('now', 'localtime'), 'utc')"); // this is necessary, it seems like a bug in sqlite
		}

		public bool DoesObjectExist(S5Header s5Header, out TblObject? existingObject)
		 => DoesObjectExist(s5Header.Name, s5Header.Checksum, out existingObject);

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
}
