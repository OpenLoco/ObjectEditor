using Dat;
using Microsoft.EntityFrameworkCore;
using OpenLoco.Common;
using OpenLoco.Dat.FileParsing;
using OpenLoco.Definitions;
using OpenLoco.Definitions.Database;
using System.Text.Json;
using System.Text.Json.Serialization;

using var db = ExampleRun();

Console.WriteLine("done");
Console.ReadLine();

static LocoDb ExampleRun()
{
	var builder = new DbContextOptionsBuilder<LocoDb>();
	_ = builder.UseSqlite(LocoDb.GetDbPath());
	var db = new LocoDb(builder.Options);

	// Note: The database must exist before this script works
	Console.WriteLine($"Database path: {LocoDb.GetDbPath()}");

	const bool seed = true;
	const bool DeleteExisting = false;

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
		_ = db.Authors.ExecuteDelete();
		_ = db.Tags.ExecuteDelete();
		_ = db.Modpacks.ExecuteDelete();
		_ = db.Licences.ExecuteDelete();
		_ = db.SaveChanges(); // not necessary since ExecuteDelete auto-saves
	}

	const string ObjDirectory = "Q:\\Games\\Locomotion\\Server";
	var allDatFiles = SawyerStreamUtils.GetDatFilesInDirectory(ObjDirectory);

	Console.WriteLine("Seeding");

	var jsonOptions = new JsonSerializerOptions() { WriteIndented = true, Converters = { new JsonStringEnumConverter() }, };

	// ...

	if (!db.Authors.Any())
	{
		Console.WriteLine("Seeding Authors");

		// load from metadata
		//var authors = metadata.Values
		//	.Select(x => x.Creator)
		//	.Distinct()
		//	.Where(x => !string.IsNullOrEmpty(x))
		//	.Select(x => new TblAuthor() { Name = x })
		//	.OrderBy(x => x.Name);

		//File.WriteAllText("Q:\\Games\\Locomotion\\Server\\authors.json", JsonSerializer.Serialize(authors.Select(x => x.Name), jsonOptions)); // write separate file

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

		// load from metadata
		//var tags = metadata.Values
		//	.Select(x => x.Tags)
		//	.SelectMany(x => x)
		//	.Distinct()
		//	.Where(x => !string.IsNullOrEmpty(x))
		//	.Select(x => new TblTag() { Name = x })
		//	.OrderBy(x => x.Name);

		//File.WriteAllText("Q:\\Games\\Locomotion\\Server\\tags.json", JsonSerializer.Serialize(tags.Select(x => x.Name), jsonOptions)); // write separate file

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

		// load from metadata
		//var licences = new List<TblLicence>
		//{
		//	new() { Name = "CC BY_NC_SA", Text = "This license enables users to distribute, remix, adapt, and build upon the material in any medium or format for noncommercial purposes only, and only so long as attribution is given to the creator. If you remix, adapt, or build upon the material, you must license the modified material under identical terms. CC BY-NC-SA includes the following elements:\r\n\r\n BY: credit must be given to the creator.\r\n NC: Only noncommercial uses of the work are permitted.\r\n SA: Adaptations must be shared under the same terms." }
		//}
		//.Where(x => !string.IsNullOrEmpty(x.Name))
		//.OrderBy(x => x.Name);

		//var toWrite = licences.Select(x => new LicenceJsonRecord(x.Name, x.Text)).ToList();
		//File.WriteAllText("Q:\\Games\\Locomotion\\Server\\licences.json", JsonSerializer.Serialize(toWrite, jsonOptions)); // write separate file

		var licences = JsonSerializer.Deserialize<IEnumerable<LicenceJsonRecord>>(File.ReadAllText("Q:\\Games\\Locomotion\\Server\\licences.json"), jsonOptions);
		if (licences != null)
		{
			db.AddRange(licences.Select(x => new TblLicence() { Name = x.Name, Text = x.Text }));
			_ = db.SaveChanges();
		}
	}

	// ...

	if (!db.Modpacks.Any())
	{
		Console.WriteLine("Seeding Modpacks");

		// load from metadata
		//var modpacks = new List<TblModpack>
		//{
		//	new() { Name = "pack1" },
		//}
		//.Where(x => !string.IsNullOrEmpty(x.Name))
		//.OrderBy(x => x.Name);

		//File.WriteAllText("Q:\\Games\\Locomotion\\Server\\modpacks.json", JsonSerializer.Serialize(modpacks.Select(x => x.Name), jsonOptions)); // write separate file

		var modpacks = JsonSerializer.Deserialize<IEnumerable<string>>(File.ReadAllText("Q:\\Games\\Locomotion\\Server\\modpacks.json"), jsonOptions);
		if (modpacks != null)
		{
			db.AddRange(modpacks.Select(x => new TblModpack() { Name = x }));
			_ = db.SaveChanges();
		}
	}

	// ...

	//if (!db.Objects.Any())
	{
		var fileArr = allDatFiles.ToArray();
		Console.WriteLine($"Seeding {fileArr.Length} Objects");

		var progress = new Progress<float>();
		var index = ObjectIndex.LoadOrCreateIndex(ObjDirectory);

		var objectMetadata = JsonSerializer.Deserialize<IEnumerable<ObjectMetadata>>(File.ReadAllText("Q:\\Games\\Locomotion\\Server\\objectMetadata.json"), jsonOptions);
		var objectMetadataDict = objectMetadata!.ToDictionary(x => (x.ObjectName, x.Checksum), x => x);

		#region RewriteMetadata

		const string MetadataFile = "Q:\\Games\\Locomotion\\Server\\database_new.json";
		var metadata = LoadMetadata(MetadataFile);

		var authors = JsonSerializer.Deserialize<IEnumerable<string>>(File.ReadAllText("Q:\\Games\\Locomotion\\Server\\authors.json"), jsonOptions);
		var modpacks = JsonSerializer.Deserialize<IEnumerable<string>>(File.ReadAllText("Q:\\Games\\Locomotion\\Server\\modpacks.json"), jsonOptions);
		var tagsj = JsonSerializer.Deserialize<IEnumerable<string>>(File.ReadAllText("Q:\\Games\\Locomotion\\Server\\tags.json"), jsonOptions);

		var allMeta = new List<ObjectMetadata>();

		foreach (var objIndex in index.Objects.DistinctBy(x => new { x.ObjectName, x.Checksum }))
		{
			var metadataKey = (objIndex.ObjectName, objIndex.Checksum.ToString());
			if (!metadata.TryGetValue(metadataKey, out var meta))
			{
				continue;
			}

			// some authors are now metadata
			List<string> authorss = modpacks.Contains(meta.Creator) ? [] : [meta.Creator];

			var tagss = meta.Tags.ToList();

			List<string> modpackss = modpacks.Contains(meta.Creator) ? [meta.Creator] : [];

			string? licence = null;
			if (meta.Creator is "Zeak" or "Walter1940")
			{
				licence = "CC BY_NC_SA";
			}

			if (modpackss.Contains("Manuel18 - Tuna Mod"))
			{
				authorss.Add("Manuel18");
			}
			if (modpackss.Contains("Zimms Depots"))
			{
				authorss.Add("Zimm");
			}
			if (modpackss.Contains("Nicholas Soares Airplane Pack"))
			{
				authorss.Add("Nicholas Soares");
			}

			var metaD = new ObjectMetadata(objIndex.ObjectName, objIndex.Checksum, meta.DescriptionAndFile, authorss, tagss, modpackss, licence);

			//			if objIndex.
			//{
			//				var author = meta?.Creator == null ? null : db.Authors.SingleOrDefault(x => x.Name == meta.Creator);
			//			}

			//			var tags = meta?.Tags == null ? null : db.Tags.Where(x => meta.Tags.Contains(x.Name));
			allMeta.Add(metaD);
		}
		//foreach (var c in metadata)
		//{
		//	var meta = c.Value;
		//	if (modpacks.Contains(meta.Creator))
		//	{
		//		meta.Creator = "";
		//		meta.Mod
		//	}
		//}

		File.WriteAllText("Q:\\Games\\Locomotion\\Server\\objectMetadata.json", JsonSerializer.Serialize(allMeta.OrderBy(x => x.ObjectName), jsonOptions)); // write separate file

		#endregion

		foreach (var objIndex in index.Objects.DistinctBy(x => new { x.ObjectName, x.Checksum }))
		{
			var metadataKey = (objIndex.ObjectName, objIndex.Checksum);
			if (!objectMetadataDict.TryGetValue(metadataKey, out var meta))
			{ }

			var author = meta?.Authors == null ? null : db.Authors.SingleOrDefault(x => x.Name == meta.Authors.FirstOrDefault());
			var tags = meta?.Tags == null ? null : db.Tags.Where(x => meta.Tags.Contains(x.Name)).ToList();
			var licence = meta?.Licence == null ? null : db.Licences.Where(x => x.Name == meta.Licence).First();

			var tblLocoObject = new TblLocoObject()
			{
				Name = $"{objIndex.ObjectName}_{objIndex.Checksum}",
				PathOnDisk = Path.Combine(ObjDirectory, objIndex.Filename),
				OriginalName = objIndex.ObjectName,
				OriginalChecksum = objIndex.Checksum,
				IsVanilla = objIndex.IsVanilla,
				ObjectType = objIndex.ObjectType,
				VehicleType = objIndex.VehicleType,
				Description = meta?.Description,
				Author = author,
				CreationDate = null,
				LastEditDate = null,
				Tags = tags ?? [],
				Availability = ObjectAvailability.NewGames,
				Licence = licence,
			};

			//_ = db.Add(tblLocoObject);

		}
		//_ = db.SaveChanges();
	}

	//Console.WriteLine("Seeding existing dat-licence data");
	//var ccbyncsa = db.Licences.Single(x => x.Name == "CC BY_NC_SA");
	//foreach (var dat in db.Objects.Where(x => x.Author != null && (x.Author.Name == "Zeak" || x.Author.Name == "Walter1940")))
	//{
	//	dat.Licence = ccbyncsa;
	//}
	//_ = db.SaveChanges();

	Console.WriteLine("Finished seeding");
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

record LicenceJsonRecord(string Name, string Text);

record ObjectMetadata(string ObjectName, uint Checksum, string Description, List<string> Authors, List<string> Tags, List<string> Modpacks, string? Licence);
