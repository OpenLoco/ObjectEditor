using Common.Json;
using Definitions.Database;
using Definitions.SourceData;
using Microsoft.EntityFrameworkCore;
using System.Text.Json;

namespace DatabaseTools.Services;

public static class DatabaseExportService
{
	public static Task ExportAllAsync(ToolsSettings settings, Action<string> log)
		=> Task.Run(() =>
		{
			using var db = LocoDbContext.GetDbFromFile(settings.DatabaseFile)
				?? throw new InvalidOperationException($"Database not found at {settings.DatabaseFile}");

			log("Loading from database...");
			_ = Directory.CreateDirectory(settings.JsonDirectory);

			var jsonOptions = JsonFile.DefaultSerializerOptions;

			var authors = JsonSerializer.Serialize<IEnumerable<string>>(
				db.Authors.Select(a => a.Name).ToList().Order(),
				jsonOptions);
			var tags = JsonSerializer.Serialize<IEnumerable<string>>(
				db.Tags.Select(t => t.Name).ToList().Order(),
				jsonOptions);
			var licences = JsonSerializer.Serialize<IEnumerable<LicenceJsonRecord>>(
				db.Licences.Select(l => new LicenceJsonRecord(l.Name, l.Text)).ToList().OrderBy(l => l.Name),
				jsonOptions);

			var scenarios = db.Scenarios
				.Include(l => l.Licence)
				.Select(x => new ExpandedTbl<TblScenario, TblScenarioPack>(x, x.Authors, x.Tags, x.ScenarioPacks))
				.ToList()
				.OrderBy(x => x.Object.Name)
				.Select(o => new ScenarioJsonRecord(
					o.Object.Name,
					o.Object.Description,
					[.. o.Authors.Select(a => a.Name)],
					[.. o.Tags.Select(t => t.Name)],
					o.Object.Licence?.Name,
					o.Object.CreatedDate,
					o.Object.ModifiedDate,
					o.Object.UploadedDate,
					o.Object.ObjectSource));
			var scenariosJson = JsonSerializer.Serialize(scenarios, jsonOptions);

			var scenarioPacks = db.ScenarioPacks
				.Include(l => l.Licence)
				.Select(x => new ExpandedTblPack<TblScenarioPack, TblScenario>(x, x.Scenarios, x.Authors, x.Tags))
				.ToList()
				.OrderBy(x => x.Pack.Name)
				.Select(o => new ScenarioPackJsonRecord(
					o.Pack.Name,
					o.Pack.Description,
					[.. o.Authors.Select(a => a.Name)],
					[.. o.Tags.Select(t => t.Name)],
					o.Pack.Licence?.Name,
					o.Pack.CreatedDate,
					o.Pack.ModifiedDate,
					o.Pack.UploadedDate));
			var scenarioPacksJson = JsonSerializer.Serialize(scenarioPacks, jsonOptions);

			var objectPacks = db.ObjectPacks
				.Include(l => l.Licence)
				.Select(x => new ExpandedTblPack<TblObjectPack, TblObject>(x, x.Objects, x.Authors, x.Tags))
				.ToList()
				.OrderBy(x => x.Pack.Name)
				.Select(o => new ObjectPackJsonRecord(
					o.Pack.Name,
					o.Pack.Description,
					[.. o.Authors.Select(a => a.Name)],
					[.. o.Tags.Select(t => t.Name)],
					o.Pack.Licence?.Name,
					o.Pack.CreatedDate,
					o.Pack.ModifiedDate,
					o.Pack.UploadedDate));
			var objectPacksJson = JsonSerializer.Serialize(objectPacks, jsonOptions);

			var objects = db.DatObjects
				.Include(x => x.Object)
				.Include(x => x.Object.Licence)
				.Select(x => new ExpandedTbl<TblObject, TblObjectPack>(x.Object, x.Object.Authors, x.Object.Tags, x.Object.ObjectPacks))
				.ToList()
				.OrderBy(x => x.Object.Name)
				.Select(o => new ObjectMetadata(
					o.Object.Name,
					o.Object.Description,
					[.. o.Authors.Select(a => a.Name)],
					[.. o.Tags.Select(t => t.Name)],
					[.. o.Packs.Select(m => m.Name)],
					o.Object.Licence?.Name,
					o.Object.Availability,
					o.Object.CreatedDate,
					o.Object.ModifiedDate,
					o.Object.UploadedDate,
					o.Object.ObjectSource));
			var objectsJson = JsonSerializer.Serialize(objects, jsonOptions);

			log("Writing JSON files…");
			File.WriteAllText(Path.Combine(settings.JsonDirectory, "authors.json"), authors);
			File.WriteAllText(Path.Combine(settings.JsonDirectory, "tags.json"), tags);
			File.WriteAllText(Path.Combine(settings.JsonDirectory, "licences.json"), licences);
			File.WriteAllText(Path.Combine(settings.JsonDirectory, "objectPacks.json"), objectPacksJson);
			File.WriteAllText(Path.Combine(settings.JsonDirectory, "objectMetadata.json"), objectsJson);
			File.WriteAllText(Path.Combine(settings.JsonDirectory, "scenarios.json"), scenariosJson);
			File.WriteAllText(Path.Combine(settings.JsonDirectory, "scenarioPacks.json"), scenarioPacksJson);
			log($"Done. Wrote 7 files to {settings.JsonDirectory}");
		});
}
