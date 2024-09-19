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
var modpacks = JsonSerializer.Serialize<IEnumerable<ModpackJsonRecord>>(db.Modpacks.Select(m => new ModpackJsonRecord(m.Name, m.Author)).ToList().OrderBy(m => m.Name), jsonOptions);

var objs = new List<ObjectMetadata>();

foreach (var o in db.Objects
		.Include(l => l.Licence)
		.Select(x => new ExpandedTblLocoObject(x, x.Authors, x.Tags, x.Modpacks))
		.ToList()
		.OrderBy(x => x.Object.UniqueName))
{
	var obj = new ObjectMetadata(
		o.Object.DatName,
		o.Object.DatChecksum,
		o.Object.Description,
		o.Authors.Select(a => a.Name).ToList(),
		o.Tags.Select(t => t.Name).ToList(),
		o.Modpacks.Select(m => m.Name).ToList(),
		o.Object.Licence?.Name);
	objs.Add(obj);
}

var objects = JsonSerializer.Serialize<IEnumerable<ObjectMetadata>>(objs, jsonOptions);

Console.WriteLine("writing");

File.WriteAllText("Q:\\Games\\Locomotion\\Database\\authors.json", authors);
File.WriteAllText("Q:\\Games\\Locomotion\\Database\\tags.json", tags);
File.WriteAllText("Q:\\Games\\Locomotion\\Database\\licences.json", licences);
File.WriteAllText("Q:\\Games\\Locomotion\\Database\\modpacks.json", modpacks);
File.WriteAllText("Q:\\Games\\Locomotion\\Database\\objects.json", objects);

Console.WriteLine("done");
