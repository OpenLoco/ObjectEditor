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

		public DbSet<TblObjectTagLink> ObjectTagLinks => Set<TblObjectTagLink>();

		public string DbPath { get; }

		protected override void OnConfiguring(DbContextOptionsBuilder options)
			=> options.UseSqlite($"Data Source={DbPath}");

		protected override void OnModelCreating(ModelBuilder modelBuilder)
		{
			modelBuilder.Entity<TblObjectTagLink>()
				.HasKey(ba => new { ba.TblLocoObjectId, ba.TblTagId });

			modelBuilder.Entity<TblObjectTagLink>()

				.HasOne(ba => ba.Object)
				.WithMany(b => b.TagLinks)
				.HasForeignKey(ba => ba.TblLocoObjectId);

			modelBuilder.Entity<TblObjectTagLink>()
				.HasOne(ba => ba.Tag)
				.WithMany(a => a.TagLinks)
				.HasForeignKey(ba => ba.TblTagId);
		}

		public static string GetDbPath()
			=> Path.Join(
				Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData),
				"OpenLoco Object Editor",
				"loco.db");
	}
}
