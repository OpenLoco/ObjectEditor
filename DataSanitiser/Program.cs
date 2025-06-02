using Microsoft.EntityFrameworkCore;
using OpenLoco.Common.Logging;
using OpenLoco.Definitions.Database;
using System.IO.Hashing;

async static void WritexxHash3()
{
	var db = LocoDb.GetDbFromFile(LocoDb.DefaultDb);
	const string objDirectory = "Q:\\Games\\Locomotion\\Server\\Objects";
	var logger = new Logger();
	var index = ObjectIndex.LoadOrCreateIndex(objDirectory, logger);

	var objects = await db.ObjectDatLookups.Include(x => x.Object).ToListAsync();

	var count = 0;
	foreach (var lookup in objects)
	{
		if (index.TryFind((lookup.DatName, lookup.DatChecksum), out var entry))
		{
			var bytes = File.ReadAllBytes(Path.Combine(objDirectory, entry.Filename));
			lookup.xxHash3 = XxHash3.HashToUInt64(bytes);
		}

		if (count++ % 1000 == 0)
		{
			Console.WriteLine(count);
		}
	}

	_ = await db.SaveChangesAsync();

	// Note: The database must exist before this script works
	Console.WriteLine($"Database path: {LocoDb.DefaultDb}");
}

WritexxHash3();

Console.ReadLine();
