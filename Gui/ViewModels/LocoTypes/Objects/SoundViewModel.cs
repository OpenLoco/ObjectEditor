using Definitions.ObjectModels.Objects.Sound;
using System.ComponentModel;

namespace Gui.ViewModels;

public class SoundViewModel(SoundObject model)
	: LocoObjectViewModel<SoundObject>(model)
{
	public uint8_t ShouldLoop
	{
		get => Model.ShouldLoop;
		set => Model.ShouldLoop = value;
	}

	public uint32_t Volume
	{
		get => Model.Volume;
		set => Model.Volume = value;
	}

	public SoundObjectData SoundObjectData
	{
		get => Model.SoundObjectData;
		set => Model.SoundObjectData = value;
	}

	[Browsable(false)]
	public byte[] PcmData
	{
		get => Model.PcmData;
		set => Model.PcmData = value;
	}

	public uint32_t NumUnkStructs
	{
		get => Model.NumUnkStructs;
		set => Model.NumUnkStructs = value;
	}

	[Browsable(false)]
	public byte[] UnkData
	{
		get => Model.UnkData;
		set => Model.UnkData = value;
	}
}
