using Common.Json;
using Definitions.ObjectModels.Objects.Competitor;
using Definitions.ObjectModels.Objects.Vehicle;
using Definitions.ObjectModels.Types;
using Microsoft.Extensions.Logging;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Text.Json;

namespace Definitions.ObjectModels.Graphics.ImageTable;

public static class ImageTableGrouper
{
	public static ImageTable CreateImageTable(ILocoStruct obj, ObjectType objectType, List<GraphicsElement> imageList)
	{
		var originalCount = imageList.Count;

		ImageTableNamer.NameImages(obj, objectType, imageList);

		var imageTable = new ImageTable();
		try
		{
			imageTable.Groups = [.. CreateGroups(obj, objectType, imageList)];
		}
		catch (Exception)
		{
			imageTable.Groups = [new("<parsing-error>", [.. imageList])];
		}

		Debug.Assert(imageTable.GraphicsElements.Count == originalCount, "Image grouping lost or gained images");

		return imageTable;
	}

	private static IEnumerable<ImageTableGroup> CreateGroups(ImageTableGroupConfigurationJson itgc, ILocoStruct obj, ObjectType objectType, List<GraphicsElement> imageList)
	{
		if (!TryGetGroupConfiguration(itgc, objectType, out var configuration))
		{
			return [new("<json-file-error>", [.. imageList])];
		}

		switch (objectType)
		{
			case ObjectType.InterfaceSkin:
				return CreateGroupsFromConfig(configuration, imageList);
			case ObjectType.Sound:
				return [new("<none>", [.. imageList])];
			case ObjectType.Currency:
				return [new("<uncategorised>", [.. imageList])];
			case ObjectType.Steam:
				return [new("<uncategorised>", [.. imageList])];
			case ObjectType.CliffEdge:
				return CreateGroupsFromConfig(configuration, imageList);
			case ObjectType.Water:
				return CreateGroupsFromConfig(configuration, imageList);
			case ObjectType.Land:
				return [new("<uncategorised>", [.. imageList])];
			case ObjectType.TownNames:
				return [new("<none>", [.. imageList])];
			case ObjectType.Cargo:
				return CreateGroupsFromConfig(configuration, imageList);
			case ObjectType.Wall:
				return [new("<uncategorised>", [.. imageList])];
			case ObjectType.TrackSignal:
				return [new("<uncategorised>", [.. imageList])];
			case ObjectType.LevelCrossing:
				return [new("<uncategorised>", [.. imageList])];
			case ObjectType.StreetLight:
				return CreateGroupsFromConfig(configuration, imageList);
			case ObjectType.Tunnel:
				return [new("<uncategorised>", [.. imageList])];
			case ObjectType.Bridge:
				return CreateGroupsFromConfig(configuration, imageList);
			case ObjectType.TrackStation:
				return [new("<uncategorised>", [.. imageList])];
			case ObjectType.TrackExtra:
				return [new("<uncategorised>", [.. imageList])];
			case ObjectType.Track:
				return [new("<uncategorised>", [.. imageList])];
			case ObjectType.RoadStation:
				return [new("<uncategorised>", [.. imageList])];
			case ObjectType.RoadExtra:
				return [new("<uncategorised>", [.. imageList])];
			case ObjectType.Road:
				return [new("<uncategorised>", [.. imageList])];
			case ObjectType.Airport:
				return CreateGroupsFromConfig(configuration, imageList);
			case ObjectType.Dock:
				return CreateGroupsFromConfig(configuration, imageList);
			case ObjectType.Vehicle:
				return CreateVehicleGroups((VehicleObject)obj, imageList);
			case ObjectType.Tree:
				return CreateGroupsFromConfig(configuration, imageList);
			case ObjectType.Snow:
				return [new("<uncategorised>", [.. imageList])];
			case ObjectType.Climate:
				return [new("<none>", [.. imageList])];
			case ObjectType.HillShapes:
				return [new("<uncategorised>", [.. imageList])];
			case ObjectType.Building:
				return CreateGroupsFromConfig(configuration, imageList);
			case ObjectType.Industry:
				return CreateGroupsFromConfig(configuration, imageList);
			case ObjectType.Region:
				return [new("<uncategorised>", [.. imageList])];
			case ObjectType.Competitor:
				return CreateCompetitorGroups((CompetitorObject)obj, imageList);
			case ObjectType.ScenarioText:
				return [new("<none>", [.. imageList])];
			case ObjectType.Scaffolding:
				return CreateGroupsFromConfig(configuration, imageList);
			default:
				return [];
		}
	}

