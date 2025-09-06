using Microsoft.EntityFrameworkCore;
using Common.Logging;
using Dat.FileParsing;
using Definitions.Database;
using System.IO.Hashing;
using System.Reflection;
using Index;
using Definitions.ObjectModels.Types;
using Definitions.ObjectModels.Objects.Industry;
using IndustryObject = Definitions.ObjectModels.Objects.Industry.IndustryObject;
using Definitions.ObjectModels.Objects.Vehicle;
using VehicleObject = Definitions.ObjectModels.Objects.Vehicle.VehicleObject;
using Dat.Data;
using Definitions.ObjectModels.Objects.Cargo;
using Definitions.ObjectModels.Objects.TrackStation;
using Definitions.ObjectModels.Objects.Track;

static void QueryTrackStationOneSidedTrack()
{
	var dir = "Q:\\Games\\Locomotion\\Server\\Objects";
	var logger = new Logger();
	var index = ObjectIndex.LoadOrCreateIndex(dir, logger);
	//var index = ObjectIndex.CreateIndex(dir, logger);
	//index.SaveIndexAsync(Path.Combine(dir, "objectIndex.json")).Wait();

	var results = new List<(ObjectIndexEntry Obj, ObjectSource ObjectSource, List<string> Flags)>();

	foreach (var obj in index.Objects.Where(x => x.ObjectType == ObjectType.TrackStation))
	{
		try
		{
			var o = SawyerStreamReader.LoadFullObject(Path.Combine(dir, obj.FileName), logger);
			if (o.LocoObject != null)
			{
				var struc = (TrackStationObject)o.LocoObject.Object;
				var header = o.DatFileInfo.S5Header;
				var source = OriginalObjectFiles.GetFileSource(header.Name, header.Checksum);

				// Build a list of all enabled TrackTraitFlags for this object
				var enabledFlags = new List<string>();
				foreach (TrackTraitFlags flag in Enum.GetValues(typeof(TrackTraitFlags)))
				{
					// skip the default/zero value if present
					if (flag.Equals((TrackTraitFlags)0))
					{
						continue;
					}

					if (struc.TrackPieces.HasFlag(flag))
					{
						enabledFlags.Add(flag.ToString());
					}
				}

				if (enabledFlags.Count > 0)
				{
					results.Add((obj, source, enabledFlags));
				}
				else
				{
					Console.WriteLine($"{header.Name} didn't have it");
				}
			}
		}
		catch (Exception ex)
		{
			Console.WriteLine($"{obj.FileName} - {ex.Message}");
		}
	}

	Console.WriteLine(results.Count);
	foreach (var result in results)
	{
		// Print object index entry and its enabled flags
		Console.WriteLine($"{result.Obj.DisplayName} - Checksum: 0x{result.Obj.DatChecksum:X} - Source: {result.ObjectSource} - Flags: {string.Join('|', result.Flags)}");
	}
}
QueryTrackStationOneSidedTrack();

static void QueryIndustryHasShadows()
{
	var dir = "Q:\\Games\\Locomotion\\Server\\Objects";
	var logger = new Logger();
	var index = ObjectIndex.LoadOrCreateIndex(dir, logger);

	var results = new List<(ObjectIndexEntry Obj, ObjectSource ObjectSource)>();

	foreach (var obj in index.Objects.Where(x => x.ObjectType == ObjectType.Industry))
	{
		try
		{
			var o = SawyerStreamReader.LoadFullObject(Path.Combine(dir, obj.FileName), logger);
			if (o.LocoObject != null)
			{
				var struc = (IndustryObject)o.LocoObject.Object;
				var header = o.DatFileInfo.S5Header;
				var source = OriginalObjectFiles.GetFileSource(header.Name, header.Checksum);

				if (struc.Flags.HasFlag(IndustryObjectFlags.HasShadows))
				{
					results.Add((obj, source));
				}
			}
		}
		catch (Exception ex)
		{
			Console.WriteLine($"{obj.FileName} - {ex.Message}");
		}
	}

	Console.WriteLine(results.Count);

	const string csvHeader = "DatName, ObjectSource";
	var lines = results
		.OrderBy(x => x.Obj.DisplayName)
		.Select(x => string.Join(',', x.Obj.DisplayName, x.ObjectSource));

	File.WriteAllLines("vehicleBodiesWithUnkSpritesFlag.csv", [csvHeader, .. lines]);
}
//QueryIndustryHasShadows();

