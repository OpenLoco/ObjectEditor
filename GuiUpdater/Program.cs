using Common;
using NuGet.Versioning;
using System.CommandLine;
using System.Diagnostics;
using System.Formats.Tar;
using System.IO.Compression;
using System.Runtime.InteropServices;

var pidOption = new Option<int>("--pid") { Description = "The process ID of the main application to wait for it to exit." };
var appPathOption = new Option<string>("--app-path") { Description = "The path to the main application executable to restart." };
var currentVersionOption = new Option<string>("--current-version") { Description = "The current version of the application in SemVer format." };

var rootCommand = new RootCommand("GUI Updater for OpenLoco Object Editor")
{
	pidOption,
	appPathOption,
	currentVersionOption,
};

// taken from ObjectEditorModel.cs
const string ApplicationName = "OpenLoco Object Editor";
const string LoggingFileName = "objectEditorUpdater.log";

rootCommand.SetAction(UpdateEditor);
return await rootCommand.Parse(args).InvokeAsync();

async Task UpdateEditor(ParseResult parseResult, CancellationToken cancellationToken)
{
	var programDataPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), ApplicationName);
	var loggingFile = Path.Combine(programDataPath, LoggingFileName);
_ = Directory.CreateDirectory(programDataPath);
	// FileMode.Create truncates the log; File.OpenWrite would leave stale trailing text from a
	// previous, longer run appended after the new (shorter) contents.
	using var logStream = new StreamWriter(File.Create(loggingFile));
	Console.SetOut(logStream);
	Console.SetError(logStream);

	Console.WriteLine($"=== OpenLoco Object Editor Updater started ===");
	Console.WriteLine($"Logs can be found in {loggingFile}");

	// parsing arguments
	var pid = parseResult.GetValue(pidOption);
	var appPath = parseResult.GetValue(appPathOption);
	var currentVersionString = parseResult.GetValue(currentVersionOption);

	if (string.IsNullOrEmpty(appPath))
	{
		Console.WriteLine("Current editor .exe path is empty; using current directory.");
		var appDir = Directory.GetCurrentDirectory();
		var appName = PlatformSpecific.EditorPlatformBinaryName(PlatformSpecific.GetPlatform);
		appPath = Path.Combine(appDir, appName);
		Console.WriteLine($"Current directory is \"{appPath}\"");
	}

	if (string.IsNullOrEmpty(currentVersionString))
	{
		Console.WriteLine("Current version is empty");
	}

// latest version
	var latestVersion = await GetLatestVersionAsync(cancellationToken);

	if (latestVersion == null)
	{
		Console.Error.WriteLine("Unable to determine the latest version. Cannot automatically update.");
		return;
	}

	// current version
	if (currentVersionString == null || !SemanticVersion.TryParse(currentVersionString, out var currentVersion) || currentVersion == null || currentVersion == VersionHelpers.UnknownVersion)
	{
		Console.WriteLine("Unable to parse current version - will attempt to download the latest version");
		currentVersion = null;
	}
	else
	{
		Console.WriteLine($"Current version: {currentVersion}");
	}

	// compare versions
	if (currentVersion != null && latestVersion <= currentVersion)
	{
		Console.WriteLine("Current version is the latest version - no update needed");
		return;
	}

	// kill existing editor processes
	if (pid > 0)
	{
		try
		{
			var mainProcess = Process.GetProcessById(pid);
			Console.WriteLine($"Waiting for main application (PID: {pid}) to exit...");
			await mainProcess.WaitForExitAsync(cancellationToken);
			Console.WriteLine("Main application has exited.");
		}
		catch (ArgumentException)
		{
			// Process with an invalid PID or one that has already exited.
			Console.WriteLine($"Process with PID {pid} not found. It might have already exited (which is a good thing)");
		}
	}

	// actually download
	var platform = PlatformSpecific.GetPlatform;
	var url = VersionHelpers.UrlForDownload(latestVersion, platform);
	var filename = Path.GetFileName(url);
	var tempArchivePath = Path.Combine(Path.GetTempPath(), filename);
	var extractionPath = GetInstallRoot(appPath, platform);

	if (string.IsNullOrEmpty(extractionPath))
	{
		Console.WriteLine("Invalid application path");
		return;
	}

	try
	{
		Console.WriteLine($"Downloading update from {url}...");
		using (var httpClient = new HttpClient())
		{
			var response = await httpClient.GetAsync(url, cancellationToken);
			_ = response.EnsureSuccessStatusCode();
			await using (var fs = new FileStream(tempArchivePath, FileMode.Create, FileAccess.Write, FileShare.None))
			{
				await response.Content.CopyToAsync(fs, cancellationToken);
			}
		}

		Console.WriteLine($"Update downloaded to {tempArchivePath}.");

		Console.WriteLine($"Extracting update to {extractionPath}...");
		ExtractArchive(tempArchivePath, extractionPath);
		RestoreExecutablePermissions(extractionPath, platform);
		Console.WriteLine("Extraction complete");
	}
	catch (Exception ex)
	{
		Console.WriteLine($"An error occurred during the update process: {ex.Message}");
		// Optionally, restart the original app even if update fails
	}
	finally
	{
		if (File.Exists(tempArchivePath))
		{
			File.Delete(tempArchivePath);
		}
	}

	Console.WriteLine($"Restarting editor at: {appPath}");
	if (platform == OSPlatform.OSX)
	{
		// relaunch through `open` so macOS boots the bundle properly rather than the raw inner binary
		var appBundlePath = GetMacAppBundlePath(appPath) ?? appPath;
		_ = Process.Start(new ProcessStartInfo("open", ["-n", appBundlePath]) { UseShellExecute = false, CreateNoWindow = true, });
	}
	else
	{
		_ = Process.Start(new ProcessStartInfo(appPath) { UseShellExecute = false, CreateNoWindow = true, });
	}
}

