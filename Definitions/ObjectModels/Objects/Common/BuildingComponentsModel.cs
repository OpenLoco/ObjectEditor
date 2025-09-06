using System.ComponentModel;

namespace Definitions.ObjectModels.Objects.Common;

public interface IHasBuildingComponents
{
	BuildingComponentsModel BuildingComponents { get; set; }
}

[TypeConverter(typeof(ExpandableObjectConverter))]
public class BuildingComponentsModel
{
	public List<uint8_t> BuildingHeights { get; set; } = [];
	public List<BuildingPartAnimation> BuildingAnimations { get; set; } = [];
	public List<List<uint8_t>> BuildingVariations { get; set; } = [];

	public bool Validate()
		=> BuildingHeights.Count is not 0 and not > 63
		&& BuildingAnimations.Count is not 0 and not > 63
		&& BuildingHeights.Count == BuildingAnimations.Count
		&& BuildingVariations.Count is not 0 and <= 31;
}
