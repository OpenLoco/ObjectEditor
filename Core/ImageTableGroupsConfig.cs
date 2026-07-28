using Common;
using Definitions.ObjectModels.Graphics;
using Microsoft.Extensions.Logging;
using System.Reflection;

namespace Core;

public static class ImageTableGroupsConfig
{
	public const string FileName = "imageTableGroups.json";
	public const string EmbeddedResourceName = "Core.ImageTableGroups.json";

	public static async Task<string?> ReadDefaultAsync(ILogger logger)
	{
		try
		{
			await using var stream = Assembly.GetExecutingAssembly().GetManifestResourceStream(EmbeddedResourceName);
			if (stream == null)
			{
				logger.LogError("Default image table group configuration resource not found");
				return null;
			}

			using var reader = new StreamReader(stream, leaveOpen: true);
			return await reader.ReadToEndAsync();
		}
		catch (Exception ex)
		{
			logger.LogError(ex, "Failed to read the default image table group config");
			return null;
		}
	}

	public static async Task LoadDefaultAsync(ILogger logger)
	{
		var json = await ReadDefaultAsync(logger);
		if (json == null)
		{
			return;
		}

		ImageTableGrouper.LoadGroupConfigurationJson(logger, json);
	}

	public static async Task EnsureOnDiskAndLoadAsync(Common.Logging.Logger logger, string pathName)
	{
		logger.LogInformation("Attempting to load image table group config from '{ImageTableGroupsFileName}'", pathName);

		var defaultImageTableGroups = await ReadDefaultAsync(logger);
		if (defaultImageTableGroups == null)
		{
			logger.LogError("Failed to load default image table group configuration - groups will not be automatically created for existing images. Please ensure the default config file is present and valid at '{ImageTableGroupsFileName}'", pathName);
			return;
		}

		var currentImageTableGroups = defaultImageTableGroups;

		if (File.Exists(pathName))
		{
			var jsonVersion = ImageTableGrouper.ReadImageTableGroupVersion(logger, pathName);
			if (jsonVersion == null || jsonVersion < VersionHelpers.GetCurrentAppVersion())
			{
				currentImageTableGroups = defaultImageTableGroups;
			}
			else
			{
				await File.WriteAllTextAsync(pathName, defaultImageTableGroups);
			}
		}
		else
		{
			await File.WriteAllTextAsync(pathName, defaultImageTableGroups);
		}

		ImageTableGrouper.LoadGroupConfigurationJson(logger, currentImageTableGroups);
	}
}
