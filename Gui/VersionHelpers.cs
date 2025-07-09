using NuGet.Versioning;
using System.Diagnostics;
using System.IO;
using System.Reflection;
using System.Text;

namespace OpenLoco.Gui
{
	public static class VersionHelpers
	{
		public const string GithubApplicationName = "ObjectEditor";
		public const string GithubIssuePage = "https://github.com/OpenLoco/ObjectEditor/issues";
		public const string GithubLatestReleaseDownloadPage = "https://github.com/OpenLoco/ObjectEditor/releases";
		public const string GithubLatestReleaseAPI = "https://api.github.com/repos/OpenLoco/ObjectEditor/releases/latest";

		public static Process? OpenDownloadPage()
			=> Process.Start(new ProcessStartInfo(GithubLatestReleaseDownloadPage) { UseShellExecute = true });

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

#if !DEBUG
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
#endif

		public static readonly SemanticVersion UnknownVersion = new(0, 0, 0, "unknown");

		static SemanticVersion GetVersionFromText(string? versionText)
			=> string.IsNullOrEmpty(versionText)
				? UnknownVersion
				: SemanticVersion.TryParse(versionText.Trim(), out var version)
					? version
					: UnknownVersion;
	}
}
