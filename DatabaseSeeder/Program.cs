using Microsoft.EntityFrameworkCore;
using OpenLoco.Dat.FileParsing;
using OpenLoco.Dat.Objects;
using OpenLoco.Db.Schema;
using OpenLoco.Shared;
using System.Text.Json;

var filename = "Q:\\Games\\Locomotion\\LocoVault\\dataBase.json";
var text = File.ReadAllText(filename);
var metadata = JsonSerializer.Deserialize<GlenDBSchema>(text).data;
var distinct = metadata.DistinctBy(x => x.ObjectName);
var dupes = metadata.Except(distinct);

foreach (var x in dupes) //.OrderBy(x => x.ObjectName))
{
	Console.WriteLine($"{x.ObjectName} - {x.DescriptionAndFile}");
}

//using var db = ExampleRun();

Console.WriteLine("done");
Console.ReadLine();

static void SeedDb(LocoDb db)
{
	Console.WriteLine("Clearing database");
	// clear
	db.Authors.ExecuteDelete();
	db.Tags.ExecuteDelete();
	db.Objects.ExecuteDelete();
	db.SaveChanges();

	// Load data

	var objDirectory = "Q:\\Games\\Locomotion\\OriginalObjects";
	var datFiles = SawyerStreamUtils.GetDatFilesInDirectory(objDirectory);

	Console.WriteLine("Loading metadata data file");
	var metadata = Utils.LoadMetadata("Q:\\Games\\Locomotion\\LocoVault\\dataBase.json");

	// Seed

	if (!db.Authors.Any())
	{
		Console.WriteLine("Seeding Authors");
		var authors = metadata.Values.Select(x => x.Author).Distinct();
		foreach (var author in authors)
		{
			db.Add(new TblAuthor() { Name = author });
		}
		db.SaveChanges();
	}

	// ...

	if (!db.Tags.Any())
	{
		Console.WriteLine("Seeding Tags");
		var tags = metadata.Values.Select(x => x.Tags).SelectMany(x => x).Distinct();
		foreach (var tag in tags.Where(x => !string.IsNullOrEmpty(x)))
		{
			db.Add(new TblTag() { Name = tag });
		}
		db.SaveChanges();
	}

	// ...

	if (!db.Objects.Any())
	{
		Console.WriteLine("Seeding Objects");
		foreach (var datFile in datFiles)
		{
			var bytes = File.ReadAllBytes(datFile);
			var (fileInfo, locoObj) = SawyerStreamReader.LoadFullObjectFromStream(bytes, filename: datFile, loadExtra: false); // currently don't need to load extra

			var tblLocoObject = new TblLocoObject()
			{
				Name = Utils.GetDatCompositeKey(fileInfo.S5Header.Name, fileInfo.S5Header.Checksum),
				OriginalName = fileInfo.S5Header.Name,
				OriginalChecksum = fileInfo.S5Header.Checksum,
				OriginalBytes = bytes,
				SourceGame = fileInfo.S5Header.SourceGame,
				ObjectType = fileInfo.S5Header.ObjectType,
				VehicleType = (locoObj?.Object is VehicleObject veh) ? veh.Type : null,
				Description = "<unk>", // todo: load from Glen's DB
				Author = null, // todo: load from Glen's DB
				CreationDate = null,
				LastEditDate = null,
			};

			db.Add(tblLocoObject);
		}
		db.SaveChanges();
	}

	Console.WriteLine("Finished seeding");
}

static LocoDb ExampleRun()
{
	var builder = new DbContextOptionsBuilder<LocoDb>();
	builder.UseSqlite(LocoDb.GetDbPath());
	var db = new LocoDb(builder.Options);

	// Note: The database must exist before this script works
	Console.WriteLine($"Database path: {LocoDb.GetDbPath()}");

	bool seed = true;
	if (seed)
	{
		SeedDb(db);
	}

	// Read
	Console.WriteLine("Querying for an Author");
	var _author = db.Authors
		.OrderBy(b => b.Name)
		.First();
	Console.WriteLine(_author.Name);

	Console.WriteLine("Querying for a Tag");
	var _tag = db.Tags
		.OrderBy(b => b.Name)
		.First();
	Console.WriteLine(_tag.Name);

	Console.WriteLine("Querying for an Object");
	var obj = db.Objects
		.OrderBy(b => b.Name)
		.First();
	Console.WriteLine(obj.OriginalName);
	Console.WriteLine(obj.Description);
	Console.WriteLine(obj.ObjectType);
	Console.WriteLine(obj.Author?.Name);
	//Console.WriteLine(obj.Tags.Count);
	//foreach (var t in obj.Tags)
	//{
	//	Console.WriteLine(t.Tag.Name);
	//}

	// clear
	//db.Authors.ExecuteDelete();
	//db.Tags.ExecuteDelete();
	//db.Objects.ExecuteDelete();
	//db.SaveChanges();
	return db;
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
