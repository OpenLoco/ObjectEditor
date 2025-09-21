using Definitions.ObjectModels.Objects.Sound;
using System.ComponentModel;

namespace Gui.ViewModels;

public class SoundViewModel : LocoObjectViewModel<SoundObject>
{
	public uint8_t ShouldLoop { get; set; }
	public uint32_t Volume { get; set; }
	public SoundObjectData SoundObjectData { get; set; }
	[Browsable(false)] public byte[] PcmData { get; set; }
	public uint32_t NumUnkStructs { get; set; }
	[Browsable(false)] public byte[] UnkData { get; set; }

	public SoundViewModel(SoundObject model)
		: base(model)
	{
		ShouldLoop = model.ShouldLoop;
		Volume = model.Volume;
		SoundObjectData = model.SoundObjectData;
		PcmData = model.PcmData;
		NumUnkStructs = model.NumUnkStructs;
		UnkData = model.UnkData;
	}

	public SoundObject CopyBackToModel()
		=> new()
		{
			ShouldLoop = ShouldLoop,
			Volume = Volume,
			SoundObjectData = SoundObjectData,
			PcmData = PcmData,
			NumUnkStructs = NumUnkStructs,
			UnkData = UnkData,
		};
}
