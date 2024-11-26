using Microsoft.EntityFrameworkCore;
using OpenLoco.Common.Logging;
using OpenLoco.Dat;
using OpenLoco.Dat.Data;
using OpenLoco.Definitions;
using OpenLoco.Definitions.Database;
using OpenLoco.Definitions.SourceData;
using System.Text.Json;
using System.Text.Json.Serialization;

using var db = Seed();

Console.WriteLine("done");
Console.ReadLine();

static LocoDb Seed()
{
	var builder = new DbContextOptionsBuilder<LocoDb>();
	const string connectionString = "Data Source=Q:\\Games\\Locomotion\\Server\\loco-dev.db";
	_ = builder.UseSqlite(connectionString);
	var db = new LocoDb(builder.Options);

	// Note: The database must exist before this script works
	Console.WriteLine($"Database path: {connectionString}");

	const bool seed = true;
	const bool DeleteExisting = true;

	if (seed)
	{
		SeedDb(db, DeleteExisting);
	}

	return db;
}

static void SeedDb(LocoDb db, bool deleteExisting)
{
	if (deleteExisting)
	{
		Console.WriteLine("Clearing database");
		_ = db.Objects.ExecuteDelete();
		_ = db.Tags.ExecuteDelete();
		_ = db.ObjectPacks.ExecuteDelete();
		_ = db.Authors.ExecuteDelete();
		_ = db.Licences.ExecuteDelete();
		_ = db.SaveChanges(); // not necessary since ExecuteDelete auto-saves
	}

	const string ObjDirectory = "Q:\\Games\\Locomotion\\Server\\Objects";

	Console.WriteLine("Seeding");
	var logger = new Logger();

	var jsonOptions = new JsonSerializerOptions() { WriteIndented = true, Converters = { new JsonStringEnumConverter() }, };

	// ...

	if (!db.Authors.Any())
	{
		Console.WriteLine("Seeding Authors");

		var authors = JsonSerializer.Deserialize<IEnumerable<string>>(File.ReadAllText("Q:\\Games\\Locomotion\\Server\\authors.json"), jsonOptions);
		if (authors != null)
		{
			db.AddRange(authors.Select(x => new TblAuthor() { Name = x }));
			_ = db.SaveChanges();
		}
	}

	// ...

	if (!db.Tags.Any())
	{
		Console.WriteLine("Seeding Tags");

		var tags = JsonSerializer.Deserialize<IEnumerable<string>>(File.ReadAllText("Q:\\Games\\Locomotion\\Server\\tags.json"), jsonOptions);
		if (tags != null)
		{
			db.AddRange(tags.Select(x => new TblTag() { Name = x }));
			_ = db.SaveChanges();
		}
	}

	// ...

	if (!db.Licences.Any())
	{
		Console.WriteLine("Seeding Licences");

		var licences = JsonSerializer.Deserialize<IEnumerable<LicenceJsonRecord>>(File.ReadAllText("Q:\\Games\\Locomotion\\Server\\licences.json"), jsonOptions);
		if (licences != null)
		{
			db.AddRange(licences.Select(x => new TblLicence() { Name = x.Name, Text = x.Text }));
			_ = db.SaveChanges();
		}
	}

	// ...

	if (!db.ObjectPacks.Any())
	{
		Console.WriteLine("Seeding ObjectPacks");

		var modpacks = JsonSerializer.Deserialize<IEnumerable<ObjectPackJsonRecord>>(File.ReadAllText("Q:\\Games\\Locomotion\\Server\\objectPacks.json"), jsonOptions);
		if (modpacks != null)
		{
			db.AddRange(modpacks.Select(x => new TblLocoObjectPack() { Name = x.Name, Author = x.Author == null ? null : db.Authors.Single(a => a.Name == x.Author) }));
			_ = db.SaveChanges();
		}
	}

	// ...

	if (!db.Objects.Any())
	{
		Console.WriteLine("Seeding Objects");

		var progress = new Progress<float>();
		var index = ObjectIndex.LoadOrCreateIndex(ObjDirectory, logger);
		var objectMetadata = JsonSerializer.Deserialize<IEnumerable<ObjectMetadata>>(File.ReadAllText("Q:\\Games\\Locomotion\\Server\\Objects\\objectMetadata.json"), jsonOptions);
		var objectMetadataDict = objectMetadata!.ToDictionary(x => (x.DatName, x.DatChecksum), x => x);
		var gameReleaseDate = new DateTimeOffset(2004, 09, 07, 0, 0, 0, TimeSpan.Zero);

		foreach (var objIndex in index!.Objects.DistinctBy(x => (x.DatName, x.DatChecksum)))
		{
			var metadataKey = (objIndex.DatName, objIndex.DatChecksum);
			if (!objectMetadataDict.TryGetValue(metadataKey, out var meta))
			{
				var newMetadata = new ObjectMetadata(Guid.NewGuid().ToString(), objIndex.DatName, objIndex.DatChecksum, null, [], [], [], null, ObjectAvailability.AllGames, ObjectSource.Custom);
				meta = newMetadata;
				objectMetadataDict.Add((objIndex.DatName, objIndex.DatChecksum), newMetadata);
			}

			var filename = Path.Combine(ObjDirectory, objIndex.Filename);
			var creationTime = objIndex.ObjectSource is ObjectSource.LocomotionSteam or ObjectSource.LocomotionGoG
				? gameReleaseDate
				: File.GetLastWriteTimeUtc(filename); // this is the "Modified" time as shown in Windows

			var authors = meta.Authors == null ? null : db.Authors.Where(x => meta.Authors.Contains(x.Name)).ToList();
			var tags = meta.Tags == null ? null : db.Tags.Where(x => meta.Tags.Contains(x.Name)).ToList();
			var objectPacks = meta.ObjectPacks == null ? null : db.ObjectPacks.Where(x => meta.ObjectPacks.Contains(x.Name)).ToList();
			var licence = meta.Licence == null ? null : db.Licences.Where(x => x.Name == meta.Licence).First();

			var tblLocoObject = new TblLocoObject()
			{
				UniqueName = meta!.UniqueName,
				DatName = objIndex.DatName,
				DatChecksum = objIndex.DatChecksum,
				ObjectSource = objIndex.ObjectSource,
				ObjectType = objIndex.ObjectType,
				VehicleType = objIndex.VehicleType,
				Description = meta?.Description,
				Authors = authors ?? [],
				CreationDate = creationTime,
				LastEditDate = null,
				UploadDate = DateTimeOffset.Now,
				Tags = tags ?? [],
				ObjectPacks = objectPacks ?? [],
				Availability = meta!.ObjectAvailability,
				Licence = licence,
			};

			_ = db.Add(tblLocoObject);
		}

		_ = db.SaveChanges();
	}

	Console.WriteLine("Finished seeding");
}