	private static bool TryGetGroupConfiguration(ImageTableGroupConfigurationJson itgc, ObjectType objectType, [NotNullWhen(true)] out List<ImageTableGroupDefinitionJson>? configuration)
	{
		configuration = null;
		return itgc.Definitions.TryGetValue(objectType.ToString(), out configuration);
	}

	private static IEnumerable<ImageTableGroup> CreateGroupsFromConfig(List<ImageTableGroupDefinitionJson> configuration, List<GraphicsElement> imageList)
	{
		var groups = configuration.OrderBy(group => group.Start).ToList();
		for (var index = 0; index < groups.Count; index++)
		{
			var current = groups[index];
			var nextStart = index + 1 < groups.Count
				? groups[index + 1].Start
				: imageList.Count;

			if (current.Start < 0)
			{
				continue;
			}

			if (current.Start >= imageList.Count)
			{
				break; // no images remain for this or later groups
			}

			var actualEnd = Math.Min(nextStart, imageList.Count);
			if (actualEnd <= current.Start)
			{
				continue; // no images for this group
			}

			if (current.ChunkSize is null)
			{
				if (nextStart > imageList.Count)
				{
					yield return new("<uncategorised>", imageList[current.Start..actualEnd]);
					break;
				}

				yield return new(current.Name, imageList[current.Start..actualEnd]);
				continue;
			}

			if (current.ChunkSize <= 0)
			{
				continue;
			}

			var actualChunkSize = current.ChunkSize.Value;
			var chunkIndex = 0;
			for (var chunkStart = current.Start; chunkStart < actualEnd; chunkStart += actualChunkSize)
			{
				var chunkEnd = Math.Min(chunkStart + actualChunkSize, actualEnd);
				var chunkName = current.Name.Contains("{i}")
					? current.Name.Replace("{i}", chunkIndex.ToString())
					: current.Name;

				yield return new(chunkName, imageList[chunkStart..chunkEnd]);
				chunkIndex++;
			}

			if (nextStart > imageList.Count)
			{
				break;
			}
		}
	}

	//public static ImageTableGroupConfigurationJson InitialiseImageTableGroupsConfigFile(SemanticVersion currentAppVersion, string imageTableGroupsPathName, string defaultImageTableGroupsPathName, ILogger logger)
	//{
	//	return LoadImageGroupsConfigFile(currentAppVersion, imageTableGroupsPathName, defaultImageTableGroupsPathName, logger);
	//}

	//static void EnsureDefaultImageTableGroupsConfigFileExists(string defaultImageTableGroupsPathName, ILogger logger)
	//{
	//	var defaultImageTableGroups = LoadDefaultImageGroupsConfigFile(logger);
	//	if (defaultImageTableGroups == null)
	//	{
	//		logger.LogError("Failed to load default image table group configuration - groups will not be automatically created for existing images. Please ensure the default config file is present and valid at '{ImageTableGroupsFileName}'", defaultImageTableGroupsPathName);
	//		return;
	//	}

	//	File.WriteAllText(defaultImageTableGroupsPathName, defaultImageTableGroups);
	//}

	//public static ImageTableGroupConfigurationJson? LoadImageGroupsConfigFile(SemanticVersion currentAppVersion, string imageTableGroupsPathName, ILogger logger)
	//{
	//	var imageGroupConfigFileToUse = FileToUse(currentAppVersion, imageTableGroupsPathName, defaultImageTableGroupsPathName, logger);

	//	LoadImageGroupsConfigFileCore(imageGroupConfigFileToUse, logger);

