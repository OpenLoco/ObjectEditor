using Microsoft.Extensions.Logging;
using NuGet.Versioning;
using System.Diagnostics;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Text;

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

	// win:   object-editor-5.3.5-win-x64.zip
	// osx:   object-editor-5.3.5-osx-arm64.zip (or -osx-x64.zip)
	// linux: object-editor-5.3.5-linux-x64.tar.gz

	static string DownloadFilename(SemanticVersion latestVersion, OSPlatform platform, Architecture architecture)
		=> $"object-editor-{latestVersion}-{PlatformSpecific.EditorPlatformZipName(platform, architecture)}";

	public static string UrlForDownload(SemanticVersion latestVersion, OSPlatform platform, Architecture? architecture = null)
		=> $"{GithubLatestReleaseDownloadPage}/download/{latestVersion}/{DownloadFilename(latestVersion, platform, architecture ?? RuntimeInformation.ProcessArchitecture)}";

	public static void StartAutoUpdater(ILogger logger, SemanticVersion currentVersion, SemanticVersion latestVersion)
	{
		logger.LogDebug("Attempting to kill existing updater processes");
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
					logger.LogError(ex, "Failed to kill existing ObjectEditorUpdater process.");
				}
			}

var editorExe = PlatformSpecific.EditorPlatformUpdaterName(PlatformSpecific.GetPlatform);
			// resolve relative to the editor's own folder: inside a macOS .app bundle the working directory
			// is unrelated to where the binaries live
			var editorExePath = Path.Combine(Path.GetDirectoryName(Environment.ProcessPath) ?? ".", editorExe);
			if (!File.Exists(editorExePath))
			{
				logger.LogError("Cannot find the auto-updater executable: {EditorExePath}. You'll need to manually download the update.", editorExePath);
				return;
			}

			var startInfo = new ProcessStartInfo(editorExePath,
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

			logger.LogDebug("CurrentProcessId: {ProcessId}", Environment.ProcessId);
			logger.LogDebug("CurrentProcessPath: {ProcessPath}", Environment.ProcessPath);
			logger.LogDebug("Attempting to start auto-updater \"{StartInfo}\"", startInfo);
			var process = Process.Start(startInfo);

			if (process != null)
			{
				logger.LogInformation("Started auto-updater process (PID {Id}) to update from {CurrentVersion} to {LatestVersion}. Editor will now close", process.Id, currentVersion, latestVersion);
				Environment.Exit(0);
			}
			else
			{
				logger.LogError("Failed to start auto-updater process. You'll need to manually download the update.");
			}
		}
		catch (Exception ex)
		{
			logger.LogError("Failed to start auto-updater: {Ex}", ex);
		}
	}

	static SemanticVersion? CachedSemanticVersion;

	public static SemanticVersion GetCurrentAppVersion()
	{
		if (CachedSemanticVersion != null)
		{
			return CachedSemanticVersion;
		}

		// prefer the version baked into the entry assembly by the build (`dotnet publish -p:Version=`), so the
		// reported version cannot drift from the binaries. SourceLink appends the commit hash as build metadata.
		var entryAssembly = Assembly.GetEntryAssembly();
		if (entryAssembly != null)
		{
			var informationalVersion = entryAssembly.GetCustomAttribute<AssemblyInformationalVersionAttribute>()?.InformationalVersion;
			CachedSemanticVersion = GetVersionFromText(informationalVersion?.Split('+')[0]);
			if (CachedSemanticVersion != UnknownVersion)
			{
				return CachedSemanticVersion;
			}
		}

		// fall back to the embedded version.txt, which is how older and CI builds report their version
		foreach (var assembly in AppDomain.CurrentDomain.GetAssemblies())
		{
			// version.txt is an embedded resource in the Definitions assembly
			using var stream = assembly.GetManifestResourceStream("Definitions.version.txt")
				?? assembly.GetManifestResourceStream("Gui.version.txt")
				?? assembly.GetManifestResourceStream("version.txt");
			if (stream == null)
			{
				continue;
			}

			using var ms = new MemoryStream();
			stream.CopyTo(ms);
			CachedSemanticVersion = GetVersionFromText(Encoding.ASCII.GetString(ms.ToArray()));
			return CachedSemanticVersion;
		}

		CachedSemanticVersion = UnknownVersion;
		return CachedSemanticVersion;
	}

	// A single, process-wide HttpClient avoids socket exhaustion that comes from
	// creating a new instance on every call. Kept internal-static deliberately:
	// there's no DI container in `Common`, so we cannot inject IHttpClientFactory here.
	static readonly HttpClient sharedHttpClient = new();

	// thanks for this one @IntelOrca, https://github.com/IntelOrca/PeggleEdit/blob/master/src/peggleedit/Forms/MainMDIForm.cs#L848-L861
	public static async Task<SemanticVersion> GetLatestAppVersionAsync(string productName, SemanticVersion? currentVersion = null, CancellationToken cancellationToken = default)
	{
		using var request = new HttpRequestMessage(HttpMethod.Get, GithubLatestReleaseAPI);
		request.Headers.UserAgent.Add(new ProductInfoHeaderValue(productName, currentVersion?.ToString()));

		using var response = await sharedHttpClient.SendAsync(request, cancellationToken).ConfigureAwait(false);
		if (!response.IsSuccessStatusCode)
		{
			return UnknownVersion;
		}

		var body = await response.Content.ReadFromJsonAsync<VersionCheckBody>(cancellationToken).ConfigureAwait(false);
		return GetVersionFromText(body?.TagName);
	}

	public static readonly SemanticVersion UnknownVersion = new(0, 0, 0, "unknown");

	static SemanticVersion GetVersionFromText(string? versionText)
		=> string.IsNullOrEmpty(versionText)
			? UnknownVersion
			: SemanticVersion.TryParse(versionText.Trim(), out var version)
				? version
				: UnknownVersion;
}
