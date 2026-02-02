using Common;
using NuGet.Versioning;
using System.CommandLine;
using System.Diagnostics;
using System.IO.Compression;

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
	using var logStream = new StreamWriter(File.OpenWrite(loggingFile));
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

	// current version
	if (SemanticVersion.TryParse(currentVersionString, out var currentVersion) || currentVersion == null || currentVersion == VersionHelpers.UnknownVersion)
	{
		Console.WriteLine($"Current version: {currentVersion}");
	}
	else
	{
		Console.WriteLine("Unable to parse current version - will attempt to download the latest version");
	}

	// latest version
	SemanticVersion? latestVersion = null;
	try
	{
		Console.WriteLine("Checking for the latest version...");
		latestVersion = VersionHelpers.GetLatestAppVersion(VersionHelpers.ObjectEditorName);
		Console.WriteLine($"Latest version available: {latestVersion}");
	}
	catch (Exception ex)
	{
		Console.Error.WriteLine($"An error occurred while checking for the latest version: {ex.Message}");
		return;
	}

	if (latestVersion == null)
	{
		Console.Error.WriteLine("Unable to determine the latest version");
		return;
	}

	// compare versions
	if (latestVersion <= currentVersion)
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
	var url = VersionHelpers.UrlForDownload(latestVersion, PlatformSpecific.GetPlatform);
	var filename = Path.GetFileName(url);
	var tempZipPath = Path.Combine(Path.GetTempPath(), filename);
	var extractionPath = Path.GetDirectoryName(appPath);

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
			await using (var fs = new FileStream(tempZipPath, FileMode.Create, FileAccess.Write, FileShare.None))
			{
				await response.Content.CopyToAsync(fs, cancellationToken);
			}
		}

		Console.WriteLine($"Update downloaded to {tempZipPath}.");

		Console.WriteLine($"Extracting update to {extractionPath}...");
		ZipFile.ExtractToDirectory(tempZipPath, extractionPath, true);
		Console.WriteLine("Extraction complete");
	}
	catch (Exception ex)
	{
		Console.WriteLine($"An error occurred during the update process: {ex.Message}");
		// Optionally, restart the original app even if update fails
	}
	finally
	{
		if (File.Exists(tempZipPath))
		{
			File.Delete(tempZipPath);
		}
	}

	Console.WriteLine($"Restarting editor at: {appPath}");
	_ = Process.Start(new ProcessStartInfo(appPath) { UseShellExecute = false, CreateNoWindow = true, });
}
