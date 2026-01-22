using Definitions.ObjectModels.Objects.Wall;
using PropertyModels.ComponentModel.DataAnnotations;

namespace Gui.ViewModels;

public class WallViewModel(WallObject model)
	: LocoObjectViewModel<WallObject>(model)
{
	public uint8_t Height
	{
		get => Model.Height;
		set => Model.Height = value;
	}

	public uint8_t ToolId
	{
		get => Model.ToolId;
		set => Model.ToolId = value;
	} // unused in loco???

	[EnumProhibitValues<WallObjectFlags1>(WallObjectFlags1.None)]
	public WallObjectFlags1 Flags1
	{
		get => Model.Flags1;
		set => Model.Flags1 = value;
	}

	[EnumProhibitValues<WallObjectFlags2>(WallObjectFlags2.None)]
	public WallObjectFlags2 Flags2
	{
		get => Model.Flags2;
		set => Model.Flags2 = value;
	}
}
