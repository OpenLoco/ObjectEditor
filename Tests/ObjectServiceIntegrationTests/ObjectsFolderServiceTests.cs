using Dat.Tests;
using Definitions;
using Definitions.Database;
using Definitions.ObjectModels.Types;
using Index;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging.Abstractions;
using NUnit.Framework;
using ObjectService.Services;

namespace ObjectService.Tests.Integration;

/// <summary>
/// Exercises the code path that the <see cref="GameDataWatcherService"/> invokes when a DAT
/// file is dropped into the server's <c>GameData/Objects</c> folder: the object must end up in both
/// the SQLite database and the persisted <c>objectIndex.json</c>.
/// </summary>
[TestFixture]
public class ObjectsFolderServiceTests
{
	private static string? FindSmallestSourceDat()
	{
		if (!Directory.Exists(TestConstants.BaseSteamObjDataPath))
		{
			return null;
		}

		return Directory
			.GetFiles(TestConstants.BaseSteamObjDataPath, "*.dat")
			.OrderBy(x => new FileInfo(x).Length)
			.ThenBy(x => x, StringComparer.OrdinalIgnoreCase)
			.FirstOrDefault();
	}

	[Test]
	public async Task ImportAsync_AddsObjectToDatabaseAndIndex()
	{
		var source = FindSmallestSourceDat();
		if (source == null)
		{
			Assert.Ignore("No source DAT files are available to import");
		}

		var root = Directory.CreateTempSubdirectory("object-file-import").FullName;
		try
		{
			var sfm = new ServerFolderManager(root);
			var destination = Path.Combine(sfm.ObjectsCustomFolder, Path.GetFileName(source!));
			File.Copy(source!, destination);

			using var connection = new SqliteConnection("DataSource=:memory:");
			connection.Open();

			var options = new DbContextOptionsBuilder<LocoDbContext>()
				.UseSqlite(connection)
				.Options;

			using var db = new LocoDbContext(options);
			_ = db.Database.EnsureCreated();

			var service = new ObjectsFolderService(
				db,
				sfm,
				NullLogger<ObjectsFolderService>.Instance,
				NullLoggerFactory.Instance);

			var result = await service.ImportAsync(destination, CancellationToken.None);

			using (Assert.EnterMultipleScope())
			{
				Assert.That(result.Status, Is.EqualTo(GameDataImportStatus.Added));
				Assert.That(result.Entry, Is.Not.Null);
			}

			// The database must now contain the object and its DAT mapping.
			var dbObject = await db.Objects
				.Include(x => x.DatObjects)
				.SingleAsync();

			using (Assert.EnterMultipleScope())
			{
				Assert.That(dbObject.DatObjects, Has.Count.EqualTo(1));
				Assert.That(dbObject.DatObjects.First().DatName, Is.EqualTo(result.Entry!.DisplayName));
				Assert.That(dbObject.DatObjects.First().DatChecksum, Is.EqualTo(result.Entry!.DatChecksum));
			}

			// The relative filename stored in the index must resolve back to the dropped file.
			Assert.That(result.Entry!.FileName, Is.EqualTo(Path.GetRelativePath(sfm.ObjectsFolder, destination)));
			Assert.That(File.Exists(Path.Combine(sfm.ObjectsFolder, result.Entry!.FileName!)), Is.True);

			// The in-memory index and the persisted index file must both know about the object.
			Assert.That(sfm.ObjectIndex.TryFind((result.Entry!.DisplayName, result.Entry!.DatChecksum!.Value), out var indexEntry), Is.True);
			Assert.That(indexEntry, Is.Not.Null);
			Assert.That(File.Exists(sfm.IndexFile), Is.True);

			var persistedIndex = await ObjectIndex.LoadIndexAsync(sfm.IndexFile);
			Assert.That(persistedIndex, Is.Not.Null);
			Assert.That(persistedIndex!.Objects.Any(x => x.DisplayName == result.Entry!.DisplayName && x.DatChecksum == result.Entry!.DatChecksum), Is.True);
		}
		finally
		{
			Directory.Delete(root, recursive: true);
		}
	}

	[Test]
	public async Task ImportAsync_ReturnsFailed_ForNonDatFile()
	{
		var root = Directory.CreateTempSubdirectory("object-file-import-invalid").FullName;
		try
		{
			var sfm = new ServerFolderManager(root);
			var destination = Path.Combine(sfm.ObjectsCustomFolder, "not-an-object.dat");
			await File.WriteAllTextAsync(destination, "this is definitely not a DAT file");

			using var connection = new SqliteConnection("DataSource=:memory:");
			connection.Open();

			var options = new DbContextOptionsBuilder<LocoDbContext>()
				.UseSqlite(connection)
				.Options;

			using var db = new LocoDbContext(options);
			_ = db.Database.EnsureCreated();

			var service = new ObjectsFolderService(
				db,
				sfm,
				NullLogger<ObjectsFolderService>.Instance,
				NullLoggerFactory.Instance);

			var result = await service.ImportAsync(destination, CancellationToken.None);

			using (Assert.EnterMultipleScope())
			{
				Assert.That(result.Status, Is.EqualTo(GameDataImportStatus.Failed));
				Assert.That(db.Objects.Any(), Is.False);
				Assert.That(sfm.ObjectIndex.Objects, Is.Empty);
			}
		}
		finally
		{
			Directory.Delete(root, recursive: true);
		}
	}

