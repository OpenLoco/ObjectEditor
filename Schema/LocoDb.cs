using Microsoft.EntityFrameworkCore;

namespace OpenLoco.Db.Schema
{
	public class LocoDb : DbContext
	{
		public LocoDb()
		{
			DbPath = GetDbPath();
		}

		public LocoDb(DbContextOptions<LocoDb> options)
			: base(options)
		{
			//var folder = Environment.SpecialFolder.LocalApplicationData;
			//var path = Environment.GetFolderPath(folder);
			DbPath = GetDbPath();
		}

		public DbSet<TblAuthor> Authors => Set<TblAuthor>();
		public DbSet<TblTag> Tags => Set<TblTag>();
		public DbSet<TblLocoObject> Objects => Set<TblLocoObject>();

		//public DbSet<TblObjectTagLink> ObjectTagLinks => Set<TblObjectTagLink>();

		public string DbPath { get; }

		protected override void OnConfiguring(DbContextOptionsBuilder options)
			=> options.UseSqlite($"Data Source={DbPath}");

		//protected override void OnModelCreating(ModelBuilder modelBuilder)
		//{
		//	// Configure the many-to-many relationship
		//	modelBuilder.Entity<TblObjectTagLink>()
		//		.HasKey(ft => new { ft.ObjectId, ft.TagId });

		//	modelBuilder.Entity<TblObjectTagLink>()
		//		.HasOne(ft => ft.Object)
		//		.WithMany(f => f.Tags)
		//		.HasForeignKey(ft => ft.ObjectId);

		//	modelBuilder.Entity<TblObjectTagLink>()
		//		.HasOne(ft => ft.Tag)
		//		.WithMany() // No navigation property on Tag to Foo
		//		.HasForeignKey(ft => ft.TagId);

		//	base.OnModelCreating(modelBuilder);
		//}

		public static string GetDbPath()
			=> Path.Join(
				Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData),
				"OpenLoco Object Editor",
				"loco.db");
	}
}
