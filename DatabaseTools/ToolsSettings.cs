using Common.Json;
using Definitions.Database;
using ReactiveUI;
using ReactiveUI.Fody.Helpers;
using System.Text.Json;

namespace DatabaseTools;

public class ToolsSettings : ReactiveObject
{
	[Reactive] public string DatabaseFile { get; set; } = BaseLocoDbContext.DefaultDb;

	[Reactive] public string ObjectDirectory { get; set; } = "Q:\\Games\\Locomotion\\Server\\Objects";

	[Reactive] public string JsonDirectory { get; set; } = "Q:\\Games\\Locomotion\\Database";

	[Reactive] public bool DeleteExistingOnImport { get; set; } = true;

	public static string DefaultSettingsPath { get; } = Path.Combine(
		Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData),
		"OpenLoco",
		"DatabaseTools",
		"settings.json");

	public static ToolsSettings Load(string filename)
	{
		if (!File.Exists(filename))
		{
			var fresh = new ToolsSettings();
			fresh.Save(filename);
			return fresh;
		}

		try
		{
			var text = File.ReadAllText(filename);
			var loaded = JsonSerializer.Deserialize<ToolsSettings>(text, JsonFile.DefaultSerializerOptions);
			if (loaded != null)
			{
				return loaded;
			}
		}
		catch (Exception ex) when (ex is JsonException or IOException or UnauthorizedAccessException)
		{
			// fall through and back up + recreate
		}

		try
		{
			var backup = $"{filename}.bad-{DateTime.UtcNow:yyyyMMddHHmmss}";
			File.Move(filename, backup, overwrite: true);
		}
		catch
		{
			// best-effort backup
		}

		var defaults = new ToolsSettings();
		defaults.Save(filename);
		return defaults;
	}

	public void Save(string filename)
	{
		var parentDir = Path.GetDirectoryName(filename);
		if (!string.IsNullOrEmpty(parentDir) && !Directory.Exists(parentDir))
		{
			_ = Directory.CreateDirectory(parentDir);
		}

		var text = JsonSerializer.Serialize(this, JsonFile.DefaultSerializerOptions);
		File.WriteAllText(filename, text);
	}
}
