using Definitions.ObjectModels.Objects.Track;
using Definitions.ObjectModels.Types;

namespace Definitions.ObjectModels.Objects.TrackStation;

public class CargoOffset
{
	public Pos3 A { get; set; }
	public Pos3 B { get; set; }
}

public class TrackStationObject : ILocoStruct, IImageTableNameProvider
{
	public uint8_t PaintStyle { get; set; }
	public uint8_t Height { get; set; }
	public TrackTraitFlags TrackPieces { get; set; }
	public int16_t BuildCostFactor { get; set; }
	public int16_t SellCostFactor { get; set; }
	public uint8_t CostIndex { get; set; }
	public TrackStationObjectFlags Flags { get; set; }
	public uint16_t DesignedYear { get; set; }
	public uint16_t ObsoleteYear { get; set; }
	public uint8_t var_0B { get; set; }
	public uint8_t var_0D { get; set; }

	public List<ObjectModelHeader> CompatibleTrackObjects { get; set; } = [];
	public uint8_t[][][] CargoOffsetBytes { get; set; }
	public uint8_t[][] ManualPower { get; set; }
	public CargoOffset[] CargoOffsets { get; init; } = [.. Enumerable.Repeat(new CargoOffset(), 15)];

	public bool Validate()
	{
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

		if (PaintStyle >= 1)
		{
			return false;
		}

		return true; //CompatibleTrackObjectCount <= TrackStationObjectLoader.Constants.MaxNumCompatible;
	}

	public bool TryGetImageName(int id, out string? value)
		=> ImageIdNameMap.TryGetValue(id, out value);

	public static readonly Dictionary<int, string> ImageIdNameMap = new()
	{
		{ 0, "preview_image" },
		{ 1, "preview_image_windows" },
		{ 2, "totalPreviewImages" },
	};

	// These are relative to ImageOffsets
	// ImageOffsets is the imageIds per sequenceIndex (for start/middle/end of the platform)
	//namespace Style0
	//{
	//    constexpr uint32_t straightBackNE = 0;
	//    constexpr uint32_t straightFrontNE = 1;
	//    constexpr uint32_t straightCanopyNE = 2;
	//    constexpr uint32_t straightCanopyTranslucentNE = 3;
	//    constexpr uint32_t straightBackSE = 4;
	//    constexpr uint32_t straightFrontSE = 5;
	//    constexpr uint32_t straightCanopySE = 6;
	//    constexpr uint32_t straightCanopyTranslucentSE = 7;
	//    constexpr uint32_t diagonalNE0 = 8;
	//    constexpr uint32_t diagonalNE3 = 9;
	//    constexpr uint32_t diagonalNE1 = 10;
	//    constexpr uint32_t diagonalCanopyNE1 = 11;
	//    constexpr uint32_t diagonalCanopyTranslucentNE1 = 12;
	//    constexpr uint32_t diagonalSE1 = 13;
	//    constexpr uint32_t diagonalSE2 = 14;
	//    constexpr uint32_t diagonalSE3 = 15;
	//    constexpr uint32_t diagonalCanopyTranslucentSE3 = 16;
	//    constexpr uint32_t totalNumImages = 17;
	//}
	//namespace Style1
	//{
	//    constexpr uint32_t totalNumImages = 8;
	//}
}