	//	static string FileToUse(SemanticVersion currentAppVersion, string imageTableGroupsPathName, string defaultImageTableGroupsPathName, ILogger logger)
	//	{
	//		var imageGroupConfigFileToUse = defaultImageTableGroupsPathName;
	//		logger.LogInformation("Attempting to load image table group config from '{ImageTableGroupsFileName}'", imageTableGroupsPathName);

	//		if (File.Exists(imageTableGroupsPathName))
	//		{
	//			var jsonVersion = ReadImageTableGroupVersion(imageTableGroupsPathName, logger);
	//			if (jsonVersion != null && jsonVersion == currentAppVersion)
	//			{
	//				logger.LogInformation("Using current image table group config file with version {Version} from '{ImageTableGroupsFileName}'", jsonVersion, imageTableGroupsPathName);
	//				imageGroupConfigFileToUse = imageTableGroupsPathName;
	//			}
	//			else
	//			{
	//				logger.LogWarning("Image table group config file version {Version} is older than the current app version ({AppVersion}) - loading default config file instead.", jsonVersion, currentAppVersion);
	//			}
	//		}
	//		else
	//		{
	//			logger.LogWarning("Image table group config file not found at '{ImageTableGroupsFileName}' - loading default config file instead.", imageTableGroupsPathName);
	//		}

	//		return imageGroupConfigFileToUse;
	//	}

	//	static SemanticVersion? ReadImageTableGroupVersion(string imageTableGroupsFileName, ILogger logger)
	//	{
	//		var existingText = File.ReadAllText(imageTableGroupsFileName);
	//		if (string.IsNullOrWhiteSpace(existingText))
	//		{
	//			logger.LogError("Existing image table group configuration file is empty");
	//			return null;
	//		}

	//		try
	//		{
	//			using var doc = JsonDocument.Parse(existingText);
	//			if (doc.RootElement.ValueKind == JsonValueKind.Object && doc.RootElement.TryGetProperty("version", out var verProp) && verProp.ValueKind == JsonValueKind.String)
	//			{
	//				var existingVersionText = verProp.GetString();
	//				if (!string.IsNullOrEmpty(existingVersionText) && SemanticVersion.TryParse(existingVersionText, out var existingVersion))
	//				{
	//					logger.LogDebug("Existing image table group configuration version: {version}", existingVersion);
	//					return existingVersion;
	//				}
	//			}
	//		}
	//		catch (Exception ex)
	//		{
	//			logger.LogError(ex, "Error occurred while reading image table group version");
	//		}

	//		return null;
	//	}
	//}

	// Just force-load the provided file. Don't check version or fall back to default.
	// This is used in ImageTableViewModel for when the user wants to reload the file.
	// They're responsible for checking the version themselves.
	public static ImageTableGroupConfigurationJson? LoadImageGroupsConfigFileCore(string imageTableGroupsPathName, ILogger logger)
	{
		try
		{
			ArgumentNullException.ThrowIfNull(imageTableGroupsPathName, nameof(imageTableGroupsPathName));
			if (!File.Exists(imageTableGroupsPathName))
			{
				throw new FileNotFoundException("Image table group configuration file not found", imageTableGroupsPathName);
			}

			var json = File.ReadAllText(imageTableGroupsPathName);
			var itgc = JsonSerializer.Deserialize<ImageTableGroupConfigurationJson>(json, JsonFile.DefaultSerializerOptions);

			if (itgc == null)
			{
				throw new InvalidOperationException("Image table group configuration file is empty or invalid");
			}

			logger.LogInformation("Loaded image table group configuration for {Count} object types", itgc.Definitions.Count);
			return itgc;
		}
		catch (Exception ex)
		{
			logger.LogError(ex, "Failed to load image table group configuration");
			return null;
		}
	}

	public static List<ImageTableGroup> CreateGroupsForExistingImages(ImageTableGroupConfigurationJson itgc, ILocoStruct obj, ObjectType objectType, List<GraphicsElement> imageList)
	{
		var originalCount = imageList.Count;
		var groups = CreateGroups(itgc, obj, objectType, imageList).ToList();

		Debug.Assert(groups.SelectMany(g => g.GraphicsElements).Count() == originalCount, "Image grouping lost or gained images");

		return groups;
	}

