using Definitions.ObjectModels.Objects.Road;
using Definitions.ObjectModels.Types;

namespace Definitions.ObjectModels.Objects.RoadStation;

public class RoadStationObject : ILocoStruct, IImageTableNameProvider
{
	public uint8_t PaintStyle { get; set; }
	public uint8_t Height { get; set; }
	public RoadTraitFlags RoadPieces { get; set; }
	public int16_t BuildCostFactor { get; set; }
	public int16_t SellCostFactor { get; set; }
	public uint8_t CostIndex { get; set; }
	public RoadStationObjectFlags Flags { get; set; }
	public uint16_t DesignedYear { get; set; }
	public uint16_t ObsoleteYear { get; set; }

	public List<ObjectModelHeader> CompatibleRoadObjects { get; set; } = [];

	public ObjectModelHeader? CargoType { get; set; }

	// for drawing the cargo on the station
	public uint8_t[][][] CargoOffsetBytes { get; set; }
	//public CargoOffset[] CargoOffsets { get; init; } = [.. Enumerable.Repeat(new CargoOffset { A = Pos3.Zero, B = Pos3.Zero }, 15)];

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

		if (CompatibleRoadObjects.Count > 7)
		{
			return false;
		}

		if (Flags.HasFlag(RoadStationObjectFlags.Passenger) && Flags.HasFlag(RoadStationObjectFlags.Freight))
		{
			return false;
		}

		return true;
	}

	public bool TryGetImageName(int id, out string? value)
		=> ImageIdNameMap.TryGetValue(id, out value);

	public static Dictionary<int, string> ImageIdNameMap = new()
	{
		{ 0, "preview_image" },
		{ 1, "preview_image_windows" },
		{ 2, "totalPreviewImages" },

		// These are relative to ImageOffsets
		// ImageOffsets is the imageIds per sequenceIndex (for start/middle/end of the platform)
		//namespace Style0
		//{
		//	constexpr uint32_t totalNumImages = 8;
		//}
};
}
