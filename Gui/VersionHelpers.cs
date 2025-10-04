//#if !DEBUG
using Common;
using Common.Logging;
using NuGet.Versioning;
using System;
using System.Diagnostics;
using System.IO;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Reflection;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
//#endif

namespace Gui;

public static class VersionHelpers
{
	public const string GithubApplicationName = "ObjectEditor";
	public const string GithubIssuePage = "https://github.com/OpenLoco/ObjectEditor/issues";
	public const string GithubLatestReleaseDownloadPage = "https://github.com/OpenLoco/ObjectEditor/releases";
	public const string GithubLatestReleaseAPI = "https://api.github.com/repos/OpenLoco/ObjectEditor/releases/latest";

	// todo: instead of going to downloads, start the auto-updater (GuiUpdater.exe) with the right args
	public static Process? OpenDownloadPage()
		=> Process.Start(new ProcessStartInfo(GithubLatestReleaseDownloadPage) { UseShellExecute = true });

	public static async Task StartGuiAutoUpdater(ILogger logger, SemanticVersion latestVersion)
	{
		try
		{
			// kill any existing processes of the updater
			foreach (var existingProcess in Process.GetProcessesByName("GuiUpdater"))
			{
				try
				{
					existingProcess.Kill();
					existingProcess.WaitForExit();
				}
				catch (Exception ex)
				{
					logger.Error(ex, "Failed to kill existing GuiUpdater process.");
				}
			}

			// win: object-editor-5.3.5-win-x64.zip
			// osx: object-editor-5.3.5-osx-x64.tar
			// linux: object-editor-5.3.5-linux-x64.tar
			var platform = PlatformSpecific.EditorPlatformExtension;
			var filename = $"object-editor-{latestVersion}-{platform}";

			var startInfo = new ProcessStartInfo("GuiUpdater.exe",
			[
				"--pid",
				$"{Environment.ProcessId}",
				"--url",
				$"{GithubLatestReleaseDownloadPage}/download/{latestVersion}/{filename}",
				"--app-path",
				$"{Environment.ProcessPath}",
			])
			{
				UseShellExecute = true,
				//RedirectStandardOutput = true,
				//RedirectStandardError = true,
				//CreateNoWindow = true,
			};

			var process = Process.Start(startInfo);
			Environment.Exit(0); // close the main app so the updater can do its thing

			//var stdOut = Task.Run(() =>
			//{
			//	string line;
			//	while ((line = process.StandardOutput.ReadLine()) != null)
			//	{
			//		logger.Info($"[Updater] {line}");
			//	}
			//});

			//var stdErro = Task.Run(() =>
			//{
			//	string line;
			//	while ((line = process.StandardError.ReadLine()) != null)
			//	{
			//		logger.Info($"[Updater] {line}");
			//	}
			//});

			//await stdOut;
			//await stdErro;
		}
		catch (Exception ex)
		{
			Debug.WriteLine($"Failed to start auto-updater: {ex}");
		}
	}

	public static SemanticVersion GetCurrentAppVersion()
	{
		var assembly = Assembly.GetExecutingAssembly();
		if (assembly == null)
		{
			return UnknownVersion;
		}

		// grab current app version from assembly
		const string versionFilename = "Gui.version.txt";
		using (var stream = assembly.GetManifestResourceStream(versionFilename))
		using (var ms = new MemoryStream())
		{
			stream!.CopyTo(ms);
			var versionText = Encoding.ASCII.GetString(ms.ToArray());
			return GetVersionFromText(versionText);
		}
	}

	//#if !DEBUG
	// thanks for this one @IntelOrca, https://github.com/IntelOrca/PeggleEdit/blob/master/src/peggleedit/Forms/MainMDIForm.cs#L848-L861
	public static SemanticVersion GetLatestAppVersion(SemanticVersion currentVersion)
	{
		var client = new HttpClient();
		client.DefaultRequestHeaders.UserAgent.Add(new ProductInfoHeaderValue(GithubApplicationName, currentVersion.ToString()));
		var response = client.GetAsync(GithubLatestReleaseAPI).Result;
		if (response.IsSuccessStatusCode)
		{
			var jsonResponse = response.Content.ReadAsStringAsync().Result;
			var body = JsonSerializer.Deserialize<VersionCheckBody>(jsonResponse);
			var versionText = body?.TagName;
			return GetVersionFromText(versionText);
		}

#pragma warning disable CA2201 // Do not raise reserved exception types
		throw new Exception($"Unable to get latest version. Error={response.StatusCode}");
#pragma warning restore CA2201 // Do not raise reserved exception types
	}
	//#endif

	public static readonly SemanticVersion UnknownVersion = new(0, 0, 0, "unknown");

	static SemanticVersion GetVersionFromText(string? versionText)
		=> string.IsNullOrEmpty(versionText)
			? UnknownVersion
			: SemanticVersion.TryParse(versionText.Trim(), out var version)
				? version
				: UnknownVersion;
}