	//public static List<ImageTableGroup> CreateGroupsForExistingImages(ILocoStruct obj, ObjectType objectType, List<GraphicsElement> imageList)
	//{
	//	var originalCount = imageList.Count;
	//	var groups = CreateGroups(obj, objectType, imageList).ToList();

	//	Debug.Assert(groups.SelectMany(g => g.GraphicsElements).Count() == originalCount, "Image grouping lost or gained images");

	//	return groups;
	//}

	private static IEnumerable<ImageTableGroup> CreateCompetitorGroups(CompetitorObject model, List<GraphicsElement> imageList)
	{
		var offset = 0;
		foreach (var emotion in Enum.GetValues<EmotionFlags>())
		{
			if (model.Emotions.HasFlag(emotion))
			{
				yield return new(emotion.ToString().ToLower(), imageList[offset..(offset + 2)]);
				offset += 2;
			}
		}
	}

	static uint8_t GetYawAccuracyFlat(uint8_t numFrames)
		=> numFrames switch
		{
			8 => 1,
			16 => 2,
			32 => 3,
			_ => 4,
		};

	static uint8_t GetYawAccuracySloped(uint8_t numFrames)
		=> numFrames switch
		{
			4 => 0,
			8 => 1,
			16 => 2,
			_ => 3,
		};