	[Test]
	public async Task RemoveAsync_RemovesIndexEntryAndMarksObjectUnavailable()
	{
		var source = FindSmallestSourceDat();
		if (source == null)
		{
			Assert.Ignore("No source DAT files are available to import");
		}

		var root = Directory.CreateTempSubdirectory("object-file-remove").FullName;
		try
		{
			var sfm = new ServerFolderManager(root);
			var destination = Path.Combine(sfm.ObjectsCustomFolder, Path.GetFileName(source!));
			File.Copy(source!, destination);

			using var connection = new SqliteConnection("DataSource=:memory:");
			connection.Open();

			var options = new DbContextOptionsBuilder<LocoDbContext>()
				.UseSqlite(connection)
				.Options;

			using var db = new LocoDbContext(options);
			_ = db.Database.EnsureCreated();

			var service = new ObjectsFolderService(
				db,
				sfm,
				NullLogger<ObjectsFolderService>.Instance,
				NullLoggerFactory.Instance);

			var import = await service.ImportAsync(destination, CancellationToken.None);
			Assert.That(import.Status, Is.EqualTo(GameDataImportStatus.Added));

			// act - the file is deleted out from under the server
			File.Delete(destination);
			var remove = await service.RemoveAsync(destination, CancellationToken.None);

			using (Assert.EnterMultipleScope())
			{
				Assert.That(remove.Status, Is.EqualTo(GameDataImportStatus.Unavailable));
				Assert.That(sfm.ObjectIndex.Objects, Is.Empty);
			}

			var dbObject = await db.Objects.SingleAsync();
			Assert.That(dbObject.Availability, Is.EqualTo(ObjectAvailability.Unavailable));

			var persistedIndex = await ObjectIndex.LoadIndexAsync(sfm.IndexFile);
			Assert.That(persistedIndex!.Objects, Is.Empty);
		}
		finally
		{
			Directory.Delete(root, recursive: true);
		}
	}

	[Test]
	public async Task ReconcileAsync_IgnoresFilesParkedInRemovedFolder()
	{
		var source = FindSmallestSourceDat();
		if (source == null)
		{
			Assert.Ignore("No source DAT files are available to import");
		}

		var root = Directory.CreateTempSubdirectory("object-file-removed").FullName;
		try
		{
			var sfm = new ServerFolderManager(root);
			var fileName = Path.GetFileName(source!);

			// The same DAT exists under Custom and under Removed; only the Custom copy may be indexed.
			_ = Directory.CreateDirectory(Path.Combine(sfm.ObjectsRemovedFolder, ServerFolderManager.CustomFolderName));
			File.Copy(source!, Path.Combine(sfm.ObjectsCustomFolder, fileName));
			File.Copy(source!, Path.Combine(sfm.ObjectsRemovedFolder, ServerFolderManager.CustomFolderName, fileName));

			using var connection = new SqliteConnection("DataSource=:memory:");
			connection.Open();

			var options = new DbContextOptionsBuilder<LocoDbContext>()
				.UseSqlite(connection)
				.Options;

			using var db = new LocoDbContext(options);
			_ = db.Database.EnsureCreated();

			var service = new ObjectsFolderService(
				db,
				sfm,
				NullLogger<ObjectsFolderService>.Instance,
				NullLoggerFactory.Instance);

			await service.ReconcileAsync(CancellationToken.None);

			using (Assert.EnterMultipleScope())
			{
				Assert.That(sfm.ObjectIndex.Objects, Has.Count.EqualTo(1));
				Assert.That(await db.Objects.CountAsync(), Is.EqualTo(1));

				var entry = sfm.ObjectIndex.Objects.Single();
				Assert.That(entry.FileName, Is.EqualTo(Path.Combine(ServerFolderManager.CustomFolderName, fileName)));
			}
		}
		finally
		{
			Directory.Delete(root, recursive: true);
		}
	}