static void QueryVehicleBodyUnkSprites()
{
	var dir = "Q:\\Games\\Locomotion\\Server\\Objects";
	var logger = new Logger();
	var index = ObjectIndex.LoadOrCreateIndex(dir, logger);

	var results = new List<(ObjectIndexEntry Obj, ObjectSource ObjectSource)>();

	foreach (var obj in index.Objects.Where(x => x.ObjectType == ObjectType.Vehicle))
	{
		try
		{
			var o = SawyerStreamReader.LoadFullObject(Path.Combine(dir, obj.FileName), logger);
			if (o.LocoObject != null)
			{
				var struc = (VehicleObject)o.LocoObject.Object;
				var header = o.DatFileInfo.S5Header;
				var source = OriginalObjectFiles.GetFileSource(header.Name, header.Checksum);

				if (struc.Flags.HasFlag(VehicleObjectFlags.AlternatingCarSprite))
				{
					results.Add((obj, source));
				}
			}
		}
		catch (Exception ex)
		{
			Console.WriteLine($"{obj.FileName} - {ex.Message}");
		}
	}

	Console.WriteLine(results.Count);

	const string csvHeader = "DatName, ObjectSource";
	var lines = results
		.OrderBy(x => x.Obj.DisplayName)
		.Select(x => string.Join(',', x.Obj.DisplayName, x.ObjectSource));

	File.WriteAllLines("vehicleBodiesWithUnkSpritesFlag.csv", [csvHeader, .. lines]);
}
//QueryVehicleBodyUnkSprites();

static void QueryCargoCategories()
{
	var dir = "Q:\\Games\\Locomotion\\Server\\Objects";
	var logger = new Logger();
	var index = ObjectIndex.LoadOrCreateIndex(dir, logger);

	var results = new List<(ObjectIndexEntry Obj, CargoCategory CargoCategory, string LocalisedName, ObjectSource ObjectSource)>();

	foreach (var obj in index.Objects.Where(x => x.ObjectType == ObjectType.Cargo))
	{
		try
		{
			var o = SawyerStreamReader.LoadFullObject(Path.Combine(dir, obj.FileName), logger);
			if (o.LocoObject != null)
			{
				var struc = (CargoObject)o.LocoObject.Object;

				var header = o.DatFileInfo.S5Header;
				var source = OriginalObjectFiles.GetFileSource(header.Name, header.Checksum);

				results.Add((obj, struc.CargoCategory, o.LocoObject.StringTable.Table["Name"][LanguageId.English_UK], source));
			}
		}
		catch (Exception ex)
		{
			Console.WriteLine($"{obj.FileName} - {ex.Message}");
		}
	}

	Console.WriteLine("writing to file");

	const string csvHeader = "DatName, CargoCategory, LocalisedName, ObjectSource";
	var lines = results
		.OrderBy(x => x.Obj.DisplayName)
		.Select(x => string.Join(',', x.Obj.DisplayName, (int)x.CargoCategory, x.LocalisedName, x.ObjectSource));
	File.WriteAllLines("cargoCategories.csv", [csvHeader, .. lines]);
}
//QueryCargoCategories();

static void QueryCostIndices()
{
	var dir = "Q:\\Games\\Locomotion\\Server\\Objects";
	var logger = new Logger();
	var index = ObjectIndex.LoadOrCreateIndex(dir, logger);

	var results = new List<(ObjectIndexEntry Obj, byte CostIndex, short? RunCostIndex)>();

	foreach (var obj in index.Objects)
	{
		try
		{
			var o = SawyerStreamReader.LoadFullObject(Path.Combine(dir, obj.FileName), logger);
			if (o.LocoObject != null)
			{
				var struc = o.LocoObject.Object;
				var type = struc.GetType();

				var costIndexProperty = type.GetProperty("CostIndex", BindingFlags.IgnoreCase | BindingFlags.Public | BindingFlags.Instance);
				var paymentIndexProperty = type.GetProperty("PaymentIndex", BindingFlags.IgnoreCase | BindingFlags.Public | BindingFlags.Instance);
				var runCostIndexProperty = type.GetProperty("RunCostIndex", BindingFlags.IgnoreCase | BindingFlags.Public | BindingFlags.Instance);

				byte? costIndex = null;
				byte? runCostIndex = null;

				if (costIndexProperty?.PropertyType == typeof(byte) && costIndexProperty.GetValue(struc) is byte costIndexValue)
				{
					costIndex = costIndexValue;
				}
				else if (paymentIndexProperty?.PropertyType == typeof(byte) && paymentIndexProperty.GetValue(struc) is byte paymentIndexValue)
				{
					costIndex = paymentIndexValue;
				}

				if (runCostIndexProperty?.PropertyType == typeof(byte) && runCostIndexProperty.GetValue(struc) is byte runCostIndexValue)
				{
					runCostIndex = runCostIndexValue;
				}

				if (costIndex != null)
				{
					results.Add((obj, costIndex.Value, runCostIndex));
				}
			}
		}
		catch (Exception ex)
		{
			Console.WriteLine($"{obj.FileName} - {ex.Message}");
		}
	}

	Console.WriteLine("writing to file");

	const string header = "DatName, ObjectType, CostIndex, RunCostIndex";
	var lines = results
		.OrderBy(x => x.Obj.DisplayName)
		.Select(x => string.Join(',', x.Obj.DisplayName, x.Obj.ObjectType, x.CostIndex, x.RunCostIndex));
	File.WriteAllLines("costIndex.csv", [header, .. lines]);
}
//QueryCostIndices();

