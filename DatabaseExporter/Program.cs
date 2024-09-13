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
var roles = JsonSerializer.Serialize<IEnumerable<string>>(db.Roles.Select(r => r.Name).ToList().Order(), jsonOptions);

var usrs = new List<UserJsonRecord>();
foreach (var u in db.Users
	.Include(x => x.Author)
	.ToList()
	.OrderBy(x => x.Name))
{
	var usr = new UserJsonRecord(
		u.Name,
		u.DisplayName,
		u.Author?.Name,
		u.Roles.Select(r => r.Name).Order().ToList(),
		u.PasswordHashed,
		u.PasswordSalt);
	usrs.Add(usr);
}
var users = JsonSerializer.Serialize<IEnumerable<UserJsonRecord>>(usrs, jsonOptions);

var objs = new List<ObjectMetadata>();
foreach (var o in db.Objects
		.Include(x => x.Licence)
		.Select(x => new ExpandedTblLocoObject(x, x.Authors, x.Tags, x.Modpacks))
		.ToList()
		.OrderBy(x => x.Object.Name))
{
	var obj = new ObjectMetadata(
		o.Object.OriginalName,
		o.Object.OriginalChecksum,
		o.Object.Description,
		o.Authors.Select(a => a.Name).Order().ToList(),
		o.Tags.Select(t => t.Name).Order().ToList(),
		o.Modpacks.Select(m => m.Name).Order().ToList(),
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
File.WriteAllText("Q:\\Games\\Locomotion\\Database\\users.json", users);
File.WriteAllText("Q:\\Games\\Locomotion\\Database\\roles.json", roles);

Console.WriteLine("done");
