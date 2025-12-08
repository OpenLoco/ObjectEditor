using Common.Logging;
using Dat.Data;
using Dat.FileParsing;
using Definitions.Database;
using Definitions.ObjectModels;
using Definitions.ObjectModels.Objects.Airport;
using Definitions.ObjectModels.Objects.Bridge;
using Definitions.ObjectModels.Objects.Building;
using Definitions.ObjectModels.Objects.Cargo;
using Definitions.ObjectModels.Objects.CliffEdge;
using Definitions.ObjectModels.Objects.Climate;
using Definitions.ObjectModels.Objects.Competitor;
using Definitions.ObjectModels.Objects.Currency;
using Definitions.ObjectModels.Objects.Dock;
using Definitions.ObjectModels.Objects.HillShape;
using Definitions.ObjectModels.Objects.Industry;
using Definitions.ObjectModels.Objects.InterfaceSkin;
using Definitions.ObjectModels.Objects.Land;
using Definitions.ObjectModels.Objects.LevelCrossing;
using Definitions.ObjectModels.Objects.Region;
using Definitions.ObjectModels.Objects.Road;
using Definitions.ObjectModels.Objects.RoadExtra;
using Definitions.ObjectModels.Objects.RoadStation;
using Definitions.ObjectModels.Objects.Scaffolding;
using Definitions.ObjectModels.Objects.ScenarioText;
using Definitions.ObjectModels.Objects.Snow;
using Definitions.ObjectModels.Objects.Sound;
using Definitions.ObjectModels.Objects.Steam;
using Definitions.ObjectModels.Objects.Streetlight;
using Definitions.ObjectModels.Objects.TownNames;
using Definitions.ObjectModels.Objects.Track;
using Definitions.ObjectModels.Objects.TrackExtra;
using Definitions.ObjectModels.Objects.TrackSignal;
using Definitions.ObjectModels.Objects.TrackStation;
using Definitions.ObjectModels.Objects.Tree;
using Definitions.ObjectModels.Objects.Tunnel;
using Definitions.ObjectModels.Objects.Vehicle;
using Definitions.ObjectModels.Objects.Wall;
using Definitions.ObjectModels.Objects.Water;
using Definitions.ObjectModels.Types;
using Index;
using Microsoft.EntityFrameworkCore;
using System.IO.Hashing;
using System.Reflection;
using IndustryObject = Definitions.ObjectModels.Objects.Industry.IndustryObject;
using VehicleObject = Definitions.ObjectModels.Objects.Vehicle.VehicleObject;

static ObjectType TypeToStruct(Type type)
	=> type switch
	{
		var t when t == typeof(AirportObject) => ObjectType.Airport,
		var t when t == typeof(BridgeObject) => ObjectType.Bridge,
		var t when t == typeof(BuildingObject) => ObjectType.Building,
		var t when t == typeof(CargoObject) => ObjectType.Cargo,
		var t when t == typeof(CliffEdgeObject) => ObjectType.CliffEdge,
		var t when t == typeof(ClimateObject) => ObjectType.Climate,
		var t when t == typeof(CompetitorObject) => ObjectType.Competitor,
		var t when t == typeof(CurrencyObject) => ObjectType.Currency,
		var t when t == typeof(DockObject) => ObjectType.Dock,
		var t when t == typeof(HillShapesObject) => ObjectType.HillShapes,
		var t when t == typeof(IndustryObject) => ObjectType.Industry,
		var t when t == typeof(InterfaceSkinObject) => ObjectType.InterfaceSkin,
		var t when t == typeof(LandObject) => ObjectType.Land,
		var t when t == typeof(LevelCrossingObject) => ObjectType.LevelCrossing,
		var t when t == typeof(RegionObject) => ObjectType.Region,
		var t when t == typeof(RoadExtraObject) => ObjectType.RoadExtra,
		var t when t == typeof(RoadObject) => ObjectType.Road,
		var t when t == typeof(RoadStationObject) => ObjectType.RoadStation,
		var t when t == typeof(ScaffoldingObject) => ObjectType.Scaffolding,
		var t when t == typeof(ScenarioTextObject) => ObjectType.ScenarioText,
		var t when t == typeof(SnowObject) => ObjectType.Snow,
		var t when t == typeof(SoundObject) => ObjectType.Sound,
		var t when t == typeof(SteamObject) => ObjectType.Steam,
		var t when t == typeof(StreetLightObject) => ObjectType.StreetLight,
		var t when t == typeof(TownNamesObject) => ObjectType.TownNames,
		var t when t == typeof(TrackExtraObject) => ObjectType.TrackExtra,
		var t when t == typeof(TrackObject) => ObjectType.Track,
		var t when t == typeof(TrackSignalObject) => ObjectType.TrackSignal,
		var t when t == typeof(TrackStationObject) => ObjectType.TrackStation,
		var t when t == typeof(TreeObject) => ObjectType.Tree,
		var t when t == typeof(TunnelObject) => ObjectType.Tunnel,
		var t when t == typeof(VehicleObject) => ObjectType.Vehicle,
		var t when t == typeof(WallObject) => ObjectType.Wall,
		var t when t == typeof(WaterObject) => ObjectType.Water,
		_ => throw new ArgumentOutOfRangeException(nameof(type), $"unknown struct type {type.FullName}")
	};

