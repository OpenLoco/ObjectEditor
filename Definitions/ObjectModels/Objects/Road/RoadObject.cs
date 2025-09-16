using Definitions.ObjectModels.Types;
using System.Diagnostics.CodeAnalysis;

namespace Definitions.ObjectModels.Objects.Road;

public class RoadObject : ILocoStruct, IImageTableNameProvider
{
	public RoadTraitFlags RoadPieces { get; set; }
	public int16_t BuildCostFactor { get; set; }
	public int16_t SellCostFactor { get; set; }
	public int16_t TunnelCostFactor { get; set; }
	public uint8_t CostIndex { get; set; }
	public int16_t MaxCurveSpeed { get; set; }
	public RoadObjectFlags Flags { get; set; }
	public uint8_t PaintStyle { get; set; }
	public uint8_t DisplayOffset { get; set; }
	public TownSize TargetTownSize { get; set; }

	public List<ObjectModelHeader> CompatibleTracksAndRoads { get; set; } = [];
	public List<ObjectModelHeader> RoadMods { get; set; } = [];
	public ObjectModelHeader Tunnel { get; set; }
	public List<ObjectModelHeader> Bridges { get; set; } = [];
	public List<ObjectModelHeader> Stations { get; set; } = [];

	public bool Validate()
	{
		// check missing in vanilla
		if (CostIndex >= 32)
		{
			return false;
		}

		if (-SellCostFactor > BuildCostFactor)
		{
			return false;
		}

		if (BuildCostFactor <= 0)
		{
			return false;
		}

		if (TunnelCostFactor <= 0)
		{
			return false;
		}

		if (Bridges.Count > 7)
		{
			return false;
		}

		if (RoadMods.Count > 2)
		{
			return false;
		}

		if (Flags.HasFlag(RoadObjectFlags.unk_03))
		{
			return RoadMods.Count == 0;
		}

		return true;
	}

	public bool TryGetImageName(int id, [MaybeNullWhen(false)] out string value)
	{
		if (id is >= 0 and <= 31)
		{
			value = $"uiPreviewImage{id}";
			return true;
		}

		if (id is >= 32 and <= 33)
		{
			return ImageIdNameMap.TryGetValue(id, out value);
		}

		// style dependent
		return PaintStyle switch
		{
			0 => ImageIdNameMap_Style0.TryGetValue(id, out value),
			1 => ImageIdNameMap_Style1.TryGetValue(id, out value),
			2 => ImageIdNameMap_Style2.TryGetValue(id, out value),
			_ => throw new NotImplementedException(id.ToString()),
		};
	}

	public static Dictionary<int, string> ImageIdNameMap = new()
	{
		{ 32, "uiPickupFromTrack" },
		{ 33, "uiPlaceOnTrack" },
	};

	public static Dictionary<int, string> ImageIdNameMap_Style0 = new()
	{
		{ 34, "kStraight0NE" },
		{ 35, "kStraight0SE" },
		{ 36, "kRightCurveVerySmall0NE" },
		{ 37, "kRightCurveVerySmall0SE" },
		{ 38, "kRightCurveVerySmall0SW" },
		{ 39, "kRightCurveVerySmall0NW" },
		{ 40, "kJunctionLeft0NE" },
		{ 41, "kJunctionLeft0SE" },
		{ 42, "kJunctionLeft0SW" },
		{ 43, "kJunctionLeft0NW" },
		{ 44, "kJunctionCrossroads0NE" },
		{ 45, "kRightCurveSmall0NE" },
		{ 46, "kRightCurveSmall1NE" },
		{ 47, "kRightCurveSmall2NE" },
		{ 48, "kRightCurveSmall3NE" },
		{ 49, "kRightCurveSmall0SE" },
		{ 50, "kRightCurveSmall1SE" },
		{ 51, "kRightCurveSmall2SE" },
		{ 52, "kRightCurveSmall3SE" },
		{ 53, "kRightCurveSmall0SW" },
		{ 54, "kRightCurveSmall1SW" },
		{ 55, "kRightCurveSmall2SW" },
		{ 56, "kRightCurveSmall3SW" },
		{ 57, "kRightCurveSmall0NW" },
		{ 58, "kRightCurveSmall1NW" },
		{ 59, "kRightCurveSmall2NW" },
		{ 60, "kRightCurveSmall3NW" },
		{ 61, "kStraightSlopeUp0NE" },
		{ 62, "kStraightSlopeUp1NE" },
		{ 63, "kStraightSlopeUp0SE" },
		{ 64, "kStraightSlopeUp1SE" },
		{ 65, "kStraightSlopeUp0SW" },
		{ 66, "kStraightSlopeUp1SW" },
		{ 67, "kStraightSlopeUp0NW" },
		{ 68, "kStraightSlopeUp1NW" },
		{ 69, "kStraightSteepSlopeUp0NE" },
		{ 70, "kStraightSteepSlopeUp0SE" },
		{ 71, "kStraightSteepSlopeUp0SW" },
		{ 72, "kStraightSteepSlopeUp0NW" },
		{ 73, "kTurnaround0NE" },
		{ 74, "kTurnaround0SE" },
		{ 75, "kTurnaround0SW" },
		{ 76, "kTurnaround0NW" },
	};

