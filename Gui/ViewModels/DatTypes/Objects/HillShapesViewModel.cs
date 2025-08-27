using Definitions.ObjectModels.Objects.HillShape;

namespace Gui.ViewModels;

public class HillShapesViewModel : LocoObjectViewModel<HillShapesObject>
{
	public uint8_t HillHeightMapCount { get; set; }
	public uint8_t MountainHeightMapCount { get; set; }
	public bool IsHeightMap { get; set; }

	public HillShapesViewModel(HillShapesObject obj)
	{
		HillHeightMapCount = obj.HillHeightMapCount;
		MountainHeightMapCount = obj.MountainHeightMapCount;
		IsHeightMap = obj.IsHeightMap;
	}

	public override HillShapesObject GetAsModel()
		=> new()
		{
			HillHeightMapCount = HillHeightMapCount,
			MountainHeightMapCount = MountainHeightMapCount,
			IsHeightMap = IsHeightMap,
		};
}