async static void WritexxHash3()
{
	var db = LocoDbContext.GetDbFromFile(LocoDbContext.DefaultDb);
	const string objDirectory = "Q:\\Games\\Locomotion\\Server\\Objects";
	var logger = new Logger();
	var index = ObjectIndex.LoadOrCreateIndex(objDirectory, logger);

	var objects = await db.DatObjects.Include(x => x.Object).ToListAsync();

	var count = 0;
	foreach (var lookup in objects)
	{
		if (index.TryFind((lookup.DatName, lookup.DatChecksum), out var entry))
		{
			var filename = Path.Combine(objDirectory, entry.FileName);
			var bytes = File.ReadAllBytes(filename);
			lookup.xxHash3 = XxHash3.HashToUInt64(bytes);
		}

		if (count++ % 1000 == 0)
		{
			Console.WriteLine(count);
		}
	}

	_ = await db.SaveChangesAsync();

	// Note: The database must exist before this script works
	Console.WriteLine($"Database path: {LocoDbContext.DefaultDb}");
}
//WritexxHash3();

async static void FixObjectDescriptions()
{
	var db = LocoDbContext.GetDbFromFile(LocoDbContext.DefaultDb);
	const string objDirectory = "Q:\\Games\\Locomotion\\Server\\Objects";
	var logger = new Logger();
	var index = ObjectIndex.LoadOrCreateIndex(objDirectory, logger);

	var objects = await db.Objects
		.Include(x => x.DatObjects)
		.ToListAsync();

	var count = 0;
	foreach (var obj in objects)
	{
		if (index.TryFind((obj.DatObjects.First().DatName, obj.DatObjects.First().DatChecksum), out var entry))
		{
			var filename = Path.Combine(objDirectory, entry.FileName);
			var bytes = File.ReadAllBytes(filename);

			try
			{
				var dat = SawyerStreamReader.LoadFullObject(filename, logger);

				if (dat.LocoObject == null)
				{
					Console.WriteLine($"Failed to read {entry.FileName}");
					continue;
				}

				obj.Description = dat.LocoObject.StringTable.Table.First().Value[LanguageId.English_UK];
			}
			catch (Exception ex)
			{
				Console.WriteLine($"Exception, failed to read {entry.FileName}, exception={ex}");
				continue;
			}
		}

		if (++count % 200 == 0)
		{
			Console.WriteLine(count);
		}
	}

	_ = await db.SaveChangesAsync();
}
//FixObjectDescriptions();

async static void WriteStringTable()
{
	var db = LocoDbContext.GetDbFromFile(LocoDbContext.DefaultDb);
	const string objDirectory = "Q:\\Games\\Locomotion\\Server\\Objects";
	var logger = new Logger();
	var index = ObjectIndex.LoadOrCreateIndex(objDirectory, logger);

	var objects = await db.Objects
		.Include(x => x.DatObjects)
		.Include(x => x.StringTable)
		.ToListAsync();

	var count = 0;
	foreach (var obj in objects)
	{
		if (index.TryFind((obj.DatObjects.First().DatName, obj.DatObjects.First().DatChecksum), out var entry))
		{
			var filename = Path.Combine(objDirectory, entry.FileName);
			var bytes = File.ReadAllBytes(filename);

			try
			{
				var dat = SawyerStreamReader.LoadFullObject(filename, logger);

				if (dat.LocoObject == null)
				{
					Console.WriteLine($"Failed to read {entry.FileName}");
					continue;
				}

				foreach (var s in dat.LocoObject.StringTable.Table)
				{
					foreach (var t in s.Value)
					{
						obj.StringTable.Add(new TblStringTableRow()
						{
							Name = s.Key,
							Language = t.Key,
							Text = t.Value,
							ObjectId = obj.Id,
						});
					}
				}
			}
			catch (Exception ex)
			{
				Console.WriteLine($"Exception, failed to read {entry.FileName}, exception={ex}");
				continue;
			}
		}

		if (++count % 200 == 0)
		{
			Console.WriteLine(count);
		}
	}

	_ = await db.SaveChangesAsync();

	// Note: The database must exist before this script works
	Console.WriteLine($"Database path: {LocoDbContext.DefaultDb}");
}
//WriteStringTable();