static void QueryBuildingProducedQuantity()
{
	var dir = "Q:\\Steam\\steamapps\\common\\Locomotion\\ObjData";
	var logger = new Logger();
	var index = ObjectIndex.LoadOrCreateIndex(dir, logger);

	var results = new List<(ObjectIndexEntry Obj, (string ProducedName, int ProducedQuantity))>();

	foreach (var obj in index.Objects.Where(x => x.ObjectType == ObjectType.Building))
	{
		try
		{
			var o = SawyerStreamReader.LoadFullObject(Path.Combine(dir, obj.FileName), logger);
			if (o.LocoObject != null)
			{
				var struc = (BuildingObject)o.LocoObject.Object;
				var header = o.DatFileInfo.S5Header;
				var source = OriginalObjectFiles.GetFileSource(header.Name, header.Checksum, header.ObjectSource);

				foreach (var cargo in struc.ProducedCargo.Zip(struc.ProducedQuantity))
				{
					results.Add((obj, (cargo.First.Name, cargo.Second)));
				}
			}
		}
		catch (Exception ex)
		{
			Console.WriteLine($"{obj.FileName} - {ex.Message}");
		}
	}

	Console.WriteLine(results.Count);

	const string csvHeader = "DatName, ProducedName, ProducedQuantity";
	var lines = results
		.OrderBy(x => x.Item2.ProducedQuantity)
		.Select(x => string.Join(',', x.Obj.DisplayName, x.Item2.ProducedName, x.Item2.ProducedQuantity));

	File.WriteAllLines("buildingProducedQuantities.csv", [csvHeader, .. lines]);

	//foreach (var line in lines)
	//{
	//	Console.WriteLine(line);
	//}
}
QueryBuildingProducedQuantity();

static void QueryHeadquarters()
{
	var dir = "Q:\\Games\\Locomotion\\Server\\Objects";
	var logger = new Logger();
	var index = ObjectIndex.LoadOrCreateIndex(dir, logger);

	var results = new List<(ObjectIndexEntry Obj, ObjectSource ObjectSource)>();

	foreach (var obj in index.Objects.Where(x => x.ObjectType == ObjectType.Building))
	{
		try
		{
			var o = SawyerStreamReader.LoadFullObject(Path.Combine(dir, obj.FileName), logger);
			if (o.LocoObject != null)
			{
				var struc = (BuildingObject)o.LocoObject.Object;
				var header = o.DatFileInfo.S5Header;
				var source = OriginalObjectFiles.GetFileSource(header.Name, header.Checksum, header.ObjectSource);

				if (struc.Flags.HasFlag(BuildingObjectFlags.IsHeadquarters))
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

	foreach (var line in lines)
	{
		Console.WriteLine(line);
	}
}
//QueryHeadquarters();

static void QueryCostIndex()
{
	var dir = "Q:\\Games\\Locomotion\\Server\\Objects";
	var logger = new Logger();
	var index = ObjectIndex.LoadOrCreateIndex(dir, logger);
	//var index = ObjectIndex.CreateIndex(dir, logger);
	//index.SaveIndexAsync(Path.Combine(dir, "objectIndex.json")).Wait();

	var results = new List<(ObjectIndexEntry Obj, ObjectSource ObjectSource, ObjectType ObjectType, byte CostIndex)>();

	var locoStructTypesWithCostIndex = AppDomain.CurrentDomain.GetAssemblies()
		.SelectMany(a => a.GetTypes())
		.Where(t =>
			t.IsClass &&
			!t.IsAbstract &&
			typeof(ILocoStruct).IsAssignableFrom(t) &&
			t.GetProperty("CostIndex", BindingFlags.Public | BindingFlags.Instance | BindingFlags.IgnoreCase) != null)
		.Select(TypeToStruct)
		.ToHashSet();

	foreach (var type in locoStructTypesWithCostIndex)
	{
		Console.WriteLine($"Type: {type} implements ILocoStruct and has a CostIndex property.");
	}

	foreach (var obj in index.Objects.Where(x => x.ObjectSource is ObjectSource.LocomotionSteam or ObjectSource.LocomotionGoG && locoStructTypesWithCostIndex.Contains(x.ObjectType)))
	//foreach (var obj in index.Objects.Where(x => locoStructTypesWithCostIndex.Contains(x.ObjectType)))
	{
		try
		{
			var o = SawyerStreamReader.LoadFullObject(Path.Combine(dir, obj.FileName), logger);
			if (o.LocoObject != null)
			{
				var type = o.LocoObject.Object.GetType();
				var costIndexProperty = type.GetProperty("CostIndex", BindingFlags.IgnoreCase | BindingFlags.Public | BindingFlags.Instance);

				var costIndex = (byte)costIndexProperty.GetValue(o.LocoObject.Object);

				var header = o.DatFileInfo.S5Header;
				var source = OriginalObjectFiles.GetFileSource(header.Name, header.Checksum, header.ObjectSource);

				results.Add((obj, source, o.LocoObject.ObjectType, costIndex));
			}
		}
		catch (Exception ex)
		{
			Console.WriteLine($"{obj.FileName} - {ex.Message}");
		}
	}

	Console.WriteLine(results.Count);
	foreach (var result in results.OrderBy(x => x.CostIndex))
	{
		// Print object index entry and its enabled flags
		Console.WriteLine($"{result.Obj.DisplayName} - {result.ObjectSource} - {result.ObjectType} - CostIndex={result.CostIndex}");
	}
}
//QueryCostIndex();

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
				var source = OriginalObjectFiles.GetFileSource(header.Name, header.Checksum, header.ObjectSource);

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
//QueryTrackStationOneSidedTrack();

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
				var source = OriginalObjectFiles.GetFileSource(header.Name, header.Checksum, header.ObjectSource);

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
				var source = OriginalObjectFiles.GetFileSource(header.Name, header.Checksum, header.ObjectSource);

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
				var source = OriginalObjectFiles.GetFileSource(header.Name, header.Checksum, header.ObjectSource);

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
