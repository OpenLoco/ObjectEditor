using Common;
using Common.Json;
using Common.Logging;
using Definitions;
using Definitions.Database;
using Definitions.ObjectModels.Types;
using Definitions.SourceData;
using Microsoft.EntityFrameworkCore;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace DatabaseTools.Services;

public static class DatabaseImportService
{
	public static Task SeedAllAsync(ToolsSettings settings, Action<string> log)
		=> Task.Run(() =>
		{
			using var db = LocoDbContext.GetDbFromFile(settings.DatabaseFile)
				?? throw new InvalidOperationException($"Database not found at {settings.DatabaseFile}");

			log($"Database: {settings.DatabaseFile}");

			if (settings.DeleteExistingOnImport)
			{
				log("Clearing database…");
				_ = db.ObjectPacks.ExecuteDelete();
				_ = db.SC5FilePacks.ExecuteDelete();
				_ = db.SC5Files.ExecuteDelete();
				_ = db.Tags.ExecuteDelete();
				_ = db.Authors.ExecuteDelete();
				_ = db.Objects.ExecuteDelete();
				_ = db.Licences.ExecuteDelete();
			}

			var jsonDir = settings.JsonDirectory;
			var authorsJson = Path.Combine(jsonDir, "authors.json");
			var tagsJson = Path.Combine(jsonDir, "tags.json");
			var licencesJson = Path.Combine(jsonDir, "licences.json");
			var sc5FilePacksJson = Path.Combine(jsonDir, "sc5FilePacks.json");
			var sc5FilesJson = Path.Combine(jsonDir, "sc5Files.json");
			var objectPacksJson = Path.Combine(jsonDir, "objectPacks.json");
			var objectMetadataJson = Path.Combine(jsonDir, "objectMetadata.json");

			var logger = new Logger();
			var jsonOptions = JsonFile.DefaultSerializerOptions;
			jsonOptions.Converters.Add(new JsonStringEnumConverter());

			if (!db.Authors.Any() && File.Exists(authorsJson))
			{
				log("Seeding Authors");
				var authors = JsonSerializer.Deserialize<IEnumerable<string>>(File.ReadAllText(authorsJson), jsonOptions);
				if (authors != null)
				{
					db.AddRange(authors.Select(x => new TblAuthor { Name = x }));
					_ = db.SaveChanges();
				}
			}

			if (!db.Tags.Any() && File.Exists(tagsJson))
			{
				log("Seeding Tags");
				var tags = JsonSerializer.Deserialize<IEnumerable<string>>(File.ReadAllText(tagsJson), jsonOptions);
				if (tags != null)
				{
					db.AddRange(tags.Select(x => new TblTag { Name = x }));
					_ = db.SaveChanges();
				}
			}

			if (!db.Licences.Any() && File.Exists(licencesJson))
			{
				log("Seeding Licences");
				var licences = JsonSerializer.Deserialize<IEnumerable<LicenceJsonRecord>>(File.ReadAllText(licencesJson), jsonOptions);
				if (licences != null)
				{
					db.AddRange(licences.Select(x => new TblLicence { Name = x.Name, Text = x.Text }));
					_ = db.SaveChanges();
				}
			}

			if (!db.SC5FilePacks.Any() && File.Exists(sc5FilePacksJson))
			{
				log("Seeding SC5FilePacks");
				var sc5FilePacks = JsonSerializer.Deserialize<IEnumerable<SC5FilePackJsonRecord>>(File.ReadAllText(sc5FilePacksJson), jsonOptions);
				if (sc5FilePacks != null)
				{
					db.AddRange(sc5FilePacks.Select(x => new TblSC5FilePack
					{
						Name = x.Name,
						Description = x.Description,
						Authors = [.. db.Authors.Where(a => x.Authors.Contains(a.Name))],
						Tags = [.. db.Tags.Where(a => x.Tags.Contains(a.Name))],
						Licence = x.Licence == null ? null : db.Licences.Single(l => l.Name == x.Licence),
						CreatedDate = x.CreatedDate,
						ModifiedDate = x.ModifiedDate,
						UploadedDate = x.UploadedDate,
					}));
					_ = db.SaveChanges();
				}
			}

			if (!db.SC5Files.Any() && File.Exists(sc5FilesJson))
			{
				log("Seeding SC5Files");
				var sc5Files = JsonSerializer.Deserialize<IEnumerable<SC5FileJsonRecord>>(File.ReadAllText(sc5FilesJson), jsonOptions);
				if (sc5Files != null)
				{
					var sC5FilePacks = db.SC5FilePacks?.Where(x => x.SC5Files.Select(fp => fp.Name).Contains(x.Name)).ToList();
					db.AddRange(sc5Files.Select(x => new TblSC5File
					{
						Name = x.Name,
						Description = x.Description,
						Authors = [.. db.Authors.Where(a => x.Authors.Contains(a.Name))],
						SC5FilePacks = sC5FilePacks ?? [],
					}));
					_ = db.SaveChanges();
				}
			}

			if (!db.ObjectPacks.Any() && File.Exists(objectPacksJson))
			{
				log("Seeding ObjectPacks");
				var objectPacks = JsonSerializer.Deserialize<IEnumerable<ObjectPackJsonRecord>>(File.ReadAllText(objectPacksJson), jsonOptions);
				if (objectPacks != null)
				{
					db.AddRange(objectPacks.Select(x => new TblObjectPack
					{
						Name = x.Name,
						Description = x.Description,
						Authors = [.. db.Authors.Where(a => x.Authors.Contains(a.Name))],
						Tags = [.. db.Tags.Where(a => x.Tags.Contains(a.Name))],
						Licence = x.Licence == null ? null : db.Licences.Single(l => l.Name == x.Licence),
						CreatedDate = null,
						ModifiedDate = null,
						UploadedDate = DateOnly.UtcToday,
					}));
					_ = db.SaveChanges();
				}
			}

			if (!db.Objects.Any() && File.Exists(objectMetadataJson) && Directory.Exists(settings.ObjectDirectory))
			{
				log("Seeding Objects");
				var scan = Dat.Services.DatFolderScanner.ScanDirectory(settings.ObjectDirectory, logger);
				var objectMetadata = JsonSerializer.Deserialize<IEnumerable<ObjectMetadata>>(File.ReadAllText(objectMetadataJson), jsonOptions);
				var objectMetadataDict = objectMetadata!.ToDictionary(x => x.InternalName, x => x);
				var gameReleaseDate = new DateOnly(2004, 09, 07);

				foreach (var objIndex in scan.Succeeded.DistinctBy(x => (x.DatName, x.DatChecksum)))
				{
					if (!objectMetadataDict.TryGetValue(objIndex.DatName, out var meta))
					{
						meta = new ObjectMetadata(Guid.NewGuid().ToString(), null, [], [], [], null, ObjectAvailability.Available, DateOnly.UtcToday, null, DateOnly.UtcToday, ObjectSource.Custom);
						objectMetadataDict.Add(objIndex.DatName, meta);
					}

					var filename = objIndex.FullPath;
					var creationTime = objIndex.ObjectSource is ObjectSource.LocomotionSteam or ObjectSource.LocomotionGoG
						? gameReleaseDate
						: DateOnly.FromDateTime(File.GetLastWriteTimeUtc(filename));

					var authors = meta.Authors == null ? null : db.Authors.Where(x => meta.Authors.Contains(x.Name)).ToList();
					var tags = meta.Tags == null ? null : db.Tags.Where(x => meta.Tags.Contains(x.Name)).ToList();
					var objectPacks = meta.ObjectPacks == null ? null : db.ObjectPacks.Where(x => meta.ObjectPacks.Contains(x.Name)).ToList();
					var licence = meta.Licence == null ? null : db.Licences.Where(x => x.Name == meta.Licence).First();

					var tblLocoObject = new TblObject
					{
						Name = meta.InternalName,
						ObjectSource = objIndex.ObjectSource,
						ObjectType = objIndex.ObjectType,
						VehicleType = objIndex.VehicleType,
						Description = meta?.Description,
						Authors = authors ?? [],
						CreatedDate = creationTime,
						ModifiedDate = null,
						UploadedDate = DateOnly.UtcToday,
						Tags = tags ?? [],
						ObjectPacks = objectPacks ?? [],
						DatObjects = [],
						Licence = licence,
					};

					var addedObj = db.Add(tblLocoObject);
					var locoLookupTbl = new TblDatObject
					{
						DatName = objIndex.DatName,
						DatChecksum = objIndex.DatChecksum,
						xxHash3 = objIndex.xxHash3,
						FileName = objIndex.RelativePath,
						ObjectId = addedObj.Entity.Id,
						Object = tblLocoObject,
					};
					_ = db.DatObjects.Add(locoLookupTbl);
					tblLocoObject.DatObjects.Add(locoLookupTbl);
				}

				_ = db.SaveChanges();
			}

			log("Finished seeding");
		});
}