	public static Dictionary<int, string> ImageIdNameMap_Style1 = new()
	{
		{ 34, "kStraight0BallastNE" },
		{ 35, "kStraight0BallastSE" },
		{ 36, "kStraight0SleeperNE" },
		{ 37, "kStraight0SleeperSE" },
		{ 38, "kStraight0RailNE" },
		{ 39, "kStraight0RailSE" },
		{ 40, "kRightCurveSmall0BallastNE" },
		{ 41, "kRightCurveSmall1BallastNE" },
		{ 42, "kRightCurveSmall2BallastNE" },
		{ 43, "kRightCurveSmall3BallastNE" },
		{ 44, "kRightCurveSmall0BallastSE" },
		{ 45, "kRightCurveSmall1BallastSE" },
		{ 46, "kRightCurveSmall2BallastSE" },
		{ 47, "kRightCurveSmall3BallastSE" },
		{ 48, "kRightCurveSmall0BallastSW" },
		{ 49, "kRightCurveSmall1BallastSW" },
		{ 50, "kRightCurveSmall2BallastSW" },
		{ 51, "kRightCurveSmall3BallastSW" },
		{ 52, "kRightCurveSmall0BallastNW" },
		{ 53, "kRightCurveSmall1BallastNW" },
		{ 54, "kRightCurveSmall2BallastNW" },
		{ 55, "kRightCurveSmall3BallastNW" },
		{ 56, "kRightCurveSmall0SleeperNE" },
		{ 57, "kRightCurveSmall1SleeperNE" },
		{ 58, "kRightCurveSmall2SleeperNE" },
		{ 59, "kRightCurveSmall3SleeperNE" },
		{ 60, "kRightCurveSmall0SleeperSE" },
		{ 61, "kRightCurveSmall1SleeperSE" },
		{ 62, "kRightCurveSmall2SleeperSE" },
		{ 63, "kRightCurveSmall3SleeperSE" },
		{ 64, "kRightCurveSmall0SleeperSW" },
		{ 65, "kRightCurveSmall1SleeperSW" },
		{ 66, "kRightCurveSmall2SleeperSW" },
		{ 67, "kRightCurveSmall3SleeperSW" },
		{ 68, "kRightCurveSmall0SleeperNW" },
		{ 69, "kRightCurveSmall1SleeperNW" },
		{ 70, "kRightCurveSmall2SleeperNW" },
		{ 71, "kRightCurveSmall3SleeperNW" },
		{ 72, "kRightCurveSmall0RailNE" },
		{ 73, "kRightCurveSmall1RailNE" },
		{ 74, "kRightCurveSmall2RailNE" },
		{ 75, "kRightCurveSmall3RailNE" },
		{ 76, "kRightCurveSmall0RailSE" },
		{ 77, "kRightCurveSmall1RailSE" },
		{ 78, "kRightCurveSmall2RailSE" },
		{ 79, "kRightCurveSmall3RailSE" },
		{ 80, "kRightCurveSmall0RailSW" },
		{ 81, "kRightCurveSmall1RailSW" },
		{ 82, "kRightCurveSmall2RailSW" },
		{ 83, "kRightCurveSmall3RailSW" },
		{ 84, "kRightCurveSmall0RailNW" },
		{ 85, "kRightCurveSmall1RailNW" },
		{ 86, "kRightCurveSmall2RailNW" },
		{ 87, "kRightCurveSmall3RailNW" },
		{ 88, "kStraightSlopeUp0BallastNE" },
		{ 89, "kStraightSlopeUp1BallastNE" },
		{ 90, "kStraightSlopeUp0BallastSE" },
		{ 91, "kStraightSlopeUp1BallastSE" },
		{ 92, "kStraightSlopeUp0BallastSW" },
		{ 93, "kStraightSlopeUp1BallastSW" },
		{ 94, "kStraightSlopeUp0BallastNW" },
		{ 95, "kStraightSlopeUp1BallastNW" },
		{ 96, "kStraightSlopeUp0SleeperNE" },
		{ 97, "kStraightSlopeUp1SleeperNE" },
		{ 98, "kStraightSlopeUp0SleeperSE" },
		{ 99, "kStraightSlopeUp1SleeperSE" },
		{ 100, "kStraightSlopeUp0SleeperSW" },
		{ 101, "kStraightSlopeUp1SleeperSW" },
		{ 102, "kStraightSlopeUp0SleeperNW" },
		{ 103, "kStraightSlopeUp1SleeperNW" },
		{ 104, "kStraightSlopeUp0RailNE" },
		{ 105, "kStraightSlopeUp1RailNE" },
		{ 106, "kStraightSlopeUp0RailSE" },
		{ 107, "kStraightSlopeUp1RailSE" },
		{ 108, "kStraightSlopeUp0RailSW" },
		{ 109, "kStraightSlopeUp1RailSW" },
		{ 110, "kStraightSlopeUp0RailNW" },
		{ 111, "kStraightSlopeUp1RailNW" },
		{ 112, "kStraightSteepSlopeUp0BallastNE" },
		{ 113, "kStraightSteepSlopeUp0BallastSE" },
		{ 114, "kStraightSteepSlopeUp0BallastSW" },
		{ 115, "kStraightSteepSlopeUp0BallastNW" },
		{ 116, "kStraightSteepSlopeUp0SleeperNE" },
		{ 117, "kStraightSteepSlopeUp0SleeperSE" },
		{ 118, "kStraightSteepSlopeUp0SleeperSW" },
		{ 119, "kStraightSteepSlopeUp0SleeperNW" },
		{ 120, "kStraightSteepSlopeUp0RailNE" },
		{ 121, "kStraightSteepSlopeUp0RailSE" },
		{ 122, "kStraightSteepSlopeUp0RailSW" },
		{ 123, "kStraightSteepSlopeUp0RailNW" },
		{ 124, "kRightCurveVerySmall0BallastNE" },
		{ 125, "kRightCurveVerySmall0BallastSE" },
		{ 126, "kRightCurveVerySmall0BallastSW" },
		{ 127, "kRightCurveVerySmall0BallastNW" },
		{ 128, "kRightCurveVerySmall0SleeperNE" },
		{ 129, "kRightCurveVerySmall0SleeperSE" },
		{ 130, "kRightCurveVerySmall0SleeperSW" },
		{ 131, "kRightCurveVerySmall0SleeperNW" },
		{ 132, "kRightCurveVerySmall0RailNE" },
		{ 133, "kRightCurveVerySmall0RailSE" },
		{ 134, "kRightCurveVerySmall0RailSW" },
		{ 135, "kRightCurveVerySmall0RailNW" },
		{ 136, "kTurnaround0BallastNE" },
		{ 137, "kTurnaround0BallastSE" },
		{ 138, "kTurnaround0BallastSW" },
		{ 139, "kTurnaround0BallastNW" },
		{ 140, "kTurnaround0SleeperNE" },
		{ 141, "kTurnaround0SleeperSE" },
		{ 142, "kTurnaround0SleeperSW" },
		{ 143, "kTurnaround0SleeperNW" },
		{ 144, "kTurnaround0RailNE" },
		{ 145, "kTurnaround0RailSE" },
		{ 146, "kTurnaround0RailSW" },
		{ 147, "kTurnaround0RailNW" },
	};

