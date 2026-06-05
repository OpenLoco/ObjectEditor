using Definitions;
using Definitions.Database;
using Definitions.ObjectModels.Types;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using NUnit.Framework;
using ObjectService.Hosting;

namespace ObjectService.Tests.Integration;

[TestFixture]
public class ObjectServiceHostModeTests
{
	[Test]
	public async Task BootstrapAsync_LocalHost_NormalisesObjectAvailability()
	{
		var testDirectory = Directory.CreateTempSubdirectory("ObjectServiceLocalHost");
		try
		{
			var databaseFile = Path.Combine(testDirectory.FullName, "local.db");
			var connectionString = $"Data Source={databaseFile}";
			var dbOptions = new DbContextOptionsBuilder<ClientLocoDbContext>()
				.UseSqlite(connectionString)
				.Options;

			await using (var db = new ClientLocoDbContext(dbOptions))
			{
				_ = await db.Database.EnsureCreatedAsync();
				var obj = new TblObject
				{
					Name = "local-unavailable-object",
					ObjectType = ObjectType.Vehicle,
					ObjectSource = ObjectSource.Custom,
					Availability = ObjectAvailability.Unavailable,
					SubObjectId = 0,
				};

				obj.DatObjects.Add(new TblDatObject
				{
					DatName = "LOCALOBJ",
					DatChecksum = 123,
					xxHash3 = 456,
					ObjectId = 0,
					Object = obj,
				});

				_ = db.Objects.Add(obj);
				_ = await db.SaveChangesAsync();
			}

			await ObjectServiceHost.BootstrapAsync(new ObjectServiceHostOptions
			{
				RootFolder = Path.Combine(testDirectory.FullName, "server-root"),
				DatabaseFile = databaseFile,
				PaletteMapFile = Path.Combine(testDirectory.FullName, "palette.png"),
				JwtKey = "0123456789abcdef0123456789abcdef",
				IsServer = false,
			});

			await using var verifiedDb = new ClientLocoDbContext(dbOptions);
			var availability = await verifiedDb.Objects
				.Select(x => x.Availability)
				.SingleAsync();

			Assert.That(availability, Is.EqualTo(ObjectAvailability.Available));
		}
		finally
		{
			SqliteConnection.ClearAllPools();
			testDirectory.Delete(recursive: true);
		}
	}
}
