using Dat;
using Microsoft.EntityFrameworkCore;
using OpenLoco.Common;
using OpenLoco.Dat.FileParsing;
using OpenLoco.Schema;
using OpenLoco.Schema.Database;
using System.Text.Json;


using var db = ExampleRun();

Console.WriteLine("done");
Console.ReadLine();

static void SeedDb(LocoDb db, bool deleteExisting)
{
	if (deleteExisting)
	{
		Console.WriteLine("Clearing database");
		_ = db.Objects.ExecuteDelete();
		_ = db.Authors.ExecuteDelete();
		_ = db.Tags.ExecuteDelete();
		_ = db.Modpacks.ExecuteDelete();
		_ = db.Licences.ExecuteDelete();
		_ = db.SaveChanges(); // not necessary since ExecuteDelete auto-saves
	}

	const string ObjDirectory = "Q:\\Games\\Locomotion\\Server";
	var allDatFiles = SawyerStreamUtils.GetDatFilesInDirectory(ObjDirectory);

	Console.WriteLine("Loading metadata");
	const string MetadataFile = "Q:\\Games\\Locomotion\\LocoVault\\database_new.json";
	var metadata = LoadMetadata(MetadataFile);

	// ...

	Console.WriteLine("Seeding");
	if (!db.Authors.Any())
	{
		Console.WriteLine("Seeding Authors");
		var authors = metadata.Values.Select(x => x.Creator).Distinct();
		foreach (var author in authors.Where(x => !string.IsNullOrEmpty(x)))
		{
			_ = db.Add(new TblAuthor() { Name = author });
		}
		_ = db.SaveChanges();
	}

	// ...

	if (!db.Tags.Any())
	{
		Console.WriteLine("Seeding Tags");
		var tags = metadata.Values.Select(x => x.Tags).SelectMany(x => x).Distinct();
		foreach (var tag in tags.Where(x => !string.IsNullOrEmpty(x)))
		{
			_ = db.Add(new TblTag() { Name = tag });
		}
		_ = db.SaveChanges();
	}

	// ...

	if (!db.Objects.Any())
	{
		var fileArr = allDatFiles.ToArray();
		Console.WriteLine($"Seeding {fileArr.Length} Objects");

		var progress = new Progress<float>();
		var index = ObjectIndex.LoadOrCreateIndex(ObjDirectory);

		foreach (var objIndex in index.Objects.DistinctBy(x => new { x.ObjectName, x.Checksum }))
		{
			var metadataKey = (objIndex.ObjectName, objIndex.Checksum.ToString());
			if (!metadata.TryGetValue(metadataKey, out var meta))
			{
				Console.WriteLine($"{objIndex} had no metadata");
			}

			var author = meta?.Creator == null ? null : db.Authors.SingleOrDefault(x => x.Name == meta.Creator);
			var tags = meta?.Tags == null ? null : db.Tags.Where(x => meta.Tags.Contains(x.Name));

			var tblLocoObject = new TblLocoObject()
			{
				Name = Path.GetFileNameWithoutExtension(objIndex.Filename),
				PathOnDisk = Path.Combine(ObjDirectory, objIndex.Filename),
				OriginalName = objIndex.ObjectName,
				OriginalChecksum = objIndex.Checksum,
				SourceGame = objIndex.SourceGame,
				ObjectType = objIndex.ObjectType,
				VehicleType = objIndex.VehicleType,
				Description = meta?.DescriptionAndFile,
				Author = author,
				CreationDate = null,
				LastEditDate = null,
				Tags = tags == null ? [] : [.. tags],
				Availability = ObjectAvailability.NewGames,
				Licence = null,
			};

			// there's a unique constraint on the composite key index (OriginalName, OriginalChecksum), so check existence first so no exceptions
			// this isn't necessary since we're already filtering in LINQ, but if we were adding to a non-empty database, this would be necessary
			//var existingEntityInDb = db.Objects
			//	.FirstOrDefault(e => e.OriginalName == tblLocoObject.OriginalName && e.OriginalChecksum == tblLocoObject.OriginalChecksum);

			//var existingEntityInChangeTracker = db.ChangeTracker.Entries()
			//	.Where(e => e.State == EntityState.Added && e.Entity.GetType() == typeof(TblLocoObject))
			//	.Select(e => e.Entity as TblLocoObject)
			//	.FirstOrDefault(e => e!.OriginalName == tblLocoObject.OriginalName && e.OriginalChecksum == tblLocoObject.OriginalChecksum);

			_ = db.Add(tblLocoObject);

		}
		_ = db.SaveChanges();
	}

	Console.WriteLine("Finished seeding");
}

static LocoDb ExampleRun()
{
	var builder = new DbContextOptionsBuilder<LocoDb>();
	_ = builder.UseSqlite(LocoDb.GetDbPath());
	var db = new LocoDb(builder.Options);

	// Note: The database must exist before this script works
	Console.WriteLine($"Database path: {LocoDb.GetDbPath()}");

	var seed = true;
	if (seed)
	{
		SeedDb(db, false);
	}

	return db;
}

static Dictionary<(string ObjectName, string Checksum), GlenDbData2> LoadMetadata(string metadataFile)
{
	if (!File.Exists(metadataFile))
	{
		return [];
	}

	var text = File.ReadAllText(metadataFile);
	var metadata = JsonSerializer.Deserialize<GlenDBSchema2>(text).data;
	var kvList = metadata.GroupBy(x => (x.ObjectName, uint32_t_LittleToBigEndian(x.Checksum)));
	return kvList.ToDictionary(x => x.Key, x => x.First());
}

static string? uint32_t_LittleToBigEndian(string input)
{
	var r = new string(input.Chunk(2).Reverse().SelectMany(x => x).ToArray());
	return Convert.ToUInt32(r, 16).ToString();
}


// Update
//Console.WriteLine("Updating the blog and adding a post");
//blog.Url = "https://devblogs.microsoft.com/dotnet";
//blog.Posts.Add(new Post { Title = "Hello World", Content = "I wrote an app using EF Core!" });
//db.SaveChanges();

// Delete
//Console.WriteLine("Delete the blog");
//db.Remove(blog);
//db.SaveChanges();
