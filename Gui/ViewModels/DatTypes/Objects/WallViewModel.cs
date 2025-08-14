using Definitions.ObjectModels.Objects.Wall;
using PropertyModels.ComponentModel.DataAnnotations;
using ReactiveUI.Fody.Helpers;

namespace Gui.ViewModels;

public class WallViewModel : LocoObjectViewModel<WallObject>
{
	[Reactive] public uint8_t Height { get; set; }

	[Reactive, EnumProhibitValues<WallObjectFlags1>(WallObjectFlags1.None)] public WallObjectFlags1 Flags1 { get; set; }
	[Reactive, EnumProhibitValues<WallObjectFlags2>(WallObjectFlags2.None)] public WallObjectFlags2 Flags2 { get; set; }

	public WallViewModel(WallObject ro)
	{
		Height = ro.Height;
		Flags1 = ro.Flags1;
		Flags2 = ro.Flags2;
	}

	public override WallObject GetAsStruct()
		=> new()
		{
			Height = Height,
			Flags1 = Flags1,
			Flags2 = Flags2,
		};
}
