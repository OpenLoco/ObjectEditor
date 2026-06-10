using Common.Json;
using Definitions.ObjectModels.Objects.Competitor;
using Definitions.ObjectModels.Objects.Vehicle;
using Definitions.ObjectModels.Types;
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

		Debug.Assert(imageTable.GraphicsElements.Count == originalCount, "Image grouping lost or gained images");

		return imageTable;
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
				return [new("<uncategorised>", [.. imageList])];
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

	private static bool TryGetGroupConfiguration(ObjectType objectType, [NotNullWhen(true)] out ImageTableGroupConfiguration? configuration)
	{
		configuration = null;
		return GroupConfigurations.TryGetValue(objectType, out configuration);
	}

	private static IEnumerable<ImageTableGroup> CreateGroupsFromConfig(ImageTableGroupConfiguration configuration, List<GraphicsElement> imageList)
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

	public static void LoadGroupConfigurationFile(string configFilePath)
	{
		if (string.IsNullOrEmpty(configFilePath) || !File.Exists(configFilePath))
		{
			GroupConfigurations = new Dictionary<ObjectType, ImageTableGroupConfiguration>();
			return;
		}

		try
		{
			var json = File.ReadAllText(configFilePath);
			var configurations = JsonSerializer.Deserialize<List<ImageTableGroupConfiguration>>(json, JsonFile.DefaultSerializerOptions) ?? [];
			GroupConfigurations = configurations
				.Select(configuration => (configuration, success: Enum.TryParse<ObjectType>(configuration.ObjectType, ignoreCase: true, out var objectType), objectType))
				.Where(pair => pair.success)
				.ToDictionary(pair => pair.objectType, pair => pair.configuration);
		}
		catch (JsonException)
		{
			GroupConfigurations = new Dictionary<ObjectType, ImageTableGroupConfiguration>();
		}
	}

	private static IReadOnlyDictionary<ObjectType, ImageTableGroupConfiguration> GroupConfigurations = new Dictionary<ObjectType, ImageTableGroupConfiguration>();

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
