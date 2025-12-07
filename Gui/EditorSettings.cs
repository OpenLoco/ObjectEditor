using Common.Json;
using Common.Logging;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace Gui;

public class EditorSettings
{
	public string ObjDataDirectory
	{
		get;
		set
		{
			field = value;
			ObjDataDirectories ??= [];
			_ = ObjDataDirectories.Add(field);
		}
	}

	public HashSet<string> ObjDataDirectories
	{
		get => field ??= [];
		set;
	} = [];

	public bool AllowSavingAsVanillaObject { get; set; }
	public bool EnableOGValidation { get; set; }
	public bool ShowLogsOnError { get; set; }
	public bool AutoObjectDiscoveryAndUpload { get; set; }

	public bool UseHttps { get; set; }
	public string ServerAddressHttp { get; set; } = "http://openloco.leftofzen.dev/";
	public string ServerAddressHttps { get; set; } = "https://openloco.leftofzen.dev/";

	//public string ServerEmail { get; set; }
	//public string ServerPassword { get; set; }

	public string ObjectIndicesFolder { get; set; } = string.Empty;
	public string DownloadFolder { get; set; } = string.Empty;

	public string AppDataObjDataFolder { get; set; } = string.Empty;
	public string LocomotionObjDataFolder { get; set; } = string.Empty;
	public string OpenLocoObjDataFolder { get; set; } = string.Empty;

	public string GetGameObjDataFolder(GameObjDataFolder folder)
		=> folder switch
		{
			GameObjDataFolder.AppData => AppDataObjDataFolder,
			GameObjDataFolder.Locomotion => LocomotionObjDataFolder,
			GameObjDataFolder.OpenLoco => OpenLocoObjDataFolder,
			_ => throw new NotImplementedException(),
		};

	[JsonIgnore]
	public string IndexFileName
	{
		get
		{
			var filename = Convert.ToBase64String(Encoding.UTF8.GetBytes(ObjDataDirectory));
			return Path.Combine(ObjectIndicesFolder, $"{filename}.json");
		}
	}

	[JsonIgnore]
	public const string DefaultFileName = "settings.json"; // "settings-dev.json" for dev, "settings.json" for prod

	public static EditorSettings Load(string filename, ILogger logger)
	{
		if (!File.Exists(filename))
		{
			logger.Info($"Settings file doesn't exist; creating now at \"{filename}\"");
			var newSettings = new EditorSettings();
			Save(newSettings, filename, logger);
			return newSettings;
		}

		var text = File.ReadAllText(filename);
		var settings = JsonSerializer.Deserialize<EditorSettings>(text, options: JsonFile.DefaultSerializerOptions); // todo: try-catch this for invalid settings files
		ArgumentNullException.ThrowIfNull(settings);
		return settings;
	}

	public void Save(string filename, ILogger logger)
		=> Save(this, filename, logger);

	static void Save(EditorSettings settings, string filename, ILogger logger)
	{
		var text = JsonSerializer.Serialize(settings, options: JsonFile.DefaultSerializerOptions);

		var parentDir = Path.GetDirectoryName(filename);
		if (parentDir != null && !Directory.Exists(parentDir))
		{
			_ = Directory.CreateDirectory(parentDir);
		}

		try
		{
			File.WriteAllText(filename, text);
		}
		catch (Exception ex)
		{
			logger.Error(ex);
		}
	}

	public bool Validate(ILogger logger)
	{
		if (string.IsNullOrEmpty(ObjDataDirectory))
		{
			logger.Warning("Invalid settings file: Object directory was null or empty");
			return false;
		}

		if (!Directory.Exists(ObjDataDirectory))
		{
			logger.Warning($"Invalid settings file: Directory \"{ObjDataDirectory}\" does not exist");
			return false;
		}

		if (!Directory.Exists(DownloadFolder))
		{
			logger.Warning($"Invalid settings file: Directory \"{DownloadFolder}\" does not exist");
			return false;
		}

		if (!Directory.Exists(ObjectIndicesFolder))
		{
			logger.Warning($"Invalid settings file: Directory \"{ObjectIndicesFolder}\" does not exist");
			return false;
		}

		return true;
	}
}
