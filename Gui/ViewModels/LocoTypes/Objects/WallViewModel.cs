using Definitions.ObjectModels.Objects.Wall;
using PropertyModels.ComponentModel.DataAnnotations;

namespace Gui.ViewModels;

public class WallViewModel : LocoObjectViewModel<WallObject>
{
	public uint8_t Height { get; set; }
	public uint8_t ToolId { get; set; } // unused in loco???

	[EnumProhibitValues<WallObjectFlags1>(WallObjectFlags1.None)] public WallObjectFlags1 Flags1 { get; set; }
	[EnumProhibitValues<WallObjectFlags2>(WallObjectFlags2.None)] public WallObjectFlags2 Flags2 { get; set; }

	public WallViewModel(WallObject ro)
	{
		Height = ro.Height;
		ToolId = ro.ToolId;
		Flags1 = ro.Flags1;
		Flags2 = ro.Flags2;
	}

	public override WallObject GetAsModel()
		=> new()
		{
			Height = Height,
			ToolId = ToolId,
			Flags1 = Flags1,
			Flags2 = Flags2,
		};
}