async static void SetGuids()
{

	var db = LocoDbContext.GetDbFromFile(LocoDbContext.DefaultDb);

	//await db.StringTable.ForEachAsync(x => x.GuidId = Guid.NewGuid());
	//await db.DatObjects.ForEachAsync(x => x.GuidId = Guid.NewGuid());
	//await db.Objects.ForEachAsync(x => x.GuidId = Guid.TryParse(x.Name.ToUpper(), out var guid) ? guid : Guid.NewGuid());
	//await db.ObjectPacks.ForEachAsync(x => x.GuidId = Guid.NewGuid());
	//await db.SC5Files.ForEachAsync(x => x.GuidId = Guid.NewGuid());
	//await db.SC5FilePacks.ForEachAsync(x => x.GuidId = Guid.NewGuid());
	//await db.Authors.ForEachAsync(x => x.GuidId = Guid.NewGuid());
	//await db.Licences.ForEachAsync(x => x.GuidId = Guid.NewGuid());
	//await db.Tags.ForEachAsync(x => x.GuidId = Guid.NewGuid());

	_ = await db.SaveChangesAsync();

	// Note: The database must exist before this script works
	Console.WriteLine($"Database path: {LocoDbContext.DefaultDb}");
}
//SetGuids();

//async static void SyncSubObjectParents()
//{
//	var db = LocoDbContext.GetDbFromFile(LocoDbContext.DefaultDb);
//	foreach (var obj in await db.ObjAirport.ToListAsync())
//	{

//	}
//}

async static void SetupSubObjects()
{
	var dir = "Q:\\Games\\Locomotion\\Server\\Objects";
	var logger = new Logger();
	var index = ObjectIndex.LoadOrCreateIndex(dir, logger);
	var db = LocoDbContext.GetDbFromFile(LocoDbContext.DefaultDb);

	var objects = await db
		.Objects
		.Include(x => x.DatObjects)
		.ToListAsync();

	foreach (var objGroup in objects.GroupBy(x => x.ObjectType))
	{
		Console.WriteLine($"=== {objGroup.Key} ===");

		foreach (var obj in objGroup)
		{
			var firstDatObj = obj.DatObjects.First();
			if (!index.TryFind((firstDatObj.DatName, firstDatObj.DatChecksum), out var entry))
			{
				Console.WriteLine($"Couldn't find in index {firstDatObj.DatName} - {obj.Name}");
				continue;
			}

			if (entry?.FileName is null)
			{
				Console.WriteLine($"Filename was null {firstDatObj.DatName} - {obj.Name}");
				continue;
			}

			try
			{
				var o = SawyerStreamReader.LoadFullObject(Path.Combine(dir, entry.FileName), logger);
				if (o.LocoObject == null)
				{
					Console.WriteLine($"Loco object was null {firstDatObj.DatName} - {obj.Name}");
					continue;
				}

				var result = await DbSubObjectHelper.AddOrUpdate(db, obj, o.LocoObject.Object);
				Console.WriteLine($"{result} data for {firstDatObj.DatName} - {obj.Name}");
			}
			catch (Exception ex)
			{
				Console.WriteLine(ex);
			}
		}

		//break; // early exit for testing
	}

	_ = await db.SaveChangesAsync();

	Console.WriteLine("Done");
}
//SetupSubObjects();

async static void QuerySubObjects()
{
	var db = LocoDbContext.GetDbFromFile(LocoDbContext.DefaultDb);
	var logger = new Logger();

	var results = await db.ObjClimate
		.Include(x => x.Parent)
		.Include(x => x.Parent.DatObjects)
		.ToListAsync();

	foreach (var result in results)
	{
		Console.WriteLine($"{result.Parent.DatObjects.First().DatName} has FirstSeason of {result.FirstSeason}");
	}

	Console.WriteLine("done");
}
//QuerySubObjects();

Console.WriteLine("Finished");
Console.ReadLine();
