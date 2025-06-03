using Microsoft.EntityFrameworkCore;
using OpenLoco.Common.Logging;
using OpenLoco.Dat.FileParsing;
using OpenLoco.Definitions.Database;
using System.IO.Hashing;

async static void WritexxHash3()
{
	var db = LocoDb.GetDbFromFile(LocoDb.DefaultDb);
	const string objDirectory = "Q:\\Games\\Locomotion\\Server\\Objects";
	var logger = new Logger();
	var index = ObjectIndex.LoadOrCreateIndex(objDirectory, logger);

	var objects = await db.DatObjects.Include(x => x.Object).ToListAsync();

	var count = 0;
	foreach (var lookup in objects)
	{
		if (index.TryFind((lookup.DatName, lookup.DatChecksum), out var entry))
		{
			var filename = Path.Combine(objDirectory, entry.Filename);
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
	Console.WriteLine($"Database path: {LocoDb.DefaultDb}");
}
//WritexxHash3();

async static void WriteStringTable()
{
	var db = LocoDb.GetDbFromFile(LocoDb.DefaultDb);
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
			var filename = Path.Combine(objDirectory, entry.Filename);
			var bytes = File.ReadAllBytes(filename);

			try
			{
				var dat = SawyerStreamReader.LoadFullObjectFromFile(filename, logger);

				if (dat == null)
				{
					Console.WriteLine($"Failed to read {entry.Filename}");
					continue;
				}

				foreach (var s in dat.Value.LocoObject.StringTable.Table)
				{
					foreach (var t in s.Value)
					{
						obj.StringTable.Add(new TblStringTable()
						{
							RowName = s.Key,
							RowLanguage = t.Key,
							RowText = t.Value,
							ObjectId = obj.Id,
						});
					}
				}
			}
			catch (Exception ex)
			{
				Console.WriteLine($"Exception, failed to read {entry.Filename}, exception={ex}");
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
	Console.WriteLine($"Database path: {LocoDb.DefaultDb}");
}
//WriteStringTable();

Console.ReadLine();
