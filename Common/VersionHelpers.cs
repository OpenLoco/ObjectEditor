using Common.Logging;
using NuGet.Versioning;
using System.Diagnostics;
using System.Net.Http.Headers;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Text;
using System.Text.Json;

namespace Common;

public static class VersionHelpers
{
	public const string ObjectEditorName = "ObjectEditor";
	public const string ObjectEditorUpdaterName = "ObjectEditorUpdater";
	public const string GithubIssuePage = "https://github.com/OpenLoco/ObjectEditor/issues";
	public const string GithubLatestReleaseDownloadPage = "https://github.com/OpenLoco/ObjectEditor/releases";
	public const string GithubLatestReleaseAPI = "https://api.github.com/repos/OpenLoco/ObjectEditor/releases/latest";

	public static Process? OpenDownloadPage()
		=> Process.Start(new ProcessStartInfo(GithubLatestReleaseDownloadPage) { UseShellExecute = true });

	// win: object-editor-5.3.5-win-x64.zip
	// osx: object-editor-5.3.5-osx-x64.tar
	// linux: object-editor-5.3.5-linux-x64.tar

	static string DownloadFilename(SemanticVersion latestVersion, OSPlatform platform)
		=> $"object-editor-{latestVersion}-{PlatformSpecific.EditorPlatformZipName(platform)}";

	public static string UrlForDownload(SemanticVersion latestVersion, OSPlatform platform)
		=> $"{GithubLatestReleaseDownloadPage}/download/{latestVersion}/{DownloadFilename(latestVersion, platform)}";

	public static void StartAutoUpdater(ILogger logger, SemanticVersion currentVersion, SemanticVersion latestVersion)
	{
		logger.Debug("Attempting to kill existing updater processes");
		try
		{
			// kill any existing processes of the updater
			foreach (var existingProcess in Process.GetProcessesByName(ObjectEditorUpdaterName))
			{
				try
				{
					existingProcess.Kill();
					existingProcess.WaitForExit();
				}
				catch (Exception ex)
				{
					logger.Error(ex, "Failed to kill existing ObjectEditorUpdater process.");
				}
			}

			var editorExe = $"{ObjectEditorUpdaterName}.exe";
			if (!File.Exists(editorExe))
			{
				logger.Error($"Cannot find the auto-updater executable: {editorExe}. You'll need to manually download the update.");
				return;
			}

			var startInfo = new ProcessStartInfo(editorExe,
			[
				"--pid",
				$"{Environment.ProcessId}",
				"--current-version",
				currentVersion.ToString(),
				"--app-path",
				$"{Environment.ProcessPath}",
			])
			{
				// updater process will log to file
				UseShellExecute = false,
				CreateNoWindow = true,
			};

			logger.Debug($"CurrentProcessId: {Environment.ProcessId}");
			logger.Debug($"CurrentProcessPath: {Environment.ProcessPath}");
			logger.Debug($"Attempting to start auto-updater \"{startInfo}\"");
			var process = Process.Start(startInfo);

			if (process != null)
			{
				logger.Info($"Started auto-updater process (PID {process.Id}) to update from {currentVersion} to {latestVersion}. Editor will now close");
				Environment.Exit(0);
			}
			else
			{
				logger.Error("Failed to start auto-updater process. You'll need to manually download the update.");
			}
		}
		catch (Exception ex)
		{
			logger.Error($"Failed to start auto-updater: {ex}");
		}
	}

	public static SemanticVersion GetCurrentAppVersion()
	{
		var assembly = Assembly.GetCallingAssembly();
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

	// thanks for this one @IntelOrca, https://github.com/IntelOrca/PeggleEdit/blob/master/src/peggleedit/Forms/MainMDIForm.cs#L848-L861
	public static SemanticVersion GetLatestAppVersion(string productName, SemanticVersion? currentVersion = null)
	{
		var client = new HttpClient();
		client.DefaultRequestHeaders.UserAgent.Add(new ProductInfoHeaderValue(productName, currentVersion?.ToString()));
		var response = client.GetAsync(GithubLatestReleaseAPI).Result;
		if (response.IsSuccessStatusCode)
		{
			var jsonResponse = response.Content.ReadAsStringAsync().Result;
			var body = JsonSerializer.Deserialize<VersionCheckBody>(jsonResponse);
			var versionText = body?.TagName;
			return GetVersionFromText(versionText);
		}

		return UnknownVersion;
	}

	public static readonly SemanticVersion UnknownVersion = new(0, 0, 0, "unknown");

	static SemanticVersion GetVersionFromText(string? versionText)
		=> string.IsNullOrEmpty(versionText)
			? UnknownVersion
			: SemanticVersion.TryParse(versionText.Trim(), out var version)
				? version
				: UnknownVersion;
}
