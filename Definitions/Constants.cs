namespace Definitions;

public static class Constants
{
	public const string ApplicationName = "OpenLoco Object Editor";
	public const string IndexFileName = "objectIndex.json";
	public const string LoggingFileName = "objectEditor.log";

	const string EditorSettingsFileName = "settings.json"; // "settings-dev.json" for dev, "settings.json" for prod
	const string EnvSettingsFile = "ENV_SETTINGS_FILE"; // Environment variable to override the settings file name
	const string DownloadFolderName = "downloads";

	public static string ProgramDataPath => Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), ApplicationName);
	public static string EditorSettingsFile => Path.Combine(ProgramDataPath, Environment.GetEnvironmentVariable(EnvSettingsFile) ?? EditorSettingsFileName);
	public static string IndexFile => Path.Combine(ProgramDataPath, IndexFileName);
	public static string LoggingFile => Path.Combine(ProgramDataPath, LoggingFileName);
	public static string DefaultDownloadsFolder => Path.Combine(ProgramDataPath, DownloadFolderName);
}
