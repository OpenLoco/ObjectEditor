using Definitions.ObjectModels.Objects.HillShape;

namespace Gui.ViewModels;

public class HillShapesViewModel(HillShapesObject model)
	: BaseViewModel<HillShapesObject>(model)
{
	public uint8_t HillHeightMapCount
	{
		get => Model.HillHeightMapCount;
		set => Model.HillHeightMapCount = value;
	}

	public uint8_t MountainHeightMapCount
	{
		get => Model.MountainHeightMapCount;
		set => Model.MountainHeightMapCount = value;
	}

	public bool IsHeightMap
	{
		get => Model.IsHeightMap;
		set => Model.IsHeightMap = value;
	}
}
