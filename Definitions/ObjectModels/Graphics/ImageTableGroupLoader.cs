using Core;
using Core.Logging;
using Definitions.ObjectModels.Types;
using Microsoft.Extensions.Logging;
using NuGet.Versioning;
using System.Reflection;
using System.Text.Json;

using GroupConfigDict = System.Collections.Generic.IReadOnlyDictionary<
	Definitions.ObjectModels.Types.ObjectType,
	Definitions.ObjectModels.Graphics.ImageTableGroupConfigurationType>;

namespace Definitions.ObjectModels.Graphics;

public static class ImageTableGroupLoader
{
	public const string FileName = "imageTableGroups.json";
	public const string EmbeddedResourceName = "Core.ImageTableGroups.json";

	public static SemanticVersion? ReadImageTableGroupVersion(Logger logger, string imageTableGroupsFileName)
	{
		var existingText = File.ReadAllText(imageTableGroupsFileName);
		if (string.IsNullOrWhiteSpace(existingText))
		{
			logger.LogError("Existing image table group configuration file is empty");
			return null;
		}

		try
		{
			using var doc = JsonDocument.Parse(existingText);
			if (doc.RootElement.ValueKind == JsonValueKind.Object && doc.RootElement.TryGetProperty("version", out var verProp) && verProp.ValueKind == JsonValueKind.String)
			{
				var existingVersionText = verProp.GetString();
				if (!string.IsNullOrEmpty(existingVersionText) && SemanticVersion.TryParse(existingVersionText, out var existingVersion))
				{
					logger.LogDebug("Existing image table group configuration version: {version}", existingVersion);
					return existingVersion;
				}
			}
		}
		catch (Exception ex)
		{
			logger.LogError(ex, "Error occurred while reading image table group version");
		}

		return null;
	}

	public static GroupConfigDict? LoadGroupConfigurationJson(ILogger logger, string json)
	{
		try
		{
			var itgc = JsonSerializer.Deserialize<ImageTableGroupConfiguration>(json, JsonFile.DefaultSerializerOptions);
			return itgc?.Definitions
				.Select(configuration => (configuration, success: Enum.TryParse<ObjectType>(configuration.ObjectType, ignoreCase: true, out var objectType), objectType))
				.Where(pair => pair.success)
				.ToDictionary(pair => pair.objectType, pair => pair.configuration) ?? [];
		}
		catch (JsonException ex)
		{
			logger.LogError(ex, "Image table group config is not valid JSON or version could not be read");
		}

		return null;
	}

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

	public static async Task<GroupConfigDict?> LoadDefaultAsync(ILogger logger)
	{
		var json = await ReadDefaultAsync(logger);
		if (json == null)
		{
			return null;
		}

		return LoadGroupConfigurationJson(logger, json);
	}

	public static async Task<GroupConfigDict?> EnsureOnDiskAndLoadAsync(Logger logger, string pathName)
	{
		logger.LogInformation("Attempting to load image table group config from '{ImageTableGroupsFileName}'", pathName);

		var defaultImageTableGroups = await ReadDefaultAsync(logger);
		if (defaultImageTableGroups == null)
		{
			logger.LogError("Failed to load default image table group configuration - groups will not be automatically created for existing images. Please ensure the default config file is present and valid at '{ImageTableGroupsFileName}'", pathName);
			return null;
		}

		var currentImageTableGroups = defaultImageTableGroups;

		if (File.Exists(pathName))
		{
			var jsonVersion = ReadImageTableGroupVersion(logger, pathName);
			if (jsonVersion == null || jsonVersion < VersionHelpers.GetCurrentAppVersion())
			{
				await File.WriteAllTextAsync(pathName, defaultImageTableGroups);
				currentImageTableGroups = defaultImageTableGroups;
			}
			else
			{
				currentImageTableGroups = await File.ReadAllTextAsync(pathName);
			}
		}
		else
		{
			await File.WriteAllTextAsync(pathName, defaultImageTableGroups);
			currentImageTableGroups = defaultImageTableGroups;
		}

		return LoadGroupConfigurationJson(logger, currentImageTableGroups);
	}
}
