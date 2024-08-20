using Microsoft.EntityFrameworkCore;

namespace OpenLoco.Db.Schema
{
	public class LocoDb : DbContext
	{
		public LocoDb() => DbPath = GetDbPath();

		public LocoDb(DbContextOptions<LocoDb> options)
			: base(options) =>
			//var folder = Environment.SpecialFolder.LocalApplicationData;
			//var path = Environment.GetFolderPath(folder);
			DbPath = GetDbPath();

		public DbSet<TblAuthor> Authors => Set<TblAuthor>();
		public DbSet<TblTag> Tags => Set<TblTag>();
		public DbSet<TblLocoObject> Objects => Set<TblLocoObject>();

		public DbSet<TblObjectTagLink> ObjectTagLinks => Set<TblObjectTagLink>();

		public string DbPath { get; }

		protected override void OnConfiguring(DbContextOptionsBuilder options)
			=> options.UseSqlite($"Data Source={DbPath}");

		protected override void OnModelCreating(ModelBuilder modelBuilder)
		{
			#region TagLinks

			_ = modelBuilder.Entity<TblObjectTagLink>()
				.HasKey(ba => new { ba.TblLocoObjectId, ba.TblTagId });

			_ = modelBuilder.Entity<TblObjectTagLink>()

				.HasOne(ba => ba.Object)
				.WithMany(b => b.TagLinks)
				.HasForeignKey(ba => ba.TblLocoObjectId);

			_ = modelBuilder.Entity<TblObjectTagLink>()
				.HasOne(ba => ba.Tag)
				.WithMany(a => a.TagLinks)
				.HasForeignKey(ba => ba.TblTagId);

			#endregion

			#region Modpack Links

			_ = modelBuilder.Entity<TblModpackTagLink>()
				.HasKey(ba => new { ba.TblLocoObjectId, ba.TblModpackId });

			_ = modelBuilder.Entity<TblModpackTagLink>()
				.HasOne(ba => ba.Object)
				.WithMany(b => b.ModpackLinks)
				.HasForeignKey(ba => ba.TblLocoObjectId);

			_ = modelBuilder.Entity<TblModpackTagLink>()
				.HasOne(ba => ba.Modpack)
				.WithMany(a => a.ModpackLinks)
				.HasForeignKey(ba => ba.TblModpackId);

			#endregion
		}

		public static string GetDbPath()
			=> Path.Join(
				Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData),
				"OpenLoco Object Editor",
				"loco.db");
	}
}
