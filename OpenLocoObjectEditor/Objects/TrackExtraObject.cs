﻿
using System.ComponentModel;
using OpenLocoObjectEditor.DatFileParsing;
using OpenLocoObjectEditor.Headers;
using OpenLocoObjectEditor.Types;

namespace OpenLocoObjectEditor.Objects
{
	[TypeConverter(typeof(ExpandableObjectConverter))]
	[LocoStructSize(0x12)]
	[LocoStructType(ObjectType.TrackExtra)]
	[LocoStringTable("Name")]
	public record TrackExtraObject(
		[property: LocoStructOffset(0x00), LocoString, Browsable(false)] string_id Name,
		[property: LocoStructOffset(0x02)] uint16_t TrackPieces,
		[property: LocoStructOffset(0x04)] uint8_t PaintStyle,
		[property: LocoStructOffset(0x05)] uint8_t CostIndex,
		[property: LocoStructOffset(0x06)] int16_t BuildCostFactor,
		[property: LocoStructOffset(0x08)] int16_t SellCostFactor,
		[property: LocoStructOffset(0x0A), Browsable(false)] image_id Image,
		[property: LocoStructOffset(0x0E), Browsable(false)] image_id var_0E)
	: ILocoStruct, IImageTableStrings
	{
		public bool TryGetImageName(int id, out string? value)
			=> ImageIdNameMap.TryGetValue(id - 8, out value);

		// taken from OpenLoco TrackExtraObject.h
		public static Dictionary<int, string> ImageIdNameMap = new()
		{
			{ -8, "previewImage0" },
			{ -7, "previewImage1" },
			{ -6, "previewImage2" },
			{ -5, "previewImage3" },
			{ -4, "previewImage4" },
			{ -3, "previewImage5" },
			{ -2, "previewImage6" },
			{ -1, "previewImage7" },
			{ 0, "kStraight0NE" },
			{ 1, "kStraight0SE" },
			{ 2, "kStraight0SW" },
			{ 3, "kStraight0NW" },
			{ 4, "kRightCurveSmall0NE" },
			{ 5, "kRightCurveSmall1NE" },
			{ 6, "kRightCurveSmall2NE" },
			{ 7, "kRightCurveSmall3NE" },
			{ 8, "kRightCurveSmall0SE" },
			{ 9, "kRightCurveSmall1SE" },
			{ 10, "kRightCurveSmall2SE" },
			{ 11, "kRightCurveSmall3SE" },
			{ 12, "kRightCurveSmall0SW" },
			{ 13, "kRightCurveSmall1SW" },
			{ 14, "kRightCurveSmall2SW" },
			{ 15, "kRightCurveSmall3SW" },
			{ 16, "kRightCurveSmall0NW" },
			{ 17, "kRightCurveSmall1NW" },
			{ 18, "kRightCurveSmall2NW" },
			{ 19, "kRightCurveSmall3NW" },
			{ 20, "kRightCurve0NE" },
			{ 21, "kRightCurve1NE" },
			{ 22, "kRightCurve2NE" },
			{ 23, "kRightCurve3NE" },
			{ 24, "kRightCurve4NE" },
			{ 25, "kRightCurve0SE" },
			{ 26, "kRightCurve1SE" },
			{ 27, "kRightCurve2SE" },
			{ 28, "kRightCurve3SE" },
			{ 29, "kRightCurve4SE" },
			{ 30, "kRightCurve0SW" },
			{ 31, "kRightCurve1SW" },
			{ 32, "kRightCurve2SW" },
			{ 33, "kRightCurve3SW" },
			{ 34, "kRightCurve4SW" },
			{ 35, "kRightCurve0NW" },
			{ 36, "kRightCurve1NW" },
			{ 37, "kRightCurve2NW" },
			{ 38, "kRightCurve3NW" },
			{ 39, "kRightCurve4NW" },
			{ 40, "kSBendLeft0NE" },
			{ 41, "kSBendLeft1NE" },
			{ 42, "kSBendLeft2NE" },
			{ 43, "kSBendLeft3NE" },
			{ 44, "kSBendLeft0SE" },
			{ 45, "kSBendLeft1SE" },
			{ 46, "kSBendLeft2SE" },
			{ 47, "kSBendLeft3SE" },
			{ 48, "kSBendLeft3SW" },
			{ 49, "kSBendLeft2SW" },
			{ 50, "kSBendLeft1SW" },
			{ 51, "kSBendLeft0SW" },
			{ 52, "kSBendLeft3NW" },
			{ 53, "kSBendLeft2NW" },
			{ 54, "kSBendLeft1NW" },
			{ 55, "kSBendLeft0NW" },
			{ 56, "kSBendRight0NE" },
			{ 57, "kSBendRight1NE" },
			{ 58, "kSBendRight2NE" },
			{ 59, "kSBendRight3NE" },
			{ 60, "kSBendRight0SE" },
			{ 61, "kSBendRight1SE" },
			{ 62, "kSBendRight2SE" },
			{ 63, "kSBendRight3SE" },
			{ 64, "kSBendRight3SW" },
			{ 65, "kSBendRight2SW" },
			{ 66, "kSBendRight1SW" },
			{ 67, "kSBendRight0SW" },
			{ 68, "kSBendRight3NW" },
			{ 69, "kSBendRight2NW" },
			{ 70, "kSBendRight1NW" },
			{ 71, "kSBendRight0NW" },
			{ 72, "kStraightSlopeUp0NE" },
			{ 73, "kStraightSlopeUp1NE" },
			{ 74, "kStraightSlopeUp0SE" },
			{ 75, "kStraightSlopeUp1SE" },
			{ 76, "kStraightSlopeUp0SW" },
			{ 77, "kStraightSlopeUp1SW" },
			{ 78, "kStraightSlopeUp0NW" },
			{ 79, "kStraightSlopeUp1NW" },
			{ 80, "kStraightSteepSlopeUp0NE" },
			{ 81, "kStraightSteepSlopeUp0SE" },
			{ 82, "kStraightSteepSlopeUp0SW" },
			{ 83, "kStraightSteepSlopeUp0NW" },
			{ 84, "kRightCurveSmallSlopeUp0NE" },
			{ 85, "kRightCurveSmallSlopeUp1NE" },
			{ 86, "kRightCurveSmallSlopeUp2NE" },
			{ 87, "kRightCurveSmallSlopeUp3NE" },
			{ 88, "kRightCurveSmallSlopeUp0SE" },
			{ 89, "kRightCurveSmallSlopeUp1SE" },
			{ 90, "kRightCurveSmallSlopeUp2SE" },
			{ 91, "kRightCurveSmallSlopeUp3SE" },
			{ 92, "kRightCurveSmallSlopeUp0SW" },
			{ 93, "kRightCurveSmallSlopeUp1SW" },
			{ 94, "kRightCurveSmallSlopeUp2SW" },
			{ 95, "kRightCurveSmallSlopeUp3SW" },
			{ 96, "kRightCurveSmallSlopeUp0NW" },
			{ 97, "kRightCurveSmallSlopeUp1NW" },
			{ 98, "kRightCurveSmallSlopeUp2NW" },
			{ 99, "kRightCurveSmallSlopeUp3NW" },
			{ 100, "kRightCurveSmallSlopeDown0NE" },
			{ 101, "kRightCurveSmallSlopeDown1NE" },
			{ 102, "kRightCurveSmallSlopeDown2NE" },
			{ 103, "kRightCurveSmallSlopeDown3NE" },
			{ 104, "kRightCurveSmallSlopeDown0SE" },
			{ 105, "kRightCurveSmallSlopeDown1SE" },
			{ 106, "kRightCurveSmallSlopeDown2SE" },
			{ 107, "kRightCurveSmallSlopeDown3SE" },
			{ 108, "kRightCurveSmallSlopeDown0SW" },
			{ 109, "kRightCurveSmallSlopeDown1SW" },
			{ 110, "kRightCurveSmallSlopeDown2SW" },
			{ 111, "kRightCurveSmallSlopeDown3SW" },
			{ 112, "kRightCurveSmallSlopeDown0NW" },
			{ 113, "kRightCurveSmallSlopeDown1NW" },
			{ 114, "kRightCurveSmallSlopeDown2NW" },
			{ 115, "kRightCurveSmallSlopeDown3NW" },
			{ 116, "kRightCurveSmallSteepSlopeUp0NE" },
			{ 117, "kRightCurveSmallSteepSlopeUp1NE" },
			{ 118, "kRightCurveSmallSteepSlopeUp2NE" },
			{ 119, "kRightCurveSmallSteepSlopeUp3NE" },
			{ 120, "kRightCurveSmallSteepSlopeUp0SE" },
			{ 121, "kRightCurveSmallSteepSlopeUp1SE" },
			{ 122, "kRightCurveSmallSteepSlopeUp2SE" },
			{ 123, "kRightCurveSmallSteepSlopeUp3SE" },
			{ 124, "kRightCurveSmallSteepSlopeUp0SW" },
			{ 125, "kRightCurveSmallSteepSlopeUp1SW" },
			{ 126, "kRightCurveSmallSteepSlopeUp2SW" },
			{ 127, "kRightCurveSmallSteepSlopeUp3SW" },
			{ 128, "kRightCurveSmallSteepSlopeUp0NW" },
			{ 129, "kRightCurveSmallSteepSlopeUp1NW" },
			{ 130, "kRightCurveSmallSteepSlopeUp2NW" },
			{ 131, "kRightCurveSmallSteepSlopeUp3NW" },
			{ 132, "kRightCurveSmallSteepSlopeDown0NE" },
			{ 133, "kRightCurveSmallSteepSlopeDown1NE" },
			{ 134, "kRightCurveSmallSteepSlopeDown2NE" },
			{ 135, "kRightCurveSmallSteepSlopeDown3NE" },
			{ 136, "kRightCurveSmallSteepSlopeDown0SE" },
			{ 137, "kRightCurveSmallSteepSlopeDown1SE" },
			{ 138, "kRightCurveSmallSteepSlopeDown2SE" },
			{ 139, "kRightCurveSmallSteepSlopeDown3SE" },
			{ 140, "kRightCurveSmallSteepSlopeDown0SW" },
			{ 141, "kRightCurveSmallSteepSlopeDown1SW" },
			{ 142, "kRightCurveSmallSteepSlopeDown2SW" },
			{ 143, "kRightCurveSmallSteepSlopeDown3SW" },
			{ 144, "kRightCurveSmallSteepSlopeDown0NW" },
			{ 145, "kRightCurveSmallSteepSlopeDown1NW" },
			{ 146, "kRightCurveSmallSteepSlopeDown2NW" },
			{ 147, "kRightCurveSmallSteepSlopeDown3NW" },
			{ 148, "kRightCurveLarge0NE" },
			{ 149, "kRightCurveLarge1NE" },
			{ 150, "kRightCurveLarge2NE" },
			{ 151, "kRightCurveLarge3NE" },
			{ 152, "kRightCurveLarge4NE" },
			{ 153, "kRightCurveLarge0SE" },
			{ 154, "kRightCurveLarge1SE" },
			{ 155, "kRightCurveLarge2SE" },
			{ 156, "kRightCurveLarge3SE" },
			{ 157, "kRightCurveLarge4SE" },
			{ 158, "kRightCurveLarge0SW" },
			{ 159, "kRightCurveLarge1SW" },
			{ 160, "kRightCurveLarge2SW" },
			{ 161, "kRightCurveLarge3SW" },
			{ 162, "kRightCurveLarge4SW" },
			{ 163, "kRightCurveLarge0NW" },
			{ 164, "kRightCurveLarge1NW" },
			{ 165, "kRightCurveLarge2NW" },
			{ 166, "kRightCurveLarge3NW" },
			{ 167, "kRightCurveLarge4NW" },
			{ 168, "kLeftCurveLarge0NE" },
			{ 169, "kLeftCurveLarge1NE" },
			{ 170, "kLeftCurveLarge2NE" },
			{ 171, "kLeftCurveLarge3NE" },
			{ 172, "kLeftCurveLarge4NE" },
			{ 173, "kLeftCurveLarge0SE" },
			{ 174, "kLeftCurveLarge1SE" },
			{ 175, "kLeftCurveLarge2SE" },
			{ 176, "kLeftCurveLarge3SE" },
			{ 177, "kLeftCurveLarge4SE" },
			{ 178, "kLeftCurveLarge0SW" },
			{ 179, "kLeftCurveLarge1SW" },
			{ 180, "kLeftCurveLarge2SW" },
			{ 181, "kLeftCurveLarge3SW" },
			{ 182, "kLeftCurveLarge4SW" },
			{ 183, "kLeftCurveLarge0NW" },
			{ 184, "kLeftCurveLarge1NW" },
			{ 185, "kLeftCurveLarge2NW" },
			{ 186, "kLeftCurveLarge3NW" },
			{ 187, "kLeftCurveLarge4NW" },
			{ 188, "kDiagonal0NE" },
			{ 189, "kDiagonal2NE" },
			{ 190, "kDiagonal1NE" },
			{ 191, "kDiagonal3NE" },
			{ 192, "kDiagonal0SE" },
			{ 193, "kDiagonal2SE" },
			{ 194, "kDiagonal1SE" },
			{ 195, "kDiagonal3SE" },
			{ 196, "kDiagonal0SW" },
			{ 197, "kDiagonal2SW" },
			{ 198, "kDiagonal1SW" },
			{ 199, "kDiagonal3SW" },
			{ 200, "kDiagonal0NW" },
			{ 201, "kDiagonal2NW" },
			{ 202, "kDiagonal1NW" },
			{ 203, "kDiagonal3NW" },
			{ 204, "kRightCurveVerySmall0NE" },
			{ 205, "kRightCurveVerySmall0SE" },
			{ 206, "kRightCurveVerySmall0SW" },
			{ 207, "kRightCurveVerySmall0NW" },
		};
	}
}
