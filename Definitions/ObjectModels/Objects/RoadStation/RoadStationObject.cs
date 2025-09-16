using Definitions.ObjectModels.Objects.Road;
using Definitions.ObjectModels.Objects.Shared;
using Definitions.ObjectModels.Types;
using System.Diagnostics.CodeAnalysis;

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

	//public uint8_t[][][] CargoOffsetBytes { get; set; }
	public CargoOffset[][][] CargoOffsets { get; set; }

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

	public bool TryGetImageName(int id, [MaybeNullWhen(false)] out string value)
		=> ImageIdNameMap.TryGetValue(id, out value);

	public static Dictionary<int, string> ImageIdNameMap = new()
	{
		{ 0, "preview_image" },
		{ 1, "preview_image_glass_overlay" },
		{ 2, "North West Back Wall" },
		{ 3, "North West Front Platform" },
		{ 4, "North West Front Wall/Roof" },
		{ 5, "North West Glass Overlay" },
		{ 6, "South West Back Wall" },
		{ 7, "South West Front Platform" },
		{ 8, "South West Front Wall/Roof" },
		{ 9, "South West Glass Overlay" },
		{ 10, "South East Back Wall" },
		{ 11, "South East Front Platform" },
		{ 12, "South East Front Wall/Roof" },
		{ 13, "South East Glass Overlay" },
		{ 14, "North East Back Wall" },
		{ 15, "North East Front Platform" },
		{ 16, "North East Front Wall/Roof" },
		{ 17, "North East Glass Overlay" },
	};
}
