using Definitions.ObjectModels.Objects.Steam;
using PropertyModels.ComponentModel.DataAnnotations;
using System.Collections.ObjectModel;

namespace Gui.ViewModels;

public class SteamViewModel : LocoObjectViewModel<SteamObject>
{
	public uint8_t NumStationaryTicks { get; set; }
	[EnumProhibitValues<SteamObjectFlags>(SteamObjectFlags.None)] public SteamObjectFlags Flags { get; set; }
	public uint32_t var_0A { get; set; }
	public ObservableCollection<ObjectModelHeaderViewModel> SoundEffects { get; set; }
	public ObservableCollection<SteamImageAndHeight> FrameInfoType0 { get; set; } // may need viewmodel for ImageAndHeight
	public ObservableCollection<SteamImageAndHeight> FrameInfoType1 { get; set; }

	public SteamViewModel(SteamObject model)
		: base(model)
	{
		NumStationaryTicks = model.NumStationaryTicks;
		Flags = model.Flags;
		var_0A = model.var_0A;
		SoundEffects = new(model.SoundEffects.ConvertAll(x => new ObjectModelHeaderViewModel(x)));
		FrameInfoType0 = new(model.FrameInfoType0);
		FrameInfoType1 = new(model.FrameInfoType1);
	}

	public SteamObject CopyBackToModel()
		=> new()
		{
			NumStationaryTicks = NumStationaryTicks,
			Flags = Flags,
			var_0A = var_0A,
			//SoundEffects = SoundEffects.ToList().ConvertAll(x => x.CopyBackToModel()),
			FrameInfoType0 = [.. FrameInfoType0],
			FrameInfoType1 = [.. FrameInfoType1],
		};
}
