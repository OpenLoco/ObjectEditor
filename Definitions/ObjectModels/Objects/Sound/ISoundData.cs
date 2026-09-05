namespace Definitions.ObjectModels.Objects.Sound;

public interface ISoundData : ILocoStruct
{
	SoundObjectData SoundObjectData { get; set; }
	byte[] PcmData { get; set; }
}
