using Microsoft.EntityFrameworkCore;
using OpenLoco.Definitions.Database;
using OpenLoco.Definitions.SourceData;
using System.Text.Json;
using System.Text.Json.Serialization;

var builder = new DbContextOptionsBuilder<LocoDb>();
const string connectionString = "Data Source=Q:\\Games\\Locomotion\\Database\\loco.db";
_ = builder.UseSqlite(connectionString);
var db = new LocoDb(builder.Options);

var jsonOptions = new JsonSerializerOptions() { WriteIndented = true, Converters = { new JsonStringEnumConverter() }, };

Console.WriteLine("loading");

var authors = JsonSerializer.Serialize<IEnumerable<string>>(db.Authors.Select(a => a.Name).ToList().Order(), jsonOptions);
var tags = JsonSerializer.Serialize<IEnumerable<string>>(db.Tags.Select(t => t.Name).ToList().Order(), jsonOptions);
var licences = JsonSerializer.Serialize<IEnumerable<LicenceJsonRecord>>(db.Licences.Select(l => new LicenceJsonRecord(l.Name, l.Text)).ToList().OrderBy(l => l.Name), jsonOptions);
var objectPacks = JsonSerializer.Serialize<IEnumerable<ObjectPackJsonRecord>>(db.ObjectPacks.Select(m => new ObjectPackJsonRecord(m.Name, m.Description, m.Authors.Select(a => a.Name).ToList())).ToList().OrderBy(m => m.Name), jsonOptions);
var sc5Files = JsonSerializer.Serialize<IEnumerable<SC5FileJsonRecord>>(db.SC5Files.Select(l => new SC5FileJsonRecord(l.Name, l.Description, l.Authors.Select(a => a.Name).ToList())).ToList().OrderBy(l => l.Name), jsonOptions);
var sc5FilePacks = JsonSerializer.Serialize<IEnumerable<SC5FilePackJsonRecord>>(db.SC5FilePacks.Select(m => new SC5FilePackJsonRecord(m.Name, m.Description, m.Authors.Select(a => a.Name).ToList())).ToList().OrderBy(m => m.Name), jsonOptions);

var objs = new List<ObjectMetadata>();

foreach (var o in db.Objects
		.Include(l => l.Licence)
		.Select(x => new ExpandedTblLocoObject(x, x.Authors, x.Tags, x.ObjectPacks))
		.ToList()
		.OrderBy(x => x.Object.Name))
{
	var obj = new ObjectMetadata(
		o.Object.Name,
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

var objects = JsonSerializer.Serialize<IEnumerable<ObjectMetadata>>(objs, jsonOptions);

Console.WriteLine("writing");

File.WriteAllText("Q:\\Games\\Locomotion\\Database\\authors.json", authors);
File.WriteAllText("Q:\\Games\\Locomotion\\Database\\tags.json", tags);
File.WriteAllText("Q:\\Games\\Locomotion\\Database\\licences.json", licences);
File.WriteAllText("Q:\\Games\\Locomotion\\Database\\objectPacks.json", objectPacks);
File.WriteAllText("Q:\\Games\\Locomotion\\Database\\objectMetadata.json", objects);
File.WriteAllText("Q:\\Games\\Locomotion\\Database\\sc5Files.json", objectPacks);
File.WriteAllText("Q:\\Games\\Locomotion\\Database\\sc5FilePacks.json", objects);

Console.WriteLine("done");
