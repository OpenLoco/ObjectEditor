using OpenLoco.Dat.Objects;
using PropertyModels.ComponentModel.DataAnnotations;
using ReactiveUI.Fody.Helpers;

namespace OpenLoco.Gui.ViewModels
{
	public class WallViewModel : LocoObjectViewModel<WallObject>
	{
		[Reactive] public uint8_t Height { get; set; }
		[Reactive] public uint8_t ToolId { get; set; }

		[Reactive, EnumProhibitValues<WallObjectFlags>(WallObjectFlags.None)] public WallObjectFlags Flags { get; set; }
		[Reactive, EnumProhibitValues<WallObjectFlags2>(WallObjectFlags2.None)] public WallObjectFlags2 Flags2 { get; set; }

		public WallViewModel(WallObject ro)
		{
			Height = ro.Height;
			ToolId = ro.ToolId;
			Flags = ro.Flags;
			Flags2 = ro.Flags2;
		}

		public override WallObject GetAsStruct(WallObject tso)
			=> tso with
			{
				Height = Height,
				ToolId = ToolId,
				Flags = Flags,
				Flags2 = Flags2,
			};
	}
}