	public static Dictionary<int, string> ImageIdNameMap_Style2 = new()
	{
		{ 34, "kStraight0NE" },
		{ 35, "kStraight0SE" },
		{ 36, "kLeftCurveVerySmall0NW" },
		{ 37, "kLeftCurveVerySmall0NE" },
		{ 38, "kLeftCurveVerySmall0SE" },
		{ 39, "kLeftCurveVerySmall0SW" },
		{ 40, "kJunctionLeft0NE" },
		{ 41, "kJunctionLeft0SE" },
		{ 42, "kJunctionLeft0SW" },
		{ 43, "kJunctionLeft0NW" },
		{ 44, "kJunctionCrossroads0NE" },
		{ 45, "kLeftCurveSmall3NW" },
		{ 46, "kLeftCurveSmall1NW" },
		{ 47, "kLeftCurveSmall2NW" },
		{ 48, "kLeftCurveSmall0NW" },
		{ 49, "kLeftCurveSmall3NE" },
		{ 50, "kLeftCurveSmall1NE" },
		{ 51, "kLeftCurveSmall2NE" },
		{ 52, "kLeftCurveSmall0NE" },
		{ 53, "kLeftCurveSmall3SE" },
		{ 54, "kLeftCurveSmall1SE" },
		{ 55, "kLeftCurveSmall2SE" },
		{ 56, "kLeftCurveSmall0SE" },
		{ 57, "kLeftCurveSmall3SW" },
		{ 58, "kLeftCurveSmall1SW" },
		{ 59, "kLeftCurveSmall2SW" },
		{ 60, "kLeftCurveSmall0SW" },
		{ 61, "kStraightSlopeUp0NE" },
		{ 62, "kStraightSlopeUp1NE" },
		{ 63, "kStraightSlopeUp0SE" },
		{ 64, "kStraightSlopeUp1SE" },
		{ 65, "kStraightSlopeUp0SW" },
		{ 66, "kStraightSlopeUp1SW" },
		{ 67, "kStraightSlopeUp0NW" },
		{ 68, "kStraightSlopeUp1NW" },
		{ 69, "kStraightSteepSlopeUp0NE" },
		{ 70, "kStraightSteepSlopeUp0SE" },
		{ 71, "kStraightSteepSlopeUp0SW" },
		{ 72, "kStraightSteepSlopeUp0NW" },
		{ 73, "kTurnaround0NE" },
		{ 74, "kTurnaround0SE" },
		{ 75, "kTurnaround0SW" },
		{ 76, "kTurnaround0NW" },

		{ 85, "kStraight0SW" },
		{ 86, "kStraight0NW" },
		{ 87, "kRightCurveVerySmall0NE" },
		{ 88, "kRightCurveVerySmall0SE" },
		{ 89, "kRightCurveVerySmall0SW" },
		{ 90, "kRightCurveVerySmall0NW" },
		{ 91, "kJunctionRight0NE" },
		{ 92, "kJunctionRight0SE" },
		{ 93, "kJunctionRight0SW" },
		{ 94, "kJunctionRight0NW" },
        // Must duplicate kJunctionCrossroads0NE
        { 95, "kJunctionCrossroads0NE2" },
		{ 96, "kRightCurveSmall0NE" },
		{ 97, "kRightCurveSmall1NE" },
		{ 98, "kRightCurveSmall2NE" },
		{ 99, "kRightCurveSmall3NE" },
		{ 100, "kRightCurveSmall0SE" },
		{ 101, "kRightCurveSmall1SE" },
		{ 102, "kRightCurveSmall2SE" },
		{ 103, "kRightCurveSmall3SE" },
		{ 104, "kRightCurveSmall0SW" },
		{ 105, "kRightCurveSmall1SW" },
		{ 106, "kRightCurveSmall2SW" },
		{ 107, "kRightCurveSmall3SW" },
		{ 108, "kRightCurveSmall0NW" },
		{ 109, "kRightCurveSmall1NW" },
		{ 110, "kRightCurveSmall2NW" },
		{ 111, "kRightCurveSmall3NW" },
		{ 112, "kStraightSlopeDown1SW" },
		{ 113, "kStraightSlopeDown0SW" },
		{ 114, "kStraightSlopeDown1NW" },
		{ 115, "kStraightSlopeDown0NW" },
		{ 116, "kStraightSlopeDown1NE" },
		{ 117, "kStraightSlopeDown0NE" },
		{ 118, "kStraightSlopeDown1SE" },
		{ 119, "kStraightSlopeDown0SE" },
		{ 120, "kStraightSteepSlopeDown0SW" },
		{ 121, "kStraightSteepSlopeDown0NW" },
		{ 122, "kStraightSteepSlopeDown0NE" },
		{ 123, "kStraightSteepSlopeDown0SE" },
	};
}
