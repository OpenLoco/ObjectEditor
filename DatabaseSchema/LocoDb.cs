using Microsoft.EntityFrameworkCore;

namespace OpenLoco.Db.Schema
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
	}
}