	[Test]
	public async Task RemoveThenReImport_FlipsAvailabilityAndRestoresTheObject()
	{
		var source = FindSmallestSourceDat();
		if (source == null)
		{
			Assert.Ignore("No source DAT files are available to import");
		}

		var root = Directory.CreateTempSubdirectory("object-file-roundtrip").FullName;
		try
		{
			var sfm = new ServerFolderManager(root);
			var destination = Path.Combine(sfm.ObjectsCustomFolder, Path.GetFileName(source!));
			File.Copy(source!, destination);

			using var connection = new SqliteConnection("DataSource=:memory:");
			connection.Open();

			var options = new DbContextOptionsBuilder<LocoDbContext>()
				.UseSqlite(connection)
				.Options;

			using var db = new LocoDbContext(options);
			_ = db.Database.EnsureCreated();

			var service = new ObjectsFolderService(
				db,
				sfm,
				NullLogger<ObjectsFolderService>.Instance,
				NullLoggerFactory.Instance);

			_ = await service.ImportAsync(destination, CancellationToken.None);
			Assert.That((await db.Objects.SingleAsync()).Availability, Is.EqualTo(ObjectAvailability.Available));

			// The file disappears (removal, or a manual move into Removed): the object becomes unavailable.
			File.Delete(destination);
			_ = await service.RemoveAsync(destination, CancellationToken.None);

			using (Assert.EnterMultipleScope())
			{
				Assert.That((await db.Objects.SingleAsync()).Availability, Is.EqualTo(ObjectAvailability.Unavailable));
				Assert.That(sfm.ObjectIndex.Objects, Is.Empty);
			}

			// Restoring the file makes the object available again, so a removal is fully recoverable.
			File.Copy(source!, destination);
			_ = await service.ImportAsync(destination, CancellationToken.None);

			using (Assert.EnterMultipleScope())
			{
				Assert.That((await db.Objects.SingleAsync()).Availability, Is.EqualTo(ObjectAvailability.Available));
				Assert.That(sfm.ObjectIndex.Objects, Has.Count.EqualTo(1));
			}
		}
		finally
		{
			Directory.Delete(root, recursive: true);
		}
	}

	private const string ServerObjectsFolder = @"Q:\Games\Locomotion\Server\GameData\Objects";

	/// <summary>
	/// Finds a DAT file whose content identifies it as an OpenLoco object, wherever it currently lives.
	/// </summary>
	private static string? FindOpenLocoSourceDat()
	{
		foreach (var folder in new[] { ServerFolderManager.CustomFolderName, ServerFolderManager.OpenLocoFolderName })
		{
			var root = Path.Combine(ServerObjectsFolder, folder);
			if (!Directory.Exists(root))
			{
				continue;
			}

			foreach (var file in Directory.GetFiles(root, "*.dat").OrderBy(x => new FileInfo(x).Length))
			{
				try
				{
					var entry = ObjectIndex.GetDatFileInfoFromBytes(file, Path.GetFileName(file), File.ReadAllBytes(file), NullLogger.Instance);
					if (entry?.ObjectSource == ObjectSource.OpenLoco)
					{
						return file;
					}
				}
				catch (Exception)
				{
					// Unreadable file - keep looking.
				}
			}
		}

		return null;
	}

	[Test]
	public async Task ReconcileAsync_MovesOpenLocoObjectsOutOfTheCustomFolder()
	{
		var source = FindOpenLocoSourceDat();
		if (source == null)
		{
			Assert.Ignore("No OpenLoco object is available to relocate");
		}

		var root = Directory.CreateTempSubdirectory("object-file-relocate").FullName;
		try
		{
			var sfm = new ServerFolderManager(root);
			var fileName = Path.GetFileName(source!);
			var customPath = Path.Combine(sfm.ObjectsCustomFolder, fileName);
			File.Copy(source!, customPath);

			using var connection = new SqliteConnection("DataSource=:memory:");
			connection.Open();

			var options = new DbContextOptionsBuilder<LocoDbContext>()
				.UseSqlite(connection)
				.Options;

			using var db = new LocoDbContext(options);
			_ = db.Database.EnsureCreated();

			var service = new ObjectsFolderService(
				db,
				sfm,
				NullLogger<ObjectsFolderService>.Instance,
				NullLoggerFactory.Instance);

			// Seed the index exactly as it would have been before the relocation existed: the file is in
			// Custom but its content says OpenLoco.
			var relative = Path.Combine(ServerFolderManager.CustomFolderName, fileName);
			var entry = ObjectIndex.GetDatFileInfoFromBytes(customPath, relative, File.ReadAllBytes(customPath), NullLogger.Instance);
			Assert.That(entry, Is.Not.Null);
			sfm.ObjectIndex.AddEntry(entry!);

			await service.ReconcileAsync(CancellationToken.None);

			var openLocoPath = Path.Combine(sfm.ObjectsOpenLocoFolder, fileName);

			using (Assert.EnterMultipleScope())
			{
				Assert.That(File.Exists(openLocoPath), Is.True, "the file should have been moved into the OpenLoco folder");
				Assert.That(File.Exists(customPath), Is.False, "the file should no longer be in the Custom folder");

				var relocated = sfm.ObjectIndex.Objects.Single();
				Assert.That(relocated.ObjectSource, Is.EqualTo(ObjectSource.OpenLoco));
				Assert.That(relocated.FileName, Is.EqualTo(Path.Combine(ServerFolderManager.OpenLocoFolderName, fileName)));

				Assert.That((await db.Objects.SingleAsync()).ObjectSource, Is.EqualTo(ObjectSource.OpenLoco));
			}
		}
		finally
		{
			Directory.Delete(root, recursive: true);
		}
	}
}