// the directory the archive should be extracted into, ie the folder that holds the app/bundle
static string? GetInstallRoot(string appPath, OSPlatform platform)
{
	if (platform == OSPlatform.OSX)
	{
		// appPath is <root>/ObjectEditor.app/Contents/MacOS/ObjectEditor, and the archive contains
		// ObjectEditor.app/..., so extract into <root>
		var bundlePath = GetMacAppBundlePath(appPath);
		return bundlePath == null ? null : Path.GetDirectoryName(bundlePath);
	}

	return Path.GetDirectoryName(appPath);
}

// eg /Applications/ObjectEditor.app/Contents/MacOS/ObjectEditor -> /Applications/ObjectEditor.app
static string? GetMacAppBundlePath(string appPath)
{
	var directory = new DirectoryInfo(Path.GetDirectoryName(appPath) ?? ".");
	// walk up looking for the .app directory
	for (var i = 0; i < 3 && directory != null; i++)
	{
		if (directory.Extension == ".app")
		{
			return directory.FullName;
		}

		directory = directory.Parent;
	}

	return null;
}

static void ExtractArchive(string archivePath, string destination)
{
	if (archivePath.EndsWith(".zip", StringComparison.OrdinalIgnoreCase))
	{
		ZipFile.ExtractToDirectory(archivePath, destination, true);
	}
	else if (archivePath.EndsWith(".tar.gz", StringComparison.OrdinalIgnoreCase) || archivePath.EndsWith(".tgz", StringComparison.OrdinalIgnoreCase))
	{
		using var fileStream = File.OpenRead(archivePath);
		using var gzipStream = new GZipStream(fileStream, CompressionMode.Decompress);
		TarFile.ExtractToDirectory(gzipStream, destination, true);
	}
	else
	{
		throw new NotSupportedException($"Unsupported archive type: \"{Path.GetFileName(archivePath)}\"");
	}
}

// zip archives do not carry unix file modes, so the extracted binaries need their exec bit setting
static void RestoreExecutablePermissions(string installRoot, OSPlatform platform)
{
	if (OperatingSystem.IsWindows())
	{
		return;
	}

	const UnixFileMode executableMode = UnixFileMode.UserRead | UnixFileMode.UserWrite | UnixFileMode.UserExecute
		| UnixFileMode.GroupRead | UnixFileMode.GroupExecute
		| UnixFileMode.OtherRead | UnixFileMode.OtherExecute;

	var executablePaths = platform == OSPlatform.OSX
		? new[]
		{
			Path.Combine(installRoot, "ObjectEditor.app", "Contents", "MacOS", VersionHelpers.ObjectEditorName),
			Path.Combine(installRoot, "ObjectEditor.app", "Contents", "MacOS", VersionHelpers.ObjectEditorUpdaterName),
		}
		: new[]
		{
			Path.Combine(installRoot, VersionHelpers.ObjectEditorName),
			Path.Combine(installRoot, VersionHelpers.ObjectEditorUpdaterName),
		};

	foreach (var path in executablePaths)
	{
		if (File.Exists(path))
		{
			File.SetUnixFileMode(path, executableMode);
			Console.WriteLine($"Restored executable permissions on \"{path}\"");
		}
	}
}

static async Task<SemanticVersion?> GetLatestVersionAsync(CancellationToken cancellationToken)
{
	try
	{
		Console.WriteLine("Checking for the latest version...");
		var latestVersion = await VersionHelpers.GetLatestAppVersionAsync(VersionHelpers.ObjectEditorName, cancellationToken: cancellationToken);
		Console.WriteLine($"Latest version available: {latestVersion}");
		return latestVersion;
	}
	catch (Exception ex)
	{
		Console.Error.WriteLine($"An error occurred while checking for the latest version: {ex.Message}");
		return null;
	}
}
