// 1. objectMetadata.json must contain a single record for every unique object we have
using Dat;
using OpenLoco.Dat.Data;
using OpenLoco.Definitions.SourceData;
using System.Text.Json;
using System.Text.Json.Serialization;

var jsonOptions = new JsonSerializerOptions() { WriteIndented = true, Converters = { new JsonStringEnumConverter() }, };
var objectMetadata = JsonSerializer.Deserialize<IEnumerable<ObjectMetadata>>(File.ReadAllText("Q:\\Games\\Locomotion\\Server\\Objects\\objectMetadata.json"), jsonOptions)
	.ToDictionary(x => (x.ObjectName, x.ObjectChecksum), x => x);

Console.WriteLine($"MetadataCount={objectMetadata.Count()}");

var dir = "Q:\\Games\\Locomotion\\Server\\Objects";
var index = ObjectIndex.CreateIndex(dir);

Console.WriteLine($"ObjectCount={index.Objects.Count}");
Console.WriteLine($"ObjectFailedCount={index.ObjectsFailed.Count}");

//AddNewObjectMetadataEntries();

foreach (var failed in index.ObjectsFailed)
{
	Console.WriteLine($"Faulty object: {failed.Filename}");
}

//foreach (var missing in index.Objects.Where(x => !objectMetadata.ContainsKey((x.ObjectName, x.Checksum))))
//{
//	Console.WriteLine(missing.Filename);
//}

//foreach (var (ObjectName, ObjectChecksum) in objectMetadata.Keys.Where(x => !index.Objects.Select(x => (x.ObjectName, x.Checksum)).Contains((x.ObjectName, x.ObjectChecksum))))
//{
//	Console.WriteLine($"{ObjectName} - {ObjectChecksum}");
//}

//var duplicates = index.Objects
//	.GroupBy(x => (x.ObjectName, x.Checksum))
//	.Where(g => g.Count() > 1);

//foreach (var d in duplicates)
//{
//	Console.WriteLine($"{d.Key}");
//	foreach (var dd in d)
//	{
//		var meta = objectMetadata[d.Key].UniqueName;
//		var shouldDelete = $"{meta}.dat" != dd.Filename;
//		var deleteFilename = Path.Combine(dir, dd.Filename);
//		Console.WriteLine($"  - {dd.Filename} MetadataUniqueName={meta} ShouldDelete={shouldDelete} DeleteFilename={deleteFilename}");
//		if (shouldDelete)
//		{
//			//File.Delete(deleteFilename);
//			//Console.WriteLine($"Deleted {deleteFilename}");
//		}
//	}
//}

// delete original objects from custom folder
foreach (var obj in index.Objects)
{
	if (OriginalObjectFiles.GetFileSource(obj.ObjectName, obj.Checksum) != FileSource.Custom)
	{
		if (obj.Filename.StartsWith("CustomObjects"))
		{
			var deleteFilename = Path.Combine(dir, obj.Filename);
			Console.WriteLine($"Original obj in custom folder: {deleteFilename}");
			//File.Delete(deleteFilename);
			//objectMetadata.Remove()
		}
		// we've got a duplicate - delete it from the custom folder
	}
}

//var indexDict = index.Objects.ToDictionary(x => (x.ObjectName, x.Checksum), x => x);

//Console.WriteLine($"IndexDictCount={indexDict.Count()}");
Console.WriteLine($"MetadataCount={objectMetadata.Count()}");

void AddNewObjectMetadataEntries()
{
	foreach (var obj in index.Objects)
	{
		var key = (obj.ObjectName, obj.Checksum);
		if (!objectMetadata.ContainsKey(key))
		{
			objectMetadata.Add(key, new ObjectMetadata(Path.GetFileNameWithoutExtension(obj.Filename), obj.ObjectName, obj.Checksum, null, [], [], [], null));
		}
	}

	var objs = JsonSerializer.Serialize<IEnumerable<ObjectMetadata>>(objectMetadata.Values.OrderBy(x => x.ObjectName), jsonOptions);
	File.WriteAllText("Q:\\Games\\Locomotion\\Server\\objectMetadata.json", objs);
}

//var objs = JsonSerializer.Serialize<IEnumerable<ObjectMetadata>>(objectMetadata.Values.OrderBy(x => x.ObjectName), jsonOptions);
//File.WriteAllText("Q:\\Games\\Locomotion\\Server\\objectMetadata.json", objs);

Console.ReadLine();
