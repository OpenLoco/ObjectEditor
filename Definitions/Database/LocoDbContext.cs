using Definitions.Database.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using OpenLoco.Dat.Types;

namespace OpenLoco.Definitions.Database
{
	public class LocoDbContext : IdentityDbContext<TblUser, TblUserRole, DbKey>
	{
		#region ReferenceData

		public DbSet<TblAuthor> Authors => Set<TblAuthor>();
		public DbSet<TblTag> Tags => Set<TblTag>();
		public DbSet<TblLicence> Licences => Set<TblLicence>();

		#endregion

		#region ObjectData

		public DbSet<TblObject> Objects => Set<TblObject>();
		public DbSet<TblStringTable> StringTable => Set<TblStringTable>();
		public DbSet<TblDatObject> DatObjects => Set<TblDatObject>();
		public DbSet<TblObjectPack> ObjectPacks => Set<TblObjectPack>();
		public DbSet<TblSC5File> SC5Files => Set<TblSC5File>();
		public DbSet<TblSC5FilePack> SC5FilePacks => Set<TblSC5FilePack>();

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

			// for the int->guid pk transition
			//_ = modelBuilder.Entity<TblDatObject>()
			//	.HasAlternateKey(x => x.GuidId);
			//_ = modelBuilder.Entity<TblObject>()
			//	.HasAlternateKey(x => x.GuidId);
			//_ = modelBuilder.Entity<TblObjectPack>()
			//	.HasAlternateKey(x => x.GuidId);
			//_ = modelBuilder.Entity<TblSC5File>()
			//	.HasAlternateKey(x => x.GuidId);
			//_ = modelBuilder.Entity<TblSC5FilePack>()
			//	.HasAlternateKey(x => x.GuidId);
			//_ = modelBuilder.Entity<TblAuthor>()
			//	.HasAlternateKey(x => x.GuidId);
			//_ = modelBuilder.Entity<TblLicence>()
			//	.HasAlternateKey(x => x.GuidId);
			//_ = modelBuilder.Entity<TblTag>()
			//	.HasAlternateKey(x => x.GuidId);

			//_ = modelBuilder.Entity<TblStringTable>()
			//	.Property(x => x.GuidId)
			//	.HasDefaultValueSql("NEWID()");
			//_ = modelBuilder.Entity<TblDatObject>()
			//	.Property(x => x.GuidId)
			//	.HasDefaultValueSql("NEWID()");
			//_ = modelBuilder.Entity<TblObject>()
			//	.Property(x => x.GuidId)
			//	.HasDefaultValueSql("NEWID()");
			//_ = modelBuilder.Entity<TblObjectPack>()
			//	.Property(x => x.GuidId)
			//	.HasDefaultValueSql("NEWID()");
			//_ = modelBuilder.Entity<TblSC5File>()
			//	.Property(x => x.GuidId)
			//	.HasDefaultValueSql("NEWID()");
			//_ = modelBuilder.Entity<TblSC5FilePack>()
			//	.Property(x => x.GuidId)
			//	.HasDefaultValueSql("NEWID()");
			//_ = modelBuilder.Entity<TblAuthor>()
			//	.Property(x => x.GuidId)
			//	.HasDefaultValueSql("NEWID()");
			//_ = modelBuilder.Entity<TblLicence>()
			//	.Property(x => x.GuidId)
			//	.HasDefaultValueSql("NEWID()");
			//_ = modelBuilder.Entity<TblTag>()
			//	.Property(x => x.GuidId)
			//	.HasDefaultValueSql("NEWID()");
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
