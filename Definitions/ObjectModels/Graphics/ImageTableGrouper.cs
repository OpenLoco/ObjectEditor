using Definitions.ObjectModels.Objects.Competitor;
using Definitions.ObjectModels.Objects.Vehicle;
using Definitions.ObjectModels.Types;
using System.Diagnostics;

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
				return CreateInterfaceGroups(imageList);
			case ObjectType.Sound:
				return [new("<none>", [.. imageList])];
			case ObjectType.Currency:
				return [new("<uncategorised>", [.. imageList])];
			case ObjectType.Steam:
				return [new("<uncategorised>", [.. imageList])];
			case ObjectType.CliffEdge:
				return CreateCliffEdgeGroups(imageList);
			case ObjectType.Water:
				return CreateWaterGroups(imageList);
			case ObjectType.Land:
				return [new("<uncategorised>", [.. imageList])];
			case ObjectType.TownNames:
				return [new("<none>", [.. imageList])];
			case ObjectType.Cargo:
				return CreateCargoGroups(imageList);
			case ObjectType.Wall:
				return [new("<uncategorised>", [.. imageList])];
			case ObjectType.TrackSignal:
				return [new("<uncategorised>", [.. imageList])];
			case ObjectType.LevelCrossing:
				return [new("<uncategorised>", [.. imageList])];
			case ObjectType.StreetLight:
				return CreateStreetLightGroups(imageList);
			case ObjectType.Tunnel:
				return [new("<uncategorised>", [.. imageList])];
			case ObjectType.Bridge:
				return CreateBridgeGroups(imageList);
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
				return CreateAirportGroups(imageList);
			case ObjectType.Dock:
				return CreateDockGroups(imageList);
			case ObjectType.Vehicle:
				return CreateVehicleGroups((VehicleObject)obj, imageList);
			case ObjectType.Tree:
				return CreateTreeGroups(imageList);
			case ObjectType.Snow:
				return [new("<uncategorised>", [.. imageList])];
			case ObjectType.Climate:
				return [new("<none>", [.. imageList])];
			case ObjectType.HillShapes:
				return [new("<uncategorised>", [.. imageList])];
			case ObjectType.Building:
				return CreateBuildingGroups(imageList);
			case ObjectType.Industry:
				return CreateBuildingGroups(imageList);
			case ObjectType.Region:
				return [new("<uncategorised>", [.. imageList])];
			case ObjectType.Competitor:
				return CreateCompetitorGroups((CompetitorObject)obj, imageList);
			case ObjectType.ScenarioText:
				return [new("<none>", [.. imageList])];
			case ObjectType.Scaffolding:
				return CreateScaffoldingGroups(imageList);
			default:
				return [];
		}
	}

	private static IEnumerable<ImageTableGroup> CreateAirportGroups(List<GraphicsElement> imageList)
	{
		yield return new("preview", imageList[0..1]);

		foreach (var group in imageList
			.Skip(1)
			.Chunk(4)
			.Select((x, i) => new ImageTableGroup($"Part {i}", [.. x])))
		{
			yield return group;
		}
	}

	private static IEnumerable<ImageTableGroup> CreateBridgeGroups(List<GraphicsElement> imageList)
	{
		yield return new("preview", imageList[0..1]);
		yield return new("base plates", imageList[1..6]);
		yield return new("unk", imageList[6..12]);
		yield return new("<uncategorised>", imageList[12..]);
	}

	private static IEnumerable<ImageTableGroup> CreateBuildingGroups(List<GraphicsElement> imageList)
		=> imageList
			.Chunk(4)
			.Select((x, i) => new ImageTableGroup($"Part {i}", [.. x]));

	private static IEnumerable<ImageTableGroup> CreateCargoGroups(List<GraphicsElement> imageList)
	{
		yield return new("preview", imageList[0..1]);
		yield return new("station variations", imageList[1..]);
	}

	private static IEnumerable<ImageTableGroup> CreateCliffEdgeGroups(List<GraphicsElement> imageList)
	{
		yield return new("left west", imageList[0..16]);
		yield return new("right east", imageList[16..32]);
		yield return new("right west", imageList[32..48]);
		yield return new("left east", imageList[48..64]);
		yield return new("far-side slopes", imageList[64..]);
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

	static uint8_t getYawAccuracyFlat(uint8_t numFrames)
	{
		switch (numFrames)
		{
			case 8:
				return 1;
			case 16:
				return 2;
			case 32:
				return 3;
			default:
				return 4;
		}
	}

	static uint8_t getYawAccuracySloped(uint8_t numFrames)
	{
		switch (numFrames)
		{
			case 4:
				return 0;
			case 8:
				return 1;
			case 16:
				return 2;
			default:
				return 3;
		}
	}

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
				bodySprite._FlatYawAccuracy = getYawAccuracyFlat(bodySprite.NumFlatRotationFrames);
				bodySprite._NumFramesPerRotation = (uint8_t)(bodySprite.NumAnimationFrames * bodySprite.NumCargoFrames * bodySprite.NumRollFrames + (bodySprite.Flags.HasFlag(BodySpriteFlags.HasBrakingLights) ? 1 : 0));

				var numFlatFrames = bodySprite._NumFramesPerRotation * bodySprite.NumFlatRotationFrames;
				offset += numFlatFrames / symmetryMultiplier;

				yield return new($"[bodySprite {counter}] flat", imageList[flatImageIdStart..offset]);
			}

			if (bodySprite.Flags.HasFlag(BodySpriteFlags.HasGentleSprites))
			{
				{
					var numGentleTransitionFrames = bodySprite._NumFramesPerRotation * 4; // transition frames up/down deg6

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
					bodySprite._SlopedYawAccuracy = getYawAccuracySloped(bodySprite.NumSlopedRotationFrames);
					var numGentleFrames = bodySprite._NumFramesPerRotation * bodySprite.NumSlopedRotationFrames; // up/down deg12

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
						var numSteepTransitionFrames = bodySprite._NumFramesPerRotation * 4; // transition frames up/down deg18

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
						var numSteepFrames = bodySprite.NumSlopedRotationFrames * bodySprite._NumFramesPerRotation; // up/down deg25

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

				// up
				{
					var gentleUpImageIdStart = offset;
					offset += numGentleFrames / symmetryMultiplier;
					yield return new($"[bogieSprite {counter}] gentle up", imageList[gentleUpImageIdStart..offset]);
				}
				{
					var gentleDownImageIdStart = offset;
					offset += numGentleFrames / symmetryMultiplier;
					yield return new($"[bogieSprite {counter}] gentle down", imageList[gentleDownImageIdStart..offset]);
				}

				if (bogieSprite.Flags.HasFlag(BogieSpriteFlags.HasSteepSprites))
				{
					var numSteepFrames = bogieSprite.NumAnimationFrames * 32; // up/down 25 deg

					// up
					{
						var steepUpImageIdStart = offset;
						offset += numSteepFrames / symmetryMultiplier;
						yield return new($"[bogieSprite {counter}] steep", imageList[steepUpImageIdStart..offset]);
					}

					// down
					{
						var steepDownImageIdStart = offset;
						offset += numSteepFrames / symmetryMultiplier;
						yield return new($"[bogieSprite {counter}] steep", imageList[steepDownImageIdStart..offset]);
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

	private static IEnumerable<ImageTableGroup> CreateDockGroups(List<GraphicsElement> imageList)
	{
		yield return new("preview", [imageList[0]]);

		foreach (var group in imageList
			.Skip(1)
			.Chunk(4)
			.Select((x, i) => new ImageTableGroup($"Part {i}", [.. x])))
		{
			yield return group;
		}
	}

	private static IEnumerable<ImageTableGroup> CreateInterfaceGroups(List<GraphicsElement> imageList)
	{
		yield return new("preview", imageList[0..1]);
		yield return new("toolbar", imageList[1..31]);
		yield return new("build-vehicle", imageList[31..43]);
		yield return new("toolbar", imageList[43..49]);
		yield return new("paint", imageList[49..57]);
		yield return new("population", imageList[57..65]);
		yield return new("performance-index", imageList[65..73]);
		yield return new("cargo-units", imageList[73..81]);
		yield return new("cargo-distance", imageList[81..89]);
		yield return new("production", imageList[89..97]);
		yield return new("wrench", imageList[97..113]);
		yield return new("finances", imageList[113..129]);
		yield return new("cup", imageList[129..145]);
		yield return new("ratings", imageList[145..161]);
		yield return new("transported", imageList[161..168]);
		yield return new("cogs", imageList[168..172]);
		yield return new("toolbar", imageList[172..203]);
		yield return new("tab-train", imageList[203..211]);
		yield return new("tab-aircraft", imageList[211..219]);
		yield return new("tab-bus", imageList[219..227]);
		yield return new("tab-tram", imageList[227..235]);
		yield return new("tab-truck", imageList[235..243]);
		yield return new("tab-ship", imageList[243..251]);
		yield return new("build-train", imageList[251..267]);
		yield return new("build-aircraft", imageList[267..283]);
		yield return new("build-bus", imageList[283..299]);
		yield return new("build-tram", imageList[299..315]);
		yield return new("build-truck", imageList[315..331]);
		yield return new("build-ship", imageList[331..347]);
		yield return new("build-industry", imageList[347..363]);
		yield return new("build-town", imageList[363..379]);
		yield return new("build-buildings", imageList[379..395]);
		yield return new("build-misc-buildings", imageList[395..411]);
		yield return new("build-extra", imageList[411..418]);
		yield return new("train", imageList[418..426]);
		yield return new("aircraft", imageList[426..434]);
		yield return new("bus", imageList[434..442]);
		yield return new("tram", imageList[442..450]);
		yield return new("truck", imageList[450..458]);
		yield return new("ship", imageList[458..466]);
		yield return new("toolbar-map", imageList[466..470]);
	}

	private static IEnumerable<ImageTableGroup> CreateScaffoldingGroups(List<GraphicsElement> imageList)
	{
		yield return new("type 0", imageList[0..10]);
		yield return new("type 1", imageList[10..24]);
		yield return new("type 2", imageList[24..36]);
	}

	private static IEnumerable<ImageTableGroup> CreateStreetLightGroups(List<GraphicsElement> imageList)
		=> imageList
			.Chunk(4)
			.Select((x, i) => new ImageTableGroup($"Year group {i}", [.. x]));

	private static IEnumerable<ImageTableGroup> CreateTreeGroups(List<GraphicsElement> imageList)
		=> imageList
			.Chunk(4)
			.Select((x, i) => new ImageTableGroup($"Variation {i}", [.. x]));

	private static IEnumerable<ImageTableGroup> CreateWaterGroups(List<GraphicsElement> imageList)
	{
		yield return new("zoom 1", imageList[0..10]);
		yield return new("zoom 2", imageList[10..20]);
		yield return new("zoom 3", imageList[20..30]);
		yield return new("zoom 4", imageList[30..40]);
		yield return new("palettes", imageList[40..42]);
		yield return new("icon-animation", imageList[42..58]);
		yield return new("icon-interaction", imageList[58..60]);
		yield return new("animation", imageList[60..76]);
	}
}
