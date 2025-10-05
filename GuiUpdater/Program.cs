using System.CommandLine;
using System.Diagnostics;
using System.IO.Compression;

var pidOption = new Option<int>("--pid") { Description = "The process ID of the main application to wait for it to exit." };
var urlOption = new Option<string>("--url") { Description = "The download URL of the update package." };
var appPathOption = new Option<string>("--app-path") { Description = "The path to the main application executable to restart." };

var rootCommand = new RootCommand("GUI Updater for OpenLoco Object Editor")
{
	pidOption,
	urlOption,
	appPathOption
};

rootCommand.SetAction(async (ParseResult parseResult, CancellationToken cancellationToken) =>
{
	var pid = parseResult.GetValue(pidOption);
	var url = new Uri(parseResult.GetValue(urlOption));
	var appPath = parseResult.GetValue(appPathOption);

	Console.WriteLine("OpenLoco Object Editor Updater started.");

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
			Console.WriteLine($"Process with PID {pid} not found. It might have already exited (which is a good thing).");
		}
	}

	if (url == null || string.IsNullOrEmpty(appPath))
	{
		Console.WriteLine("URL and App Path are required.");
		return;
	}

	// url is something like "https://github.com/OpenLoco/ObjectEditor/releases/download/5.3.5/object-editor-5.3.5-win-x64.zip"
	var filename = Path.GetFileName(url.ToString());
	var tempZipPath = Path.Combine(Path.GetTempPath(), filename);
	var extractionPath = Path.GetDirectoryName(appPath);

	if (string.IsNullOrEmpty(extractionPath))
	{
		Console.WriteLine("Invalid application path.");
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
		Console.WriteLine("Extraction complete.");
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

	Console.WriteLine($"Restarting application: {appPath}");
	_ = Process.Start(new ProcessStartInfo(appPath) { UseShellExecute = false, CreateNoWindow = true, });

});

return await rootCommand.Parse(args).InvokeAsync();
