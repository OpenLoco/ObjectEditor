using Definitions.ObjectModels.Objects.Steam;
using PropertyModels.ComponentModel.DataAnnotations;
using System.ComponentModel;
using System.Linq;

namespace Gui.ViewModels;

public class SteamViewModel : LocoObjectViewModel<SteamObject>
{
	public uint8_t NumStationaryTicks { get; set; }
	[EnumProhibitValues<SteamObjectFlags>(SteamObjectFlags.None)] public SteamObjectFlags Flags { get; set; }
	public uint32_t var_0A { get; set; }
	public BindingList<ObjectModelHeaderViewModel> SoundEffects { get; set; }
	public BindingList<SteamImageAndHeight> FrameInfoType0 { get; set; } // may need viewmodel for ImageAndHeight
	public BindingList<SteamImageAndHeight> FrameInfoType1 { get; set; }

	public SteamViewModel(SteamObject so)
	{
		NumStationaryTicks = so.NumStationaryTicks;
		Flags = so.Flags;
		var_0A = so.var_0A;
		SoundEffects = new(so.SoundEffects.ConvertAll(x => new ObjectModelHeaderViewModel(x)));
		FrameInfoType0 = new(so.FrameInfoType0);
		FrameInfoType1 = new(so.FrameInfoType1);
	}

	public override SteamObject GetAsModel()
		=> new()
		{
			NumStationaryTicks = NumStationaryTicks,
			Flags = Flags,
			var_0A = var_0A,
			SoundEffects = SoundEffects.ToList().ConvertAll(x => x.GetAsModel()),
			FrameInfoType0 = [.. FrameInfoType0],
			FrameInfoType1 = [.. FrameInfoType1],
		};
}
