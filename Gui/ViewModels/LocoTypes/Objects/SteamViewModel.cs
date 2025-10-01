using Definitions.ObjectModels.Objects.Steam;
using Definitions.ObjectModels.Types;
using PropertyModels.ComponentModel.DataAnnotations;
using System.ComponentModel;

namespace Gui.ViewModels;

public class SteamViewModel(SteamObject model)
		: LocoObjectViewModel<SteamObject>(model)
{
	public uint8_t NumStationaryTicks
	{
		get => Model.NumStationaryTicks;
		set => Model.NumStationaryTicks = value;
	}

	[EnumProhibitValues<SteamObjectFlags>(SteamObjectFlags.None)]
	public SteamObjectFlags Flags
	{
		get => Model.Flags;
		set => Model.Flags = value;
	}

	public uint32_t var_0A
	{
		get => Model.var_0A;
		set => Model.var_0A = value;
	}

	public BindingList<ObjectModelHeader> SoundEffects { get; init; } = new(model.SoundEffects);

	public BindingList<SteamImageAndHeight> FrameInfoType0 { get; init; } = new(model.FrameInfoType0);

	public BindingList<SteamImageAndHeight> FrameInfoType1 { get; init; } = new(model.FrameInfoType1);
}