	private static IEnumerable<ImageTableGroup> CreateVehicleGroups(VehicleObject model, List<GraphicsElement> imageList)
	{
		var offset = 0;

		var counter = 0;
		foreach (var bodySprite in model.BodySprites)
		{
			if (!bodySprite.Flags.HasFlag(BodySpriteFlags.HasSprites))
			{
				continue;
			}

			var symmetryMultiplier = bodySprite.Flags.HasFlag(BodySpriteFlags.RotationalSymmetry) ? 2 : 1;

			// flat
			{
				var flatImageIdStart = offset;
				bodySprite._FlatYawAccuracy = GetYawAccuracyFlat(bodySprite.NumFlatRotationFrames);
				bodySprite._NumPermutationsPerRotation = (uint8_t)(bodySprite.NumAnimationFrames * bodySprite.NumCargoFrames * bodySprite.NumRollFrames + (bodySprite.Flags.HasFlag(BodySpriteFlags.HasBrakingLights) ? 1 : 0));

				var numFlatFrames = bodySprite._NumPermutationsPerRotation * bodySprite.NumFlatRotationFrames;
				offset += numFlatFrames / symmetryMultiplier;

				yield return new($"[bodySprite {counter}] flat", imageList[flatImageIdStart..offset]);
			}

			if (bodySprite.Flags.HasFlag(BodySpriteFlags.HasGentleSprites))
			{
				{
					var numGentleTransitionFrames = bodySprite._NumPermutationsPerRotation * 4; // transition frames up/down deg6

					// gentle transition up
					{
						var gentleTransitionUpImageIdStart = offset;
						offset += numGentleTransitionFrames / symmetryMultiplier;
						yield return new($"[bodySprite {counter}] gentle transition up", imageList[gentleTransitionUpImageIdStart..offset]);
					}

					// gentle transition down
					{
						var gentleTransitionDownImageIdStart = offset;
						offset += numGentleTransitionFrames / symmetryMultiplier;
						yield return new($"[bodySprite {counter}] gentle transition down", imageList[gentleTransitionDownImageIdStart..offset]);
					}
				}

				{
					bodySprite._SlopedYawAccuracy = GetYawAccuracySloped(bodySprite.NumSlopedRotationFrames);
					var numGentleFrames = bodySprite._NumPermutationsPerRotation * bodySprite.NumSlopedRotationFrames; // up/down deg12

					// gentle up
					{
						var gentleUpImageIdStart = offset;
						offset += numGentleFrames / symmetryMultiplier;
						yield return new($"[bodySprite {counter}] gentle up", imageList[gentleUpImageIdStart..offset]);
					}

					// gentle down
					{
						var gentleDownImageIdStart = offset;
						offset += numGentleFrames / symmetryMultiplier;
						yield return new($"[bodySprite {counter}] gentle down", imageList[gentleDownImageIdStart..offset]);
					}
				}

				if (bodySprite.Flags.HasFlag(BodySpriteFlags.HasSteepSprites))
				{
					{
						var numSteepTransitionFrames = bodySprite._NumPermutationsPerRotation * 4; // transition frames up/down deg18

						// steep transition up
						{
							var steepTransitionUpImageIdStart = offset;
							offset += numSteepTransitionFrames / symmetryMultiplier;
							yield return new($"[bodySprite {counter}] steep transition up", imageList[steepTransitionUpImageIdStart..offset]);
						}

						// steep transition down
						{
							var steepTransitionDownImageIdStart = offset;
							offset += numSteepTransitionFrames / symmetryMultiplier;
							yield return new($"[bodySprite {counter}] steep transition down", imageList[steepTransitionDownImageIdStart..offset]);
						}
					}

					{
						var numSteepFrames = bodySprite.NumSlopedRotationFrames * bodySprite._NumPermutationsPerRotation; // up/down deg25

						// steep up
						{
							var steepUpImageIdStart = offset;
							offset += numSteepFrames / symmetryMultiplier;
							yield return new($"[bodySprite {counter}] steep up", imageList[steepUpImageIdStart..offset]);
						}

						// steep down
						{
							var steepDownImageIdStart = offset;
							offset += numSteepFrames / symmetryMultiplier;
							yield return new($"[bodySprite {counter}] steep down", imageList[steepDownImageIdStart..offset]);
						}
					}
				}
			}

			counter++;
		}

		counter = 0;
		foreach (var bogieSprite in model.BogieSprites)
		{
			if (!bogieSprite.Flags.HasFlag(BogieSpriteFlags.HasSprites))
			{
				continue;
			}

			var symmetryMultiplier = bogieSprite.Flags.HasFlag(BogieSpriteFlags.RotationalSymmetry) ? 2 : 1;

			// flat
			{
				var flatImageIdStart = offset;
				var numFlatFrames = bogieSprite.NumAnimationFrames * 32;
				offset += numFlatFrames / symmetryMultiplier;

				yield return new($"[bogieSprite {counter}] flat", imageList[flatImageIdStart..offset]);
			}

			if (bogieSprite.Flags.HasFlag(BogieSpriteFlags.HasGentleSprites))
			{
				var numGentleFrames = bogieSprite.NumAnimationFrames * 32; // up/down 12 deg

				// gentle up
				{
					var gentleUpImageIdStart = offset;
					offset += numGentleFrames / symmetryMultiplier;
					yield return new($"[bogieSprite {counter}] gentle up", imageList[gentleUpImageIdStart..offset]);
				}

				// gentle down
				{
					var gentleDownImageIdStart = offset;
					offset += numGentleFrames / symmetryMultiplier;
					yield return new($"[bogieSprite {counter}] gentle down", imageList[gentleDownImageIdStart..offset]);
				}

				if (bogieSprite.Flags.HasFlag(BogieSpriteFlags.HasSteepSprites))
				{
					var numSteepFrames = bogieSprite.NumAnimationFrames * 32; // up/down 25 deg

					// steep up
					{
						var steepUpImageIdStart = offset;
						offset += numSteepFrames / symmetryMultiplier;
						yield return new($"[bogieSprite {counter}] steep up", imageList[steepUpImageIdStart..offset]);
					}

					// steep down
					{
						var steepDownImageIdStart = offset;
						offset += numSteepFrames / symmetryMultiplier;
						yield return new($"[bogieSprite {counter}] steep down", imageList[steepDownImageIdStart..offset]);
					}
				}
			}

			counter++;
		}

		var remainder = imageList[offset..];
		if (remainder.Count > 0)
		{
			yield return new("<uncategorised>", remainder);
		}
	}

}
