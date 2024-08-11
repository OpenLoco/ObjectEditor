using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace ObjectDatabase
{
	[PrimaryKey("PK")]
	public class DatObject
	{
		Guid PK { get; set; }

		string Name { get; set; }

		[ForeignKey("Author")]
		Author Author { get; set; }
	}

	[PrimaryKey("Name")]
	public class Tag
	{
		public string Name { get; set; }
	}

	public class ObjectTagJunction
	{
		[ForeignKey("Author")]
		public Author Author { get; set; }

		[ForeignKey("Tag")]
		public Tag Tag { get; set; }
	}

	[PrimaryKey("Name")]
	public class Author
	{
		public string Name { get; set; }
	}

	public class DatObjectServiceContext : DbContext
	{
		public DbSet<Blog> Blogs { get; set; }
		public DbSet<Post> Posts { get; set; }

		public DbSet<DatObject> Objects { get; set; }
		public DbSet<Author> Authors { get; set; }
		public DbSet<Tag> Tags { get; set; }
		public DbSet<ObjectTagJunction> ObjectTagJunctions { get; set; }

		public string DbPath { get; }

		public DatObjectServiceContext()
		{
			var folder = Environment.SpecialFolder.LocalApplicationData;
			var path = Environment.GetFolderPath(folder);
			DbPath = System.IO.Path.Join(path, "blogging.db");
		}

		// The following configures EF to create a Sqlite database file in the
		// special "local" folder for your platform.
		protected override void OnConfiguring(DbContextOptionsBuilder options)
			=> options.UseSqlite($"Data Source={DbPath}");

		protected override void OnModelCreating(ModelBuilder modelBuilder)
		{
			modelBuilder.Entity<ObjectTagJunction>()
				.HasMany(o => o.Tag)
				.WithMany(t => t.DatObject)
				.UsingEntity(j => j.ToTable("ObjectTags")); // Specify the name of the junction table (if needed)

			//base.OnModelCreating(modelBuilder);
		}
	}
}
