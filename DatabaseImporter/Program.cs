using Common;
using Common.Json;
using Microsoft.EntityFrameworkCore;
using Common.Logging;
using Definitions;
using Definitions.Database;
using Definitions.SourceData;
using System.Text.Json;
using System.Text.Json.Serialization;
using Index;
using Definitions.ObjectModels.Types;
using var db = Seed();

Console.WriteLine("done");
Console.ReadLine();

static LocoDbContext Seed()
{
	var db = LocoDbContext.GetDbFromFile(LocoDbContext.DefaultDb);

	// Note: The database must exist before this script works
	Console.WriteLine($"Database path: {LocoDbContext.DefaultDb}");

	const bool seed = true;
	const bool DeleteExisting = true;

	if (seed)
	{
		SeedDb(db, DeleteExisting);
	}

	return db;
}

static void SeedDb(LocoDbContext db, bool deleteExisting)
{
	if (deleteExisting)
	{
		Console.WriteLine("Clearing database");
		_ = db.ObjectPacks.ExecuteDelete();
		_ = db.SC5FilePacks.ExecuteDelete();
		_ = db.SC5Files.ExecuteDelete();
		_ = db.Tags.ExecuteDelete();
		_ = db.Authors.ExecuteDelete();
		_ = db.Objects.ExecuteDelete();
		_ = db.Licences.ExecuteDelete();
		_ = db.SaveChanges(); // not necessary since ExecuteDelete auto-saves
	}

	const string objDirectory = "Q:\\Games\\Locomotion\\Server\\Objects";

	const string databaseDirectory = "Q:\\Games\\Locomotion\\Database";
	var authorsJson = Path.Combine(databaseDirectory, "authors.json");
	var tagsJson = Path.Combine(databaseDirectory, "tags.json");
	var licencesJson = Path.Combine(databaseDirectory, "licences.json");
	var sc5FilePacksJson = Path.Combine(databaseDirectory, "sc5FilePacks.json");
	var sc5FilesJson = Path.Combine(databaseDirectory, "sc5Files.json");
	var objectPacksJson = Path.Combine(databaseDirectory, "objectPacks.json");
	var objectMetadataJson = Path.Combine(databaseDirectory, "objectMetadata.json");

	Console.WriteLine("Seeding");
	var logger = new Logger();

	var jsonOptions = JsonFile.DefaultSerializerOptions;
	jsonOptions.Converters.Add(new JsonStringEnumConverter());

	// ...

	if (!db.Authors.Any())
	{
		Console.WriteLine("Seeding Authors");

		var authors = JsonSerializer.Deserialize<IEnumerable<string>>(File.ReadAllText(authorsJson), jsonOptions);
		if (authors != null)
		{
			db.AddRange(authors.Select(x => new TblAuthor()
			{
				Name = x
			}));

			_ = db.SaveChanges();
		}
	}

	// ...

	if (!db.Tags.Any())
	{
		Console.WriteLine("Seeding Tags");

		var tags = JsonSerializer.Deserialize<IEnumerable<string>>(File.ReadAllText(tagsJson), jsonOptions);
		if (tags != null)
		{
			db.AddRange(tags.Select(x => new TblTag()
			{
				Name = x
			}));

			_ = db.SaveChanges();
		}
	}

	// ...

	if (!db.Licences.Any())
	{
		Console.WriteLine("Seeding Licences");

		var licences = JsonSerializer.Deserialize<IEnumerable<LicenceJsonRecord>>(File.ReadAllText(licencesJson), jsonOptions);
		if (licences != null)
		{
			db.AddRange(licences.Select(x => new TblLicence()
			{
				Name = x.Name,
				Text = x.Text
			}));

			_ = db.SaveChanges();
		}
	}

	// ...

	if (!db.SC5FilePacks.Any())
	{
		Console.WriteLine("Seeding SC5FilePacks");

		var sc5FilePacks = JsonSerializer.Deserialize<IEnumerable<SC5FilePackJsonRecord>>(File.ReadAllText(sc5FilePacksJson), jsonOptions);
		if (sc5FilePacks != null)
		{
			db.AddRange(sc5FilePacks.Select(x => new TblSC5FilePack()
			{
				Name = x.Name,
				Description = x.Description,
				Authors = [.. db.Authors.Where(a => x.Authors.Contains(a.Name))],
				Tags = [.. db.Tags.Where(a => x.Tags.Contains(a.Name))],
				Licence = x.Licence == null ? null : db.Licences.Single(l => l.Name == x.Licence),
				CreatedDate = x.CreatedDate,
				ModifiedDate = x.ModifiedDate,
				UploadedDate = x.UploadedDate,
			}));

			_ = db.SaveChanges();
		}
	}

	// ...

	if (!db.SC5Files.Any())
	{
		Console.WriteLine("Seeding SC5Files");

		var sc5Files = JsonSerializer.Deserialize<IEnumerable<SC5FileJsonRecord>>(File.ReadAllText(sc5FilesJson), jsonOptions);
		if (sc5Files != null)
		{
			var sC5FilePacks = db.SC5FilePacks?.Where(x => x.SC5Files.Select(fp => fp.Name).Contains(x.Name)).ToList();
			db.AddRange(sc5Files.Select(x => new TblSC5File()
			{
				Name = x.Name,
				Description = x.Description,
				Authors = [.. db.Authors.Where(a => x.Authors.Contains(a.Name))],
				SC5FilePacks = sC5FilePacks ?? []
			}));

			_ = db.SaveChanges();
		}
	}

	// ...

	if (!db.ObjectPacks.Any())
	{
		Console.WriteLine("Seeding ObjectPacks");

		var objectPacks = JsonSerializer.Deserialize<IEnumerable<ObjectPackJsonRecord>>(File.ReadAllText(objectPacksJson), jsonOptions);
		if (objectPacks != null)
		{
			db.AddRange(objectPacks.Select(x => new TblObjectPack()
			{
				Name = x.Name,
				Description = x.Description,
				Authors = [.. db.Authors.Where(a => x.Authors.Contains(a.Name))],
				Tags = [.. db.Tags.Where(a => x.Tags.Contains(a.Name))],
				Licence = x.Licence == null ? null : db.Licences.Single(l => l.Name == x.Licence),
				CreatedDate = null,
				ModifiedDate = null,
				UploadedDate = DateOnly.Today,
			}));
			_ = db.SaveChanges();
		}
	}

	// ...

	if (!db.Objects.Any())
	{
		Console.WriteLine("Seeding Objects");

		var progress = new Progress<float>();
		var index = ObjectIndex.LoadOrCreateIndex(objDirectory, logger);
		var objectMetadata = JsonSerializer.Deserialize<IEnumerable<ObjectMetadata>>(File.ReadAllText(objectMetadataJson), jsonOptions);
		var objectMetadataDict = objectMetadata!.ToDictionary(x => x.InternalName, x => x);
		var gameReleaseDate = new DateOnly(2004, 09, 07);

		foreach (var objIndex in index!.Objects.DistinctBy(x => (x.DisplayName, x.DatChecksum)))
		{
			var metadataKey = objIndex.DisplayName; // should be InternalName
			if (!objectMetadataDict.TryGetValue(metadataKey, out var meta))
			{
				var newMetadata = new ObjectMetadata(Guid.NewGuid().ToString(), null, [], [], [], null, ObjectAvailability.Available, DateOnly.Today, null, DateOnly.Today, ObjectSource.Custom);
				meta = newMetadata;
				objectMetadataDict.Add(objIndex.DisplayName, newMetadata);
			}

			var filename = Path.Combine(objDirectory, objIndex.FileName);
			var creationTime = objIndex.ObjectSource is ObjectSource.LocomotionSteam or ObjectSource.LocomotionGoG
				? gameReleaseDate
				: DateOnly.FromDateTime(File.GetLastWriteTimeUtc(filename)); // this is the "Modified" time as shown in Windows

			var authors = meta.Authors == null ? null : db.Authors.Where(x => meta.Authors.Contains(x.Name)).ToList();
			var tags = meta.Tags == null ? null : db.Tags.Where(x => meta.Tags.Contains(x.Name)).ToList();
			var objectPacks = meta.ObjectPacks == null ? null : db.ObjectPacks.Where(x => meta.ObjectPacks.Contains(x.Name)).ToList();
			var licence = meta.Licence == null ? null : db.Licences.Where(x => x.Name == meta.Licence).First();

			var tblLocoObject = new TblObject()
			{
				Name = meta!.InternalName,
				ObjectSource = objIndex.ObjectSource,
				ObjectType = objIndex.ObjectType,
				VehicleType = objIndex.VehicleType,
				Description = meta?.Description,
				Authors = authors ?? [],
				CreatedDate = creationTime,
				ModifiedDate = null,
				UploadedDate = DateOnly.Today,
				Tags = tags ?? [],
				ObjectPacks = objectPacks ?? [],
				DatObjects = [],
				Licence = licence,
			};

			var addedObj = db.Add(tblLocoObject);

			var locoLookupTbl = new TblDatObject()
			{
				DatName = objIndex.DisplayName,
				DatChecksum = objIndex.DatChecksum.Value,
				xxHash3 = objIndex.xxHash3.Value,
				ObjectId = addedObj.Entity.Id,
				Object = tblLocoObject,
			};
			_ = db.DatObjects.Add(locoLookupTbl);

			tblLocoObject.DatObjects.Add(locoLookupTbl);
		}

		_ = db.SaveChanges();
	}

	Console.WriteLine("Finished seeding");
}
