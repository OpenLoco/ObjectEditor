using Common;
using Common.Json;
using Common.Logging;
using Definitions.ObjectModels.Objects.Competitor;
using Definitions.ObjectModels.Objects.LevelCrossing;
using Definitions.ObjectModels.Objects.Vehicle;
using Definitions.ObjectModels.Types;
using Microsoft.Extensions.Logging;
using NuGet.Versioning;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Text.Json;

namespace Definitions.ObjectModels.Graphics;

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

		//Debug.Assert(imageTable.GraphicsElements.Count == originalCount, "Image grouping lost or gained images");

		return imageTable;
	}

	public static IEnumerable<ImageTableGroup> CreateGroupsForExistingImages(ILocoStruct obj, ObjectType objectType, List<GraphicsElement> imageList)
	{
		var originalCount = imageList.Count;
		var groups = CreateGroups(obj, objectType, imageList).ToList();

		Debug.Assert(groups.SelectMany(g => g.GraphicsElements).Count() == originalCount, "Image grouping lost or gained images");

		return groups;
	}

	private static IEnumerable<ImageTableGroup> CreateGroups(ILocoStruct obj, ObjectType objectType, List<GraphicsElement> imageList)
	{
		switch (objectType)
		{
			case ObjectType.InterfaceSkin:
				return CreateGroupsFromConfig(ObjectType.InterfaceSkin, imageList);
			case ObjectType.Sound:
				return [new("<none>", [.. imageList])];
			case ObjectType.Currency:
				return [new("<uncategorised>", [.. imageList])];
			case ObjectType.Steam:
				return [new("<uncategorised>", [.. imageList])];
			case ObjectType.CliffEdge:
				return CreateGroupsFromConfig(ObjectType.CliffEdge, imageList);
			case ObjectType.Water:
				return CreateGroupsFromConfig(ObjectType.Water, imageList);
			case ObjectType.Land:
				return [new("<uncategorised>", [.. imageList])];
			case ObjectType.TownNames:
				return [new("<none>", [.. imageList])];
			case ObjectType.Cargo:
				return CreateGroupsFromConfig(ObjectType.Cargo, imageList);
			case ObjectType.Wall:
				return [new("<uncategorised>", [.. imageList])];
			case ObjectType.TrackSignal:
				return [new("<uncategorised>", [.. imageList])];
			case ObjectType.LevelCrossing:
				//return CreateLevelCrossingGroups((LevelCrossingObject)obj, imageList);
				return CreateLevelCrossingGroups2((LevelCrossingObject)obj, imageList);
				//return [new("<uncategorised>", [.. imageList])];
			case ObjectType.StreetLight:
				return CreateGroupsFromConfig(ObjectType.StreetLight, imageList);
			case ObjectType.Tunnel:
				return [new("<uncategorised>", [.. imageList])];
			case ObjectType.Bridge:
				return CreateGroupsFromConfig(ObjectType.Bridge, imageList);
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
				return CreateGroupsFromConfig(ObjectType.Airport, imageList);
			case ObjectType.Dock:
				return CreateGroupsFromConfig(ObjectType.Dock, imageList);
			case ObjectType.Vehicle:
				return CreateVehicleGroups((VehicleObject)obj, imageList);
			case ObjectType.Tree:
				return CreateGroupsFromConfig(ObjectType.Tree, imageList);
			case ObjectType.Snow:
				return [new("<uncategorised>", [.. imageList])];
			case ObjectType.Climate:
				return [new("<none>", [.. imageList])];
			case ObjectType.HillShapes:
				return [new("<uncategorised>", [.. imageList])];
			case ObjectType.Building:
				return CreateGroupsFromConfig(ObjectType.Building, imageList);
			case ObjectType.Industry:
				return CreateGroupsFromConfig(ObjectType.Industry, imageList);
			case ObjectType.Region:
				return [new("<uncategorised>", [.. imageList])];
			case ObjectType.Competitor:
				return CreateCompetitorGroups((CompetitorObject)obj, imageList);
			case ObjectType.ScenarioText:
				return [new("<none>", [.. imageList])];
			case ObjectType.Scaffolding:
				return CreateGroupsFromConfig(ObjectType.Scaffolding, imageList);
			default:
				return [];
		}
	}

	private static IEnumerable<ImageTableGroup> CreateGroupsFromConfig(ObjectType objectType, List<GraphicsElement> imageList)
	{
		if (TryGetGroupConfiguration(objectType, out var configuration))
		{
			return CreateGroupsFromConfig(configuration, imageList);
		}

		return [new("<json-file-error>", [.. imageList])];
	}

	private static bool TryGetGroupConfiguration(ObjectType objectType, [NotNullWhen(true)] out ImageTableGroupConfigurationType? configuration)
	{
		configuration = null;
		return GroupConfigurations.TryGetValue(objectType, out configuration);
	}

	private static IEnumerable<ImageTableGroup> CreateGroupsFromConfig(ImageTableGroupConfigurationType configuration, List<GraphicsElement> imageList)
	{
		var groups = configuration.Groups.OrderBy(group => group.Start).ToList();
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

	public static void LoadGroupConfigurationJson(ILogger logger, string json)
	{
		try
		{
			var itgc = JsonSerializer.Deserialize<ImageTableGroupConfiguration>(json, JsonFile.DefaultSerializerOptions);
			GroupConfigurations = itgc?.Definitions
				.Select(configuration => (configuration, success: Enum.TryParse<ObjectType>(configuration.ObjectType, ignoreCase: true, out var objectType), objectType))
				.Where(pair => pair.success)
				.ToDictionary(pair => pair.objectType, pair => pair.configuration) ?? [];
		}
		catch (JsonException ex)
		{
			logger.LogError(ex, "Image table group config is not valid JSON or version could not be read");
		}
	}

	private static IReadOnlyDictionary<ObjectType, ImageTableGroupConfigurationType> GroupConfigurations = new Dictionary<ObjectType, ImageTableGroupConfigurationType>();

	static string GetDirection(int i)
		=> i switch
		{
			0 => "SW",
			1 => "SW",
			2 => "NE",
			3 => "NE",
			4 => "SE",
			5 => "SE",
			6 => "NW",
			7 => "NW",
			_ => $"direction {i}",
		};

	private static IEnumerable<ImageTableGroup> CreateLevelCrossingGroups2(LevelCrossingObject model, List<GraphicsElement> imageList)
	{
		for (var i = 0; i < 8; ++i)
		{
			yield return new($"{GetDirection(i)} side {i % 2}", imageList.PickEach(8, i).ToList());
		}
	}

	private static IEnumerable<ImageTableGroup> CreateLevelCrossingGroups(LevelCrossingObject model, List<GraphicsElement> imageList)
	{
		var offset = 0;

		yield return new("closing SW 1", imageList.PickEach(8, 0).ToList());
		yield return new("closing SW 2", imageList.PickEach(8, 1).ToList());
		yield return new("closing NE 1", imageList.PickEach(8, 2).ToList());
		yield return new("closing NE 2", imageList.PickEach(8, 3).ToList());
		yield return new("closing SE 1", imageList.PickEach(8, 4).ToList());
		yield return new("closing SE 2", imageList.PickEach(8, 5).ToList());
		yield return new("closing NW 1", imageList.PickEach(8, 6).ToList());
		yield return new("closing NW 2", imageList.PickEach(8, 7).ToList());

		offset += model.IdleClosedFrames * 8;

		yield return new("closing SW 1", imageList.Skip(offset).PickEach(8, 0).ToList());
		yield return new("closing SW 2", imageList.Skip(offset).PickEach(8, 1).ToList());
		yield return new("closing NE 1", imageList.Skip(offset).PickEach(8, 2).ToList());
		yield return new("closing NE 2", imageList.Skip(offset).PickEach(8, 3).ToList());
		yield return new("closing SE 1", imageList.Skip(offset).PickEach(8, 4).ToList());
		yield return new("closing SE 2", imageList.Skip(offset).PickEach(8, 5).ToList());
		yield return new("closing NW 1", imageList.Skip(offset).PickEach(8, 6).ToList());
		yield return new("closing NW 2", imageList.Skip(offset).PickEach(8, 7).ToList());
		//yield return new("closed", imageList[offset..(offset + model.ClosedFrames * 8)]);
		//offset += model.ClosedFrames * 8;

		//yield return new("opened", imageList[offset..(offset + model.var_0A * 4)]);
		//offset += model.var_0A * 4;
	}

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
