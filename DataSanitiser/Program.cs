using Microsoft.EntityFrameworkCore;
using OpenLoco.Common.Logging;
using OpenLoco.Dat.FileParsing;
using OpenLoco.Dat.Objects;
using OpenLoco.Definitions.Database;
using System.IO.Hashing;

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
				var dat = SawyerStreamReader.LoadFullObjectFromFile(filename, logger);

				if (dat == null)
				{
					Console.WriteLine($"Failed to read {entry.FileName}");
					continue;
				}

				obj.Description = dat.Value.LocoObject.StringTable.Table.First().Value[OpenLoco.Dat.Data.LanguageId.English_UK];
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
FixObjectDescriptions();

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
				var dat = SawyerStreamReader.LoadFullObjectFromFile(filename, logger);

				if (dat == null)
				{
					Console.WriteLine($"Failed to read {entry.FileName}");
					continue;
				}

				foreach (var s in dat.Value.LocoObject.StringTable.Table)
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

	foreach (var obj in objects)
	{
		var firstDatObj = obj.DatObjects.First();
		if (!index.TryFind((firstDatObj.DatName, firstDatObj.DatChecksum), out var entry))
		{
			continue;
		}

		switch (obj.ObjectType)
		{
			case OpenLoco.Dat.Data.ObjectType.Airport:
				break;
			case OpenLoco.Dat.Data.ObjectType.Bridge:
				break;
			case OpenLoco.Dat.Data.ObjectType.Building:
				break;
			case OpenLoco.Dat.Data.ObjectType.Cargo:
				break;
			case OpenLoco.Dat.Data.ObjectType.CliffEdge:
				break;
			case OpenLoco.Dat.Data.ObjectType.Climate:
				var o = SawyerStreamReader.LoadFullObjectFromFile(Path.Combine(dir, entry.FileName), logger);
				if (o?.LocoObject != null)
				{
					var struc = (ClimateObject)o.Value.LocoObject.Object;
					var dbObj = new TblObjectClimate()
					{
						Parent = obj,
						FirstSeason = struc.FirstSeason,
						WinterSnowLine = struc.WinterSnowLine,
						SummerSnowLine = struc.SummerSnowLine,
						SeasonLength1 = struc.SeasonLengths[0],
						SeasonLength2 = struc.SeasonLengths[1],
						SeasonLength3 = struc.SeasonLengths[2],
						SeasonLength4 = struc.SeasonLengths[3]
					};
					_ = await db.ObjClimate.AddAsync(dbObj);
					Console.WriteLine($"Added data for {firstDatObj.DatName} - {obj.Name}");
				}
				break;
			case OpenLoco.Dat.Data.ObjectType.Competitor:
				break;
			case OpenLoco.Dat.Data.ObjectType.Currency:
				break;
			case OpenLoco.Dat.Data.ObjectType.Dock:
				break;
			case OpenLoco.Dat.Data.ObjectType.HillShapes:
				break;
			case OpenLoco.Dat.Data.ObjectType.Industry:
				break;
			case OpenLoco.Dat.Data.ObjectType.InterfaceSkin:
				break;
			case OpenLoco.Dat.Data.ObjectType.Land:
				break;
			case OpenLoco.Dat.Data.ObjectType.LevelCrossing:
				break;
			case OpenLoco.Dat.Data.ObjectType.Region:
				break;
			case OpenLoco.Dat.Data.ObjectType.RoadExtra:
				break;
			case OpenLoco.Dat.Data.ObjectType.Road:
				break;
			case OpenLoco.Dat.Data.ObjectType.RoadStation:
				break;
			case OpenLoco.Dat.Data.ObjectType.Scaffolding:
				break;
			case OpenLoco.Dat.Data.ObjectType.ScenarioText:
				break;
			case OpenLoco.Dat.Data.ObjectType.Snow:
				break;
			case OpenLoco.Dat.Data.ObjectType.Sound:
				break;
			case OpenLoco.Dat.Data.ObjectType.Steam:
				break;
			case OpenLoco.Dat.Data.ObjectType.StreetLight:
				break;
			case OpenLoco.Dat.Data.ObjectType.TownNames:
				break;
			case OpenLoco.Dat.Data.ObjectType.TrackExtra:
				break;
			case OpenLoco.Dat.Data.ObjectType.Track:
				break;
			case OpenLoco.Dat.Data.ObjectType.TrainSignal:
				break;
			case OpenLoco.Dat.Data.ObjectType.TrainStation:
				break;
			case OpenLoco.Dat.Data.ObjectType.Tree:
				break;
			case OpenLoco.Dat.Data.ObjectType.Tunnel:
				break;
			case OpenLoco.Dat.Data.ObjectType.Vehicle:
				break;
			case OpenLoco.Dat.Data.ObjectType.Water:
				break;
			case OpenLoco.Dat.Data.ObjectType.Wall:
				break;
			default:
				break;
		}
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
QuerySubObjects();

Console.ReadLine();
