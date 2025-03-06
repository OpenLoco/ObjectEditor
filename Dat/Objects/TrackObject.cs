using OpenLoco.Dat.Data;
using OpenLoco.Dat.FileParsing;
using OpenLoco.Dat.Types;
using System.ComponentModel;

namespace OpenLoco.Dat.Objects
{
	[Flags]
	public enum TrackTraitFlags : uint16_t
	{
		None = 0,
		Diagonal = 1 << 0,
		LargeCurve = 1 << 1,
		NormalCurve = 1 << 2,
		SmallCurve = 1 << 3,
		VerySmallCurve = 1 << 4,
		Slope = 1 << 5,
		SteepSlope = 1 << 6,
		OneSided = 1 << 7,
		SlopedCurve = 1 << 8,
		SBend = 1 << 9,
		Junction = 1 << 10,
	}

	[Flags]
	public enum TrackObjectFlags : uint16_t
	{
		None = 0,
		unk_00 = 1 << 0,
		unk_01 = 1 << 1,
		unk_02 = 1 << 2,
	}

	[TypeConverter(typeof(ExpandableObjectConverter))]
	[LocoStructSize(0x36)]
	[LocoStructType(ObjectType.Track)]
	[LocoStringTable("Name")]
	public record TrackObject(
		[property: LocoStructOffset(0x00), LocoString, Browsable(false)] string_id Name,
		[property: LocoStructOffset(0x02)] TrackTraitFlags TrackPieces,
		[property: LocoStructOffset(0x04)] TrackTraitFlags StationTrackPieces,
		[property: LocoStructOffset(0x06)] uint8_t var_06,
		[property: LocoStructOffset(0x07)] uint8_t NumCompatibleTracksAndRoads,
		[property: LocoStructOffset(0x08)] uint8_t NumMods,
		[property: LocoStructOffset(0x09)] uint8_t NumSignals,
		[property: LocoStructOffset(0x0A), LocoArrayLength(TrackObject.MaxMods), LocoStructVariableLoad, Browsable(false)] object_id[] _Mods,
		[property: LocoStructOffset(0x0E), LocoStructVariableLoad, Browsable(false)] uint16_t _Signals, // bitset
		[property: LocoStructOffset(0x10), Browsable(false)] uint16_t _CompatibleTracks, // bitset
		[property: LocoStructOffset(0x12), Browsable(false)] uint16_t _CompatibleRoads, // bitset
		[property: LocoStructOffset(0x14)] int16_t BuildCostFactor,
		[property: LocoStructOffset(0x16)] int16_t SellCostFactor,
		[property: LocoStructOffset(0x18)] int16_t TunnelCostFactor,
		[property: LocoStructOffset(0x1A)] uint8_t CostIndex,
		[property: LocoStructOffset(0x1B), Browsable(false)] object_id _Tunnel,
		[property: LocoStructOffset(0x1C)] Speed16 CurveSpeed,
		[property: LocoStructOffset(0x1E), Browsable(false)] image_id Image,
		[property: LocoStructOffset(0x22)] TrackObjectFlags Flags,
		[property: LocoStructOffset(0x24)] uint8_t NumBridges,
		[property: LocoStructOffset(0x25), LocoArrayLength(TrackObject.MaxBridges), Browsable(false)] object_id[] _Bridges,       // 0x25
		[property: LocoStructOffset(0x2C)] uint8_t NumStations,
		[property: LocoStructOffset(0x2D), LocoArrayLength(TrackObject.MaxStations), Browsable(false)] object_id[] _Stations,       // 0x2D
		[property: LocoStructOffset(0x34)] uint8_t DisplayOffset,
		[property: LocoStructOffset(0x35), Browsable(false)] uint8_t var_35
		) : ILocoStruct, ILocoStructVariableData, IImageTableNameProvider
	{
		public List<S5Header> TracksAndRoads { get; set; } = [];
		public List<S5Header> TrackMods { get; set; } = []; // aka TrackExtraObject
		public List<S5Header> Signals { get; set; } = [];
		public S5Header Tunnel { get; set; }
		public List<S5Header> Bridges { get; set; } = [];
		public List<S5Header> Stations { get; set; } = [];

		public const int MaxTunnels = 1;
		public const int MaxBridges = 7;
		public const int MaxStations = 7;
		public const int MaxMods = 4;

		public ReadOnlySpan<byte> Load(ReadOnlySpan<byte> remainingData)
		{
			// compatible roads/tracks
			TracksAndRoads = SawyerStreamReader.LoadVariableCountS5Headers(remainingData, NumCompatibleTracksAndRoads);
			remainingData = remainingData[(S5Header.StructLength * NumCompatibleTracksAndRoads)..];

			// mods
			TrackMods = SawyerStreamReader.LoadVariableCountS5Headers(remainingData, NumMods);
			remainingData = remainingData[(S5Header.StructLength * NumMods)..];

			// signals
			Signals = SawyerStreamReader.LoadVariableCountS5Headers(remainingData, NumSignals);
			remainingData = remainingData[(S5Header.StructLength * NumSignals)..];

			// tunnel
			Tunnel = SawyerStreamReader.LoadVariableCountS5Headers(remainingData, MaxTunnels)[0];
			remainingData = remainingData[(S5Header.StructLength * MaxTunnels)..];

			// bridges
			Bridges = SawyerStreamReader.LoadVariableCountS5Headers(remainingData, NumBridges);
			remainingData = remainingData[(S5Header.StructLength * NumBridges)..];

			// stations
			Stations = SawyerStreamReader.LoadVariableCountS5Headers(remainingData, NumStations);
			remainingData = remainingData[(S5Header.StructLength * NumStations)..];

			// set _CompatibleRoads?
			// set _CompatibleTracks?

			return remainingData;
		}

		public ReadOnlySpan<byte> Save()
		{
			var headers = TracksAndRoads
				.Concat(TrackMods)
				.Concat(Signals)
				.Concat(Enumerable.Repeat(Tunnel, 1))
				.Concat(Bridges)
				.Concat(Stations);

			return headers.SelectMany(h => h.Write().ToArray()).ToArray();
		}

		public bool TryGetImageName(int id, out string? value)
			=> ImageIdNameMap.TryGetValue(id, out value);

		public bool Validate()
		{
			if (var_06 >= 3)
			{
				return false;
			}

			// vanilla missed this check
			if (CostIndex > 32)
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

			if (TrackPieces.HasFlag(TrackTraitFlags.Diagonal | TrackTraitFlags.LargeCurve)
				&& TrackPieces.HasFlag(TrackTraitFlags.OneSided | TrackTraitFlags.VerySmallCurve))
			{
				return false;
			}

			if (NumBridges > 7)
			{
				return false;
			}

			return NumStations <= 7;
		}

		// taken from OpenLoco TrackObject.h
		public static Dictionary<int, string> ImageIdNameMap = new()
		{
			{ 0, "uiPreviewImage0" },
			{ 1, "uiPreviewImage1" },
			{ 2, "uiPreviewImage2" },
			{ 3, "uiPreviewImage3" },
			{ 4, "uiPreviewImage4" },
			{ 5, "uiPreviewImage5" },
			{ 6, "uiPreviewImage6" },
			{ 7, "uiPreviewImage7" },
			{ 8, "uiPreviewImage8" },
			{ 9, "uiPreviewImage9" },
			{ 10, "uiPreviewImage10" },
			{ 11, "uiPreviewImage11" },
			{ 12, "uiPreviewImage12" },
			{ 13, "uiPreviewImage13" },
			{ 14, "uiPreviewImage14" },
			{ 15, "uiPreviewImage15" },
			{ 16, "uiPickupFromTrack" },
			{ 17, "uiPlaceOnTrack" },
			//
			{ 18, "straight0BallastNE" },
			{ 19, "straight0BallastSE" },
			{ 20, "straight0SleeperNE" },
			{ 21, "straight0SleeperSE" },
			{ 22, "straight0RailNE" },
			{ 23, "straight0RailSE" },
			{ 24, "rightCurveSmall0BallastNE" },
			{ 25, "rightCurveSmall1BallastNE" },
			{ 26, "rightCurveSmall2BallastNE" },
			{ 27, "rightCurveSmall3BallastNE" },
			{ 28, "rightCurveSmall0BallastSE" },
			{ 29, "rightCurveSmall1BallastSE" },
			{ 30, "rightCurveSmall2BallastSE" },
			{ 31, "rightCurveSmall3BallastSE" },
			{ 32, "rightCurveSmall0BallastSW" },
			{ 33, "rightCurveSmall1BallastSW" },
			{ 34, "rightCurveSmall2BallastSW" },
			{ 35, "rightCurveSmall3BallastSW" },
			{ 36, "rightCurveSmall0BallastNW" },
			{ 37, "rightCurveSmall1BallastNW" },
			{ 38, "rightCurveSmall2BallastNW" },
			{ 39, "rightCurveSmall3BallastNW" },
			{ 40, "rightCurveSmall0SleeperNE" },
			{ 41, "rightCurveSmall1SleeperNE" },
			{ 42, "rightCurveSmall2SleeperNE" },
			{ 43, "rightCurveSmall3SleeperNE" },
			{ 44, "rightCurveSmall0SleeperSE" },
			{ 45, "rightCurveSmall1SleeperSE" },
			{ 46, "rightCurveSmall2SleeperSE" },
			{ 47, "rightCurveSmall3SleeperSE" },
			{ 48, "rightCurveSmall0SleeperSW" },
			{ 49, "rightCurveSmall1SleeperSW" },
			{ 50, "rightCurveSmall2SleeperSW" },
			{ 51, "rightCurveSmall3SleeperSW" },
			{ 52, "rightCurveSmall0SleeperNW" },
			{ 53, "rightCurveSmall1SleeperNW" },
			{ 54, "rightCurveSmall2SleeperNW" },
			{ 55, "rightCurveSmall3SleeperNW" },
			{ 56, "rightCurveSmall0RailNE" },
			{ 57, "rightCurveSmall1RailNE" },
			{ 58, "rightCurveSmall2RailNE" },
			{ 59, "rightCurveSmall3RailNE" },
			{ 60, "rightCurveSmall0RailSE" },
			{ 61, "rightCurveSmall1RailSE" },
			{ 62, "rightCurveSmall2RailSE" },
			{ 63, "rightCurveSmall3RailSE" },
			{ 64, "rightCurveSmall0RailSW" },
			{ 65, "rightCurveSmall1RailSW" },
			{ 66, "rightCurveSmall2RailSW" },
			{ 67, "rightCurveSmall3RailSW" },
			{ 68, "rightCurveSmall0RailNW" },
			{ 69, "rightCurveSmall1RailNW" },
			{ 70, "rightCurveSmall2RailNW" },
			{ 71, "rightCurveSmall3RailNW" },
			{ 72, "rightCurveSmallSlopeUp0NE" },
			{ 73, "rightCurveSmallSlopeUp1NE" },
			{ 74, "rightCurveSmallSlopeUp2NE" },
			{ 75, "rightCurveSmallSlopeUp3NE" },
			{ 76, "rightCurveSmallSlopeUp0SE" },
			{ 77, "rightCurveSmallSlopeUp1SE" },
			{ 78, "rightCurveSmallSlopeUp2SE" },
			{ 79, "rightCurveSmallSlopeUp3SE" },
			{ 80, "rightCurveSmallSlopeUp0SW" },
			{ 81, "rightCurveSmallSlopeUp1SW" },
			{ 82, "rightCurveSmallSlopeUp2SW" },
			{ 83, "rightCurveSmallSlopeUp3SW" },
			{ 84, "rightCurveSmallSlopeUp0NW" },
			{ 85, "rightCurveSmallSlopeUp1NW" },
			{ 86, "rightCurveSmallSlopeUp2NW" },
			{ 87, "rightCurveSmallSlopeUp3NW" },
			{ 88, "rightCurveSmallSlopeDown0NE" },
			{ 89, "rightCurveSmallSlopeDown1NE" },
			{ 90, "rightCurveSmallSlopeDown2NE" },
			{ 91, "rightCurveSmallSlopeDown3NE" },
			{ 92, "rightCurveSmallSlopeDown0SE" },
			{ 93, "rightCurveSmallSlopeDown1SE" },
			{ 94, "rightCurveSmallSlopeDown2SE" },
			{ 95, "rightCurveSmallSlopeDown3SE" },
			{ 96, "rightCurveSmallSlopeDown0SW" },
			{ 97, "rightCurveSmallSlopeDown1SW" },
			{ 98, "rightCurveSmallSlopeDown2SW" },
			{ 99, "rightCurveSmallSlopeDown3SW" },
			{ 100, "rightCurveSmallSlopeDown0NW" },
			{ 101, "rightCurveSmallSlopeDown1NW" },
			{ 102, "rightCurveSmallSlopeDown2NW" },
			{ 103, "rightCurveSmallSlopeDown3NW" },
			{ 104, "rightCurveSmallSteepSlopeUp0NE" },
			{ 105, "rightCurveSmallSteepSlopeUp1NE" },
			{ 106, "rightCurveSmallSteepSlopeUp2NE" },
			{ 107, "rightCurveSmallSteepSlopeUp3NE" },
			{ 108, "rightCurveSmallSteepSlopeUp0SE" },
			{ 109, "rightCurveSmallSteepSlopeUp1SE" },
			{ 110, "rightCurveSmallSteepSlopeUp2SE" },
			{ 111, "rightCurveSmallSteepSlopeUp3SE" },
			{ 112, "rightCurveSmallSteepSlopeUp0SW" },
			{ 113, "rightCurveSmallSteepSlopeUp1SW" },
			{ 114, "rightCurveSmallSteepSlopeUp2SW" },
			{ 115, "rightCurveSmallSteepSlopeUp3SW" },
			{ 116, "rightCurveSmallSteepSlopeUp0NW" },
			{ 117, "rightCurveSmallSteepSlopeUp1NW" },
			{ 118, "rightCurveSmallSteepSlopeUp2NW" },
			{ 119, "rightCurveSmallSteepSlopeUp3NW" },
			{ 120, "rightCurveSmallSteepSlopeDown0NE" },
			{ 121, "rightCurveSmallSteepSlopeDown1NE" },
			{ 122, "rightCurveSmallSteepSlopeDown2NE" },
			{ 123, "rightCurveSmallSteepSlopeDown3NE" },
			{ 124, "rightCurveSmallSteepSlopeDown0SE" },
			{ 125, "rightCurveSmallSteepSlopeDown1SE" },
			{ 126, "rightCurveSmallSteepSlopeDown2SE" },
			{ 127, "rightCurveSmallSteepSlopeDown3SE" },
			{ 128, "rightCurveSmallSteepSlopeDown0SW" },
			{ 129, "rightCurveSmallSteepSlopeDown1SW" },
			{ 130, "rightCurveSmallSteepSlopeDown2SW" },
			{ 131, "rightCurveSmallSteepSlopeDown3SW" },
			{ 132, "rightCurveSmallSteepSlopeDown0NW" },
			{ 133, "rightCurveSmallSteepSlopeDown1NW" },
			{ 134, "rightCurveSmallSteepSlopeDown2NW" },
			{ 135, "rightCurveSmallSteepSlopeDown3NW" },
			{ 136, "rightCurve0BallastNE" },
			{ 137, "rightCurve1BallastNE" },
			{ 138, "rightCurve2BallastNE" },
			{ 139, "rightCurve3BallastNE" },
			{ 140, "rightCurve4BallastNE" },
			{ 141, "rightCurve0BallastSE" },
			{ 142, "rightCurve1BallastSE" },
			{ 143, "rightCurve2BallastSE" },
			{ 144, "rightCurve3BallastSE" },
			{ 145, "rightCurve4BallastSE" },
			{ 146, "rightCurve0BallastSW" },
			{ 147, "rightCurve1BallastSW" },
			{ 148, "rightCurve2BallastSW" },
			{ 149, "rightCurve3BallastSW" },
			{ 150, "rightCurve4BallastSW" },
			{ 151, "rightCurve0BallastNW" },
			{ 152, "rightCurve1BallastNW" },
			{ 153, "rightCurve2BallastNW" },
			{ 154, "rightCurve3BallastNW" },
			{ 155, "rightCurve4BallastNW" },
			{ 156, "rightCurve0SleeperNE" },
			{ 157, "rightCurve1SleeperNE" },
			{ 158, "rightCurve2SleeperNE" },
			{ 159, "rightCurve3SleeperNE" },
			{ 160, "rightCurve4SleeperNE" },
			{ 161, "rightCurve0SleeperSE" },
			{ 162, "rightCurve1SleeperSE" },
			{ 163, "rightCurve2SleeperSE" },
			{ 164, "rightCurve3SleeperSE" },
			{ 165, "rightCurve4SleeperSE" },
			{ 166, "rightCurve0SleeperSW" },
			{ 167, "rightCurve1SleeperSW" },
			{ 168, "rightCurve2SleeperSW" },
			{ 169, "rightCurve3SleeperSW" },
			{ 170, "rightCurve4SleeperSW" },
			{ 171, "rightCurve0SleeperNW" },
			{ 172, "rightCurve1SleeperNW" },
			{ 173, "rightCurve2SleeperNW" },
			{ 174, "rightCurve3SleeperNW" },
			{ 175, "rightCurve4SleeperNW" },
			{ 176, "rightCurve0RailNE" },
			{ 177, "rightCurve1RailNE" },
			{ 178, "rightCurve2RailNE" },
			{ 179, "rightCurve3RailNE" },
			{ 180, "rightCurve4RailNE" },
			{ 181, "rightCurve0RailSE" },
			{ 182, "rightCurve1RailSE" },
			{ 183, "rightCurve2RailSE" },
			{ 184, "rightCurve3RailSE" },
			{ 185, "rightCurve4RailSE" },
			{ 186, "rightCurve0RailSW" },
			{ 187, "rightCurve1RailSW" },
			{ 188, "rightCurve2RailSW" },
			{ 189, "rightCurve3RailSW" },
			{ 190, "rightCurve4RailSW" },
			{ 191, "rightCurve0RailNW" },
			{ 192, "rightCurve1RailNW" },
			{ 193, "rightCurve2RailNW" },
			{ 194, "rightCurve3RailNW" },
			{ 195, "rightCurve4RailNW" },
			{ 196, "straightSlopeUp0NE" },
			{ 197, "straightSlopeUp1NE" },
			{ 198, "straightSlopeUp0SE" },
			{ 199, "straightSlopeUp1SE" },
			{ 200, "straightSlopeUp0SW" },
			{ 201, "straightSlopeUp1SW" },
			{ 202, "straightSlopeUp0NW" },
			{ 203, "straightSlopeUp1NW" },
			{ 204, "straightSteepSlopeUp0NE" },
			{ 205, "straightSteepSlopeUp0SE" },
			{ 206, "straightSteepSlopeUp0SW" },
			{ 207, "straightSteepSlopeUp0NW" },
			{ 208, "rightCurveLarge0BallastNE" },
			{ 209, "rightCurveLarge1BallastNE" },
			{ 210, "rightCurveLarge2BallastNE" },
			{ 211, "rightCurveLarge3BallastNE" },
			{ 212, "rightCurveLarge4BallastNE" },
			{ 213, "rightCurveLarge0BallastSE" },
			{ 214, "rightCurveLarge1BallastSE" },
			{ 215, "rightCurveLarge2BallastSE" },
			{ 216, "rightCurveLarge3BallastSE" },
			{ 217, "rightCurveLarge4BallastSE" },
			{ 218, "rightCurveLarge0BallastSW" },
			{ 219, "rightCurveLarge1BallastSW" },
			{ 220, "rightCurveLarge2BallastSW" },
			{ 221, "rightCurveLarge3BallastSW" },
			{ 222, "rightCurveLarge4BallastSW" },
			{ 223, "rightCurveLarge0BallastNW" },
			{ 224, "rightCurveLarge1BallastNW" },
			{ 225, "rightCurveLarge2BallastNW" },
			{ 226, "rightCurveLarge3BallastNW" },
			{ 227, "rightCurveLarge4BallastNW" },
			{ 228, "leftCurveLarge0BallastNE" },
			{ 229, "leftCurveLarge1BallastNE" },
			{ 230, "leftCurveLarge2BallastNE" },
			{ 231, "leftCurveLarge3BallastNE" },
			{ 232, "leftCurveLarge4BallastNE" },
			{ 233, "leftCurveLarge0BallastSE" },
			{ 234, "leftCurveLarge1BallastSE" },
			{ 235, "leftCurveLarge2BallastSE" },
			{ 236, "leftCurveLarge3BallastSE" },
			{ 237, "leftCurveLarge4BallastSE" },
			{ 238, "leftCurveLarge0BallastSW" },
			{ 239, "leftCurveLarge1BallastSW" },
			{ 240, "leftCurveLarge2BallastSW" },
			{ 241, "leftCurveLarge3BallastSW" },
			{ 242, "leftCurveLarge4BallastSW" },
			{ 243, "leftCurveLarge0BallastNW" },
			{ 244, "leftCurveLarge1BallastNW" },
			{ 245, "leftCurveLarge2BallastNW" },
			{ 246, "leftCurveLarge3BallastNW" },
			{ 247, "leftCurveLarge4BallastNW" },
			{ 248, "rightCurveLarge0SleeperNE" },
			{ 249, "rightCurveLarge1SleeperNE" },
			{ 250, "rightCurveLarge2SleeperNE" },
			{ 251, "rightCurveLarge3SleeperNE" },
			{ 252, "rightCurveLarge4SleeperNE" },
			{ 253, "rightCurveLarge0SleeperSE" },
			{ 254, "rightCurveLarge1SleeperSE" },
			{ 255, "rightCurveLarge2SleeperSE" },
			{ 256, "rightCurveLarge3SleeperSE" },
			{ 257, "rightCurveLarge4SleeperSE" },
			{ 258, "rightCurveLarge0SleeperSW" },
			{ 259, "rightCurveLarge1SleeperSW" },
			{ 260, "rightCurveLarge2SleeperSW" },
			{ 261, "rightCurveLarge3SleeperSW" },
			{ 262, "rightCurveLarge4SleeperSW" },
			{ 263, "rightCurveLarge0SleeperNW" },
			{ 264, "rightCurveLarge1SleeperNW" },
			{ 265, "rightCurveLarge2SleeperNW" },
			{ 266, "rightCurveLarge3SleeperNW" },
			{ 267, "rightCurveLarge4SleeperNW" },
			{ 268, "leftCurveLarge0SleeperNE" },
			{ 269, "leftCurveLarge1SleeperNE" },
			{ 270, "leftCurveLarge2SleeperNE" },
			{ 271, "leftCurveLarge3SleeperNE" },
			{ 272, "leftCurveLarge4SleeperNE" },
			{ 273, "leftCurveLarge0SleeperSE" },
			{ 274, "leftCurveLarge1SleeperSE" },
			{ 275, "leftCurveLarge2SleeperSE" },
			{ 276, "leftCurveLarge3SleeperSE" },
			{ 277, "leftCurveLarge4SleeperSE" },
			{ 278, "leftCurveLarge0SleeperSW" },
			{ 279, "leftCurveLarge1SleeperSW" },
			{ 280, "leftCurveLarge2SleeperSW" },
			{ 281, "leftCurveLarge3SleeperSW" },
			{ 282, "leftCurveLarge4SleeperSW" },
			{ 283, "leftCurveLarge0SleeperNW" },
			{ 284, "leftCurveLarge1SleeperNW" },
			{ 285, "leftCurveLarge2SleeperNW" },
			{ 286, "leftCurveLarge3SleeperNW" },
			{ 287, "leftCurveLarge4SleeperNW" },
			{ 288, "rightCurveLarge0RailNE" },
			{ 289, "rightCurveLarge1RailNE" },
			{ 290, "rightCurveLarge2RailNE" },
			{ 291, "rightCurveLarge3RailNE" },
			{ 292, "rightCurveLarge4RailNE" },
			{ 293, "rightCurveLarge0RailSE" },
			{ 294, "rightCurveLarge1RailSE" },
			{ 295, "rightCurveLarge2RailSE" },
			{ 296, "rightCurveLarge3RailSE" },
			{ 297, "rightCurveLarge4RailSE" },
			{ 298, "rightCurveLarge0RailSW" },
			{ 299, "rightCurveLarge1RailSW" },
			{ 300, "rightCurveLarge2RailSW" },
			{ 301, "rightCurveLarge3RailSW" },
			{ 302, "rightCurveLarge4RailSW" },
			{ 303, "rightCurveLarge0RailNW" },
			{ 304, "rightCurveLarge1RailNW" },
			{ 305, "rightCurveLarge2RailNW" },
			{ 306, "rightCurveLarge3RailNW" },
			{ 307, "rightCurveLarge4RailNW" },
			{ 308, "leftCurveLarge0RailNE" },
			{ 309, "leftCurveLarge1RailNE" },
			{ 310, "leftCurveLarge2RailNE" },
			{ 311, "leftCurveLarge3RailNE" },
			{ 312, "leftCurveLarge4RailNE" },
			{ 313, "leftCurveLarge0RailSE" },
			{ 314, "leftCurveLarge1RailSE" },
			{ 315, "leftCurveLarge2RailSE" },
			{ 316, "leftCurveLarge3RailSE" },
			{ 317, "leftCurveLarge4RailSE" },
			{ 318, "leftCurveLarge0RailSW" },
			{ 319, "leftCurveLarge1RailSW" },
			{ 320, "leftCurveLarge2RailSW" },
			{ 321, "leftCurveLarge3RailSW" },
			{ 322, "leftCurveLarge4RailSW" },
			{ 323, "leftCurveLarge0RailNW" },
			{ 324, "leftCurveLarge1RailNW" },
			{ 325, "leftCurveLarge2RailNW" },
			{ 326, "leftCurveLarge3RailNW" },
			{ 327, "leftCurveLarge4RailNW" },
			{ 328, "diagonal0BallastNE" },
			{ 329, "diagonal1BallastSW" },
			{ 330, "diagonal1BallastNE" },
			{ 331, "diagonal0BallastSW" },
			{ 332, "diagonal0BallastSE" },
			{ 333, "diagonal1BallastNW" },
			{ 334, "diagonal1BallastSE" },
			{ 335, "diagonal0BallastNW" },
			{ 336, "diagonal0SleeperNE" },
			{ 337, "diagonal1SleeperSW" },
			{ 338, "diagonal1SleeperNE" },
			{ 339, "diagonal0SleeperSW" },
			{ 340, "diagonal0SleeperSE" },
			{ 341, "diagonal1SleeperNW" },
			{ 342, "diagonal1SleeperSE" },
			{ 343, "diagonal0SleeperNW" },
			{ 344, "diagonal0RailNE" },
			{ 345, "diagonal1RailSW" },
			{ 346, "diagonal1RailNE" },
			{ 347, "diagonal0RailSW" },
			{ 348, "diagonal0RailSE" },
			{ 349, "diagonal1RailNW" },
			{ 350, "diagonal1RailSE" },
			{ 351, "diagonal0RailNW" },
			{ 352, "sBendLeft0BallastNE" },
			{ 353, "sBendLeft1BallastNE" },
			{ 354, "sBendLeft1BallastSW" },
			{ 355, "sBendLeft0BallastSW" },
			{ 356, "sBendLeft0BallastSE" },
			{ 357, "sBendLeft1BallastSE" },
			{ 358, "sBendLeft1BallastNW" },
			{ 359, "sBendLeft0BallastNW" },
			{ 360, "sBendRight0BallastNE" },
			{ 361, "sBendRight1BallastNE" },
			{ 362, "sBendRight1BallastSW" },
			{ 363, "sBendRight0BallastSW" },
			{ 364, "sBendRight0BallastSE" },
			{ 365, "sBendRight1BallastSE" },
			{ 366, "sBendRight1BallastNW" },
			{ 367, "sBendRight0BallastNW" },
			{ 368, "sBendLeft0SleeperNE" },
			{ 369, "sBendLeft1SleeperNE" },
			{ 370, "sBendLeft1SleeperSW" },
			{ 371, "sBendLeft0SleeperSW" },
			{ 372, "sBendLeft0SleeperSE" },
			{ 373, "sBendLeft1SleeperSE" },
			{ 374, "sBendLeft1SleeperNW" },
			{ 375, "sBendLeft0SleeperNW" },
			{ 376, "sBendRight0SleeperNE" },
			{ 377, "sBendRight1SleeperNE" },
			{ 378, "sBendRight1SleeperSW" },
			{ 379, "sBendRight0SleeperSW" },
			{ 380, "sBendRight0SleeperSE" },
			{ 381, "sBendRight1SleeperSE" },
			{ 382, "sBendRight1SleeperNW" },
			{ 383, "sBendRight0SleeperNW" },
			{ 384, "sBendLeft0RailNE" },
			{ 385, "sBendLeft1RailNE" },
			{ 386, "sBendLeft1RailSW" },
			{ 387, "sBendLeft0RailSW" },
			{ 388, "sBendLeft0RailSE" },
			{ 389, "sBendLeft1RailSE" },
			{ 390, "sBendLeft1RailNW" },
			{ 391, "sBendLeft0RailNW" },
			{ 392, "sBendRight0RailNE" },
			{ 393, "sBendRight1RailNE" },
			{ 394, "sBendRight1RailSW" },
			{ 395, "sBendRight0RailSW" },
			{ 396, "sBendRight0RailSE" },
			{ 397, "sBendRight1RailSE" },
			{ 398, "sBendRight1RailNW" },
			{ 399, "sBendRight0RailNW" },
			{ 400, "rightCurveVerySmall0BallastNE" },
			{ 401, "rightCurveVerySmall0BallastSE" },
			{ 402, "rightCurveVerySmall0BallastSW" },
			{ 403, "rightCurveVerySmall0BallastNW" },
			{ 404, "rightCurveVerySmall0SleeperNE" },
			{ 405, "rightCurveVerySmall0SleeperSE" },
			{ 406, "rightCurveVerySmall0SleeperSW" },
			{ 407, "rightCurveVerySmall0SleeperNW" },
			{ 408, "rightCurveVerySmall0RailNE" },
			{ 409, "rightCurveVerySmall0RailSE" },
			{ 410, "rightCurveVerySmall0RailSW" },
			{ 411, "rightCurveVerySmall0RailNW" },
		};

		// ai generated - nice idea, maybe implement?
		//public static TrackObject FromDatFile(DatFile datFile, int index)
		//{
		//	var trackObject = datFile.Get<TrackObject>(index);

		//	trackObject.Compatible = datFile.GetVariableCountS5Headers(trackObject.NumCompatible, index, 0);
		//	trackObject.Mods = datFile.GetVariableCountS5Headers(trackObject.NumMods, index, 1);
		//	trackObject.Signals = datFile.GetVariableCountS5Headers(trackObject.NumSignals, index, 2);
		//	trackObject.Tunnel = datFile.GetVariableCountS5Headers(1, index, 3)[0];
		//	trackObject.Bridges = datFile.GetVariableCountS5Headers(trackObject.NumBridges, index, 4);
		//	trackObject.Stations = datFile.GetVariableCountS5Headers(trackObject.NumStations, index, 5);

		//	return trackObject;
		//}
	}
}
