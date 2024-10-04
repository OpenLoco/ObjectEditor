// 1. objectMetadata.json must contain a single record for every unique object we have
using OpenLoco.Common.Logging;
using OpenLoco.Dat;
using OpenLoco.Dat.Data;
using OpenLoco.Dat.FileParsing;
using OpenLoco.Dat.Objects;
using OpenLoco.Dat.Types;
using OpenLoco.Definitions.SourceData;
using System.Text.Json;
using System.Text.Json.Serialization;

var jsonOptions = new JsonSerializerOptions() { WriteIndented = true, Converters = { new JsonStringEnumConverter() }, };
var objectMetadata = JsonSerializer.Deserialize<IEnumerable<ObjectMetadata>>(File.ReadAllText("Q:\\Games\\Locomotion\\Server\\Objects\\objectMetadata.json"), jsonOptions)
	.ToDictionary(x => (x.DatName, x.DatChecksum), x => x);

Console.WriteLine($"MetadataCount={objectMetadata.Count}");

var dir = "Q:\\Games\\Locomotion\\Server\\Objects";
var index = ObjectIndex.LoadOrCreateIndex(dir);
var logger = new Logger();

foreach (var meta in objectMetadata)
{
	if (!index.TryFind(meta.Key, out var entry) || entry == null)
	{
		continue;
	}

	var source = OriginalObjectFiles.GetFileSource(meta.Key);
	var sourceString = source switch
	{
		ObjectSource.Custom => "custom",
		ObjectSource.LocomotionSteam => "loco.steam",
		ObjectSource.LocomotionGoG => "loco.gog",
		ObjectSource.OpenLoco => "openloco",
		_ => throw new NotImplementedException(),
	};

	var vehicleString = entry.ObjectType.ToString().ToLower();
	if (entry.VehicleType != null)
	{
		vehicleString += entry.VehicleType.ToString()!.ToLower();
	}

	var uniqueName = string.Join('.', source.ToString(), entry.VehicleType.ToString(), entry.DatName);

	// check uniqueness and append an identifier if necessary

	objectMetadata[meta.Key] = meta.Value with { UniqueName = uniqueName };
}

var objectList = new List<(DatFileInfo DatFileInfo, VehicleObject Vehicle)>();
var count = 0;
foreach (var obj in index.Objects.Where(x => x.ObjectType == ObjectType.Vehicle))
{
	try
	{
		var o = SawyerStreamReader.LoadFullObjectFromFile(Path.Combine(dir, obj.Filename), logger);
		if (o?.LocoObject != null)
		{
			objectList.Add((o!.Value.DatFileInfo, o!.Value.LocoObject.Object! as VehicleObject));
		}
	}
	catch (Exception ex)
	{
		Console.WriteLine($"{obj.Filename} - {ex.Message}");
	}

	if (count++ % 1000 == 0)
	{
		Console.WriteLine(count);
	}
}

// every object is loaded in ram here - do your query:
var cargo1 = objectList
	.OrderByDescending(x => x.Vehicle.CompatibleCargoCategories1.Count)
	.Take(10);

foreach (var x in cargo1)
{
	Console.WriteLine($"{x.DatFileInfo.S5Header.Name} - ({x.Vehicle.CompatibleCargoCategories1.Count})");
	foreach (var c in x.Vehicle.CompatibleCargoCategories1)
	{
		Console.WriteLine($"  - {c}");
	}
}

Console.WriteLine("-----");

var cargo2 = objectList
	.OrderByDescending(x => x.Vehicle.CompatibleCargoCategories2.Count)
	.Take(10);

foreach (var x in cargo2)
{
	Console.WriteLine($"{x.DatFileInfo.S5Header.Name} - ({x.Vehicle.CompatibleCargoCategories2.Count})");
	foreach (var c in x.Vehicle.CompatibleCargoCategories2)
	{
		Console.WriteLine($"  - {c}");
	}
}

Console.WriteLine($"ObjectCount={index.Objects.Count}");

//AddNewObjectMetadataEntries();

void AddNewObjectMetadataEntries()
{
	foreach (var obj in index.Objects)
	{
		var key = (obj.DatName, obj.DatChecksum);
		var source = OriginalObjectFiles.GetFileSource(key);

		if (!objectMetadata.ContainsKey(key))
		{
			objectMetadata.Add(key, new ObjectMetadata(Path.GetFileNameWithoutExtension(obj.Filename), obj.DatName, obj.DatChecksum, null, [], [], [], null, OpenLoco.Definitions.ObjectAvailability.AllGames, source));
		}
	}

	var objs = JsonSerializer.Serialize<IEnumerable<ObjectMetadata>>(objectMetadata.Values.OrderBy(x => x.DatName), jsonOptions);
	File.WriteAllText("Q:\\Games\\Locomotion\\Server\\objectMetadata.json", objs);
}

//var objs = JsonSerializer.Serialize<IEnumerable<ObjectMetadata>>(objectMetadata.Values.OrderBy(x => x.DatName), jsonOptions);
//File.WriteAllText("Q:\\Games\\Locomotion\\Server\\objectMetadata.json", objs);

Console.ReadLine();
