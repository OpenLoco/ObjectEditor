using Common;
using Common.Json;
using Definitions.ObjectModels.Graphics;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text.Json;

namespace Gui;

public class EditorSettings
{
	public const string ApplicationName = "OpenLoco Object Editor";
	public const string SettingsFileName = "settings.json"; // "settings-dev.json" for dev, "settings.json" for prod
	public const string LoggingFileName = "objectEditor.log";
	public const string DefaultImageTableGroupsFileName = "imageTableGroups.json";

	// stores settings.json, objectEditor.log, etc
	public static string ProgramDataPath => Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), ApplicationName);
	public static string SettingsFilePathName => Path.Combine(ProgramDataPath, Environment.GetEnvironmentVariable("ENV_SETTINGS_FILE") ?? SettingsFileName);
	public static string LoggingFilePathName => Path.Combine(ProgramDataPath, LoggingFileName);

	//public string ImageTableGroupsPathName =>
	//	ImageTableGroupConfigFile ?? Path.Combine(ConfigFolder, DefaultImageTableGroupsFileName);

	public string ObjDataDirectory
	{
		get => field ??= string.Empty;
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
	public const string DefaultCacheFolder  = "cache";
	public const string DefaultConfigFolder  = "config";
	public const string DefaultDownloadFolder = "downloads";
	public const string DefaultObjectIndicesFolder = "objectIndices";

	public string CacheFolder { get; set; } = DefaultCacheFolder;
	public string ConfigFolder { get; set; } = DefaultConfigFolder;
	public string DownloadFolder { get; set; } = DefaultDownloadFolder;
	public string ObjectIndicesFolder { get; set; } = DefaultObjectIndicesFolder;

	public string ImageTableGroupsConfigFile { get; set; } = string.Empty;
	public static string DefaultImageTableGroupsConfigFile => Path.Combine(ProgramDataPath, DefaultConfigFolder, DefaultImageTableGroupsFileName);

	public string LocomotionSteamObjDataFolder { get; set; } = string.Empty;
	public string LocomotionGoGObjDataFolder { get; set; } = string.Empty;
	public string AppDataObjDataFolder { get; set; } = string.Empty;
	public string OpenLocoObjDataFolder { get; set; } = string.Empty;

	public EditorSettings()
	{
		ImageTableGroupsConfigFile = DefaultImageTableGroupsConfigFile;
	}

	public string GetGameObjDataFolder(GameObjDataFolder folder)
		=> folder switch
		{
			GameObjDataFolder.AppData => AppDataObjDataFolder,
			GameObjDataFolder.LocomotionSteam => LocomotionSteamObjDataFolder,
			GameObjDataFolder.LocomotionGoG => LocomotionGoGObjDataFolder,
			GameObjDataFolder.OpenLoco => OpenLocoObjDataFolder,
			_ => throw new NotImplementedException(),
		};

	public static EditorSettings Load(string filename, ILogger logger)
	{
		var settings = LoadCore(filename, logger);
		ArgumentNullException.ThrowIfNull(settings);

		if (settings.Validate(logger))
		{
			logger.LogInformation("Settings loaded and validated successfully.");
		}
		else
		{
			logger.LogError("Unable to validate settings file - please delete it and it will be recreated on next editor start-up.");
		}

		settings.PostSetup(logger);

		return settings;
	}

	static EditorSettings LoadCore(string filename, ILogger logger)
	{
		if (!File.Exists(filename))
		{
			logger.LogInformation("Settings file doesn't exist; creating now at \"{Filename}\"", filename);
			var newSettings = new EditorSettings();
			Save(newSettings, filename, logger);
			return newSettings;
		}

		try
		{
			var text = File.ReadAllText(filename);
			var settings = JsonSerializer.Deserialize<EditorSettings>(text, options: JsonFile.DefaultSerializerOptions);
			if (settings != null)
			{
				return settings;
			}

			logger.LogWarning("Settings file at \"{Filename}\" deserialized to null; backing up and recreating with defaults.", filename);
		}
		catch (Exception ex) when (ex is JsonException or IOException or UnauthorizedAccessException)
		{
			logger.LogError(ex, "Failed to load settings from \"{Filename}\"; backing up and recreating with defaults.", filename);
		}

		// Move the bad file aside so we don't silently destroy whatever the user had.
		try
		{
			var backup = $"{filename}.bad-{DateTime.UtcNow:yyyyMMddHHmmss}";
			File.Move(filename, backup, overwrite: true);
			logger.LogInformation("Backed up invalid settings file to \"{Backup}\"", backup);
		}
		catch (Exception backupEx)
		{
			logger.LogError(backupEx, "Failed to back up invalid settings file \"{Filename}\"; it will be overwritten.", filename);
		}

		var defaults = new EditorSettings();
		Save(defaults, filename, logger);
		return defaults;
	}

	void PostSetup(ILogger logger)
	{
		ObjectIndicesFolder = InitialiseDirectory(ObjectIndicesFolder, DefaultObjectIndicesFolder, logger);
		CacheFolder = InitialiseDirectory(CacheFolder, DefaultCacheFolder, logger);
		DownloadFolder = InitialiseDirectory(DownloadFolder, DefaultDownloadFolder, logger);
		ConfigFolder = InitialiseDirectory(ConfigFolder, DefaultConfigFolder, logger);
		ImageTableGrouper.InitialiseImageTableGroupsConfigFile(VersionHelpers.GetCurrentAppVersion(), ImageTableGroupsConfigFile, DefaultImageTableGroupsConfigFile, logger);
	}

	static string InitialiseDirectory(string folder, string defaultName, ILogger logger)
	{
		if (string.IsNullOrEmpty(folder))
		{
			folder = Path.Combine(ProgramDataPath, defaultName);
		}

		if (!Directory.Exists(folder))
		{
			logger.LogInformation("\"{DefaultName}\" folder doesn't exist; creating now at \"{Folder}\"", defaultName, folder);
			_ = Directory.CreateDirectory(folder);
		}

		return folder;
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
			logger.LogError(ex, "Failed to save settings to \"{Filename}\"", filename);
		}
	}

	public bool Validate(ILogger logger)
	{
		if (string.IsNullOrEmpty(ObjDataDirectory))
		{
			logger.LogWarning("Invalid settings file: Object directory was null or empty");
			return false;
		}

		if (!Directory.Exists(ObjDataDirectory))
		{
			logger.LogWarning("Invalid settings file: ObjData folder \"{ObjDataDirectory}\" does not exist", ObjDataDirectory);
			return false;
		}

		if (!string.IsNullOrEmpty(ConfigFolder) && !Directory.Exists(ConfigFolder))
		{
			logger.LogWarning("Invalid settings file: Config folder \"{ConfigFolder}\" does not exist", ConfigFolder);
			return false;
		}

		if (!string.IsNullOrEmpty(CacheFolder) && !Directory.Exists(CacheFolder))
		{
			logger.LogWarning("Invalid settings file: Cache folder \"{CacheFolder}\" does not exist", CacheFolder);
			return false;
		}

		if (!string.IsNullOrEmpty(DownloadFolder) && !Directory.Exists(DownloadFolder))
		{
			logger.LogWarning("Invalid settings file: Download folder \"{DownloadFolder}\" does not exist", DownloadFolder);
			return false;
		}

		if (!string.IsNullOrEmpty(ObjectIndicesFolder) && !Directory.Exists(ObjectIndicesFolder))
		{
			logger.LogWarning("Invalid settings file: Object index folder \"{ObjectIndicesFolder}\" does not exist", ObjectIndicesFolder);
			return false;
		}

		if (!string.IsNullOrEmpty(ImageTableGroupsConfigFile) && !File.Exists(ImageTableGroupsConfigFile))
		{
			logger.LogWarning("Invalid settings file: Image table group config file \"{ImageTableGroupConfigFile}\" does not exist", ImageTableGroupsConfigFile);
			return false;
		}

		return true;
	}
}
