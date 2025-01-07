using Common.Json;
using Microsoft.EntityFrameworkCore;
using OpenLoco.Definitions.Database;
using OpenLoco.Definitions.SourceData;
using System.Text.Json;

var builder = new DbContextOptionsBuilder<LocoDb>();
const string connectionString = "Data Source=Q:\\Games\\Locomotion\\Database\\loco.db";
_ = builder.UseSqlite(connectionString);
var db = new LocoDb(builder.Options);

Console.WriteLine("loading");

var authors = JsonSerializer.Serialize<IEnumerable<string>>(db.Authors.Select(a => a.Name).ToList().Order(), JsonFile.SerializerOptions);
var tags = JsonSerializer.Serialize<IEnumerable<string>>(db.Tags.Select(t => t.Name).ToList().Order(), JsonFile.SerializerOptions);
var licences = JsonSerializer.Serialize<IEnumerable<LicenceJsonRecord>>(db.Licences.Select(l => new LicenceJsonRecord(l.Name, l.Text)).ToList().OrderBy(l => l.Name), JsonFile.SerializerOptions);
var objectPacks = JsonSerializer.Serialize<IEnumerable<ObjectPackJsonRecord>>(db.ObjectPacks.Select(m => new ObjectPackJsonRecord(m.Name, m.Description, m.Author)).ToList().OrderBy(m => m.Name), JsonFile.SerializerOptions);
//var scv5Files = JsonSerializer.Serialize<IEnumerable<?>>(db.Licences.Select(l => new LicenceJsonRecord(l.Name, l.Text)).ToList().OrderBy(l => l.Name), JsonFile.SerializerOptions);
//var scv5FilePacks = JsonSerializer.Serialize<IEnumerable<SCV5FilePackJsonRecord>>(db.ObjectPacks.Select(m => new SCV5FilePackJsonRecord(m.Name, m.Description, m.Author)).ToList().OrderBy(m => m.Name), JsonFile.SerializerOptions);

var objs = new List<ObjectMetadata>();

foreach (var o in db.Objects
		.Include(l => l.Licence)
		.Select(x => new ExpandedTblLocoObject(x, x.Authors, x.Tags, x.ObjectPacks))
		.ToList()
		.OrderBy(x => x.Object.UniqueName))
{
	var obj = new ObjectMetadata(
		o.Object.UniqueName,
		o.Object.DatName,
		o.Object.DatChecksum,
		o.Object.Description,
		o.Authors.Select(a => a.Name).ToList(),
		o.Tags.Select(t => t.Name).ToList(),
		o.ObjectPacks.Select(m => m.Name).ToList(),
		o.Object.Licence?.Name,
		o.Object.Availability,
		o.Object.ObjectSource);
	objs.Add(obj);
}

var objects = JsonSerializer.Serialize<IEnumerable<ObjectMetadata>>(objs, JsonFile.SerializerOptions);

Console.WriteLine("writing");

File.WriteAllText("Q:\\Games\\Locomotion\\Database\\authors.json", authors);
File.WriteAllText("Q:\\Games\\Locomotion\\Database\\tags.json", tags);
File.WriteAllText("Q:\\Games\\Locomotion\\Database\\licences.json", licences);
File.WriteAllText("Q:\\Games\\Locomotion\\Database\\objectPacks.json", objectPacks);
File.WriteAllText("Q:\\Games\\Locomotion\\Database\\objectMetadata.json", objects);
//File.WriteAllText("Q:\\Games\\Locomotion\\Database\\scv5Files.json", objectPacks);
//File.WriteAllText("Q:\\Games\\Locomotion\\Database\\scv5FilePacks.json", objects);

Console.WriteLine("done");
